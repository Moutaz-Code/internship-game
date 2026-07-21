using UnityEngine;

namespace InternshipGame.Prototype
{
    public interface IRunObjective
    {
        string ObjectiveId { get; }
        RunProgressSnapshot Progress { get; }
        bool IsComplete { get; }
        bool IsCollected { get; }
        void ResetObjective();
    }

    public interface IRunProgressSink
    {
        void NotifyObjectiveCollected(IRunObjective objective);
        void TryExtract(IRunObjective objective);
    }

    public interface IRunStateListener
    {
        void OnRunReset();
        void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current);
    }

    public interface IRunCommandTarget
    {
        void ResetRun();
    }

    public interface IRunFailureSink
    {
        bool TryFailRun(string reason);
    }

    public interface IPlayerContext
    {
        PlayerControlMode ActiveMode { get; }
        Transform PlayerRoot { get; }
        Transform ViewTransform { get; }
        Transform InteractionOrigin { get; }
    }

    public interface IMapContext
    {
        Vector3 PlayerSpawnPosition { get; }
        Quaternion PlayerSpawnRotation { get; }
    }

    public readonly struct PlayerInteractionContext
    {
        public PlayerInteractionContext(IPlayerContext player)
        {
            Player = player;
        }

        public IPlayerContext Player { get; }
    }

    public interface IPlayerInteractable
    {
        string InteractionPrompt { get; }
        bool CanInteract(in PlayerInteractionContext context);
        void Interact(in PlayerInteractionContext context);
    }

    public interface IPrototypeStatusProvider
    {
        string StatusLabel { get; }
    }
}
