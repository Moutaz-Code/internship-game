using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public interface IRunSeedCommandTarget
    {
        void NewSeedAndReset();
    }

    public sealed class PrototypeRunController : MonoBehaviour, IRunProgressSink, IRunCommandTarget, IRunFailureSink, IRunSeedCommandTarget
    {
        public const float DefaultRunDurationSeconds = 1200f;

        [SerializeField] private MonoBehaviour objectiveBehaviour;
        [SerializeField] private MonoBehaviour[] stateListenerBehaviours = Array.Empty<MonoBehaviour>();
        [SerializeField, Min(30f)] private float runDurationSeconds = DefaultRunDurationSeconds;
        [SerializeField, Min(1f)] private float extractionCycleSeconds = PrototypeRunStateMachine.DefaultExtractionCycleSeconds;
        [SerializeField] private int initialSeed = 1337;

        private readonly PrototypeRunStateMachine stateMachine = new();
        private IRunObjective objective;
        private FuelRodObjectiveSet objectiveSet;
        private float elapsed;

        public event Action<PrototypeRunState, PrototypeRunState> StateChanged;
        public event Action<RunProgressSnapshot> ProgressChanged;
        public event Action<RunConfiguration> ConfigurationChanged;

        public PrototypeRunState CurrentState => stateMachine.State;
        public float TimeRemaining => Mathf.Max(0f, runDurationSeconds - elapsed);
        public float ExtractionCycleRemaining => stateMachine.ExtractionCycleRemaining;
        public RunProgressSnapshot Progress { get; private set; } = RunProgressSnapshot.Empty();
        public RunConfiguration CurrentConfiguration { get; private set; }
        public string LastFailureReason { get; private set; } = string.Empty;

        public void Configure(MonoBehaviour runObjective, params MonoBehaviour[] stateListeners)
        {
            objectiveBehaviour = runObjective;
            stateListenerBehaviours = stateListeners ?? Array.Empty<MonoBehaviour>();
            BindObjective(runObjective);
        }

        public void ConfigureTiming(float totalRunSeconds, float cycleSeconds)
        {
            if (totalRunSeconds <= 0f || float.IsNaN(totalRunSeconds) || float.IsInfinity(totalRunSeconds))
            {
                throw new ArgumentOutOfRangeException(nameof(totalRunSeconds));
            }

            runDurationSeconds = totalRunSeconds;
            extractionCycleSeconds = cycleSeconds;
            stateMachine.SetExtractionCycleDuration(cycleSeconds);
        }

        public void ConfigureSeed(int seed)
        {
            initialSeed = seed;
            SetConfiguration(RunConfiguration.Generate(seed));
        }

        private void Awake()
        {
            stateMachine.SetExtractionCycleDuration(Mathf.Max(1f, extractionCycleSeconds));
            BindObjective(objectiveBehaviour);
            SetConfiguration(RunConfiguration.Generate(initialSeed));
        }

        private void OnDestroy()
        {
            UnbindObjectiveSet();
        }

        private void Start()
        {
            ResetRun();
        }

        private void Update()
        {
            if (!IsActiveState(stateMachine.State))
            {
                return;
            }

            elapsed += Time.deltaTime;
            if (elapsed >= runDurationSeconds)
            {
                TryFailRun("Facility lockdown timer expired.");
                return;
            }

            if (stateMachine.State == PrototypeRunState.ExtractionCycling)
            {
                var previous = stateMachine.State;
                if (stateMachine.AdvanceExtractionCycle(Time.deltaTime))
                {
                    NotifyStateChanged(previous);
                }
            }
        }

        public void NotifyObjectiveCollected(IRunObjective collectedObjective)
        {
            if (objectiveSet != null)
            {
                return;
            }

            if (!ReferenceEquals(collectedObjective, objective))
            {
                return;
            }

            var previous = stateMachine.State;
            if (stateMachine.CollectFuelRod())
            {
                NotifyStateChanged(previous);
            }
        }

        public void TryExtract(IRunObjective enteringObjective)
        {
            if (objectiveSet != null)
            {
                if (enteringObjective is FuelRodItem item)
                {
                    TryDepositFuelRod(item);
                }

                return;
            }

            if (!ReferenceEquals(enteringObjective, objective))
            {
                return;
            }

            var previous = stateMachine.State;
            if (stateMachine.Extract(enteringObjective.IsCollected))
            {
                NotifyStateChanged(previous);
            }
        }

        public DepositResult TryDepositFuelRod(FuelRodItem item, Transform slot = null)
        {
            if (objectiveSet == null)
            {
                return DepositResult.Unregistered;
            }

            if (stateMachine.State != PrototypeRunState.RetrieveFuelRods)
            {
                return DepositResult.RunInactive;
            }

            return objectiveSet.TryDeposit(item, slot);
        }

        public bool TryEscape()
        {
            var previous = stateMachine.State;
            if (!stateMachine.Escape())
            {
                return false;
            }

            NotifyStateChanged(previous);
            return true;
        }

        public void ResetRun()
        {
            elapsed = 0f;
            LastFailureReason = string.Empty;
            if (objective == null)
            {
                BindObjective(objectiveBehaviour);
            }

            NotifyRunReset();
            objective?.ResetObjective();

            var previous = stateMachine.State;
            if (objectiveSet != null)
            {
                stateMachine.StartMultiRodRun();
                Progress = objectiveSet.Progress;
            }
            else
            {
                stateMachine.StartRun();
                Progress = RunProgressSnapshot.Empty(1);
            }

            ProgressChanged?.Invoke(Progress);
            NotifyStateChanged(previous);
        }

        public void NewSeedAndReset()
        {
            var nextSeed = CurrentConfiguration == null
                ? unchecked(initialSeed + 1)
                : unchecked(CurrentConfiguration.Seed + 1);
            initialSeed = nextSeed;
            // Deposited items must be mutable again before configuration listeners assign
            // their new identities and authored socket poses.
            objective?.ResetObjective();
            SetConfiguration(RunConfiguration.Generate(nextSeed));
            ResetRun();
        }

        public bool TryFailRun(string reason)
        {
            var previous = stateMachine.State;
            if (!stateMachine.Fail())
            {
                return false;
            }

            LastFailureReason = string.IsNullOrWhiteSpace(reason) ? "Mission failed." : reason;
            NotifyStateChanged(previous);
            return true;
        }

        private void BindObjective(MonoBehaviour behaviour)
        {
            UnbindObjectiveSet();
            objective = behaviour as IRunObjective;
            objectiveSet = behaviour as FuelRodObjectiveSet;
            if (objectiveSet == null)
            {
                return;
            }

            objectiveSet.ProgressChanged += OnObjectiveProgressChanged;
            objectiveSet.AllRequiredDeposited += OnAllRequiredDeposited;
            Progress = objectiveSet.Progress;
        }

        private void UnbindObjectiveSet()
        {
            if (objectiveSet == null)
            {
                return;
            }

            objectiveSet.ProgressChanged -= OnObjectiveProgressChanged;
            objectiveSet.AllRequiredDeposited -= OnAllRequiredDeposited;
            objectiveSet = null;
        }

        private void OnObjectiveProgressChanged(RunProgressSnapshot snapshot)
        {
            Progress = snapshot;
            ProgressChanged?.Invoke(snapshot);
        }

        private void OnAllRequiredDeposited()
        {
            var previous = stateMachine.State;
            if (stateMachine.BeginExtractionCycle())
            {
                NotifyStateChanged(previous);
            }
        }

        private void SetConfiguration(RunConfiguration configuration)
        {
            CurrentConfiguration = configuration;
            ConfigurationChanged?.Invoke(configuration);
        }

        private static bool IsActiveState(PrototypeRunState state)
        {
#pragma warning disable CS0618
            return state is PrototypeRunState.RetrieveFuelRods
                or PrototypeRunState.ExtractionCycling
                or PrototypeRunState.EscapeReady
                or PrototypeRunState.FindFuelRod
                or PrototypeRunState.ReturnToExtraction;
#pragma warning restore CS0618
        }

        private void NotifyRunReset()
        {
            foreach (var behaviour in stateListenerBehaviours)
            {
                if (behaviour is not IRunStateListener listener)
                {
                    continue;
                }

                try
                {
                    listener.OnRunReset();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, behaviour);
                }
            }
        }

        private void NotifyStateChanged(PrototypeRunState previous)
        {
            var current = stateMachine.State;
            foreach (var behaviour in stateListenerBehaviours)
            {
                if (behaviour is not IRunStateListener listener)
                {
                    continue;
                }

                try
                {
                    listener.OnRunStateChanged(previous, current);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, behaviour);
                }
            }

            StateChanged?.Invoke(previous, current);
        }
    }
}
