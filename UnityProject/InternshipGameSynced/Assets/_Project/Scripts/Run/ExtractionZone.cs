using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class ExtractionZone : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour progressSinkBehaviour;

        private IRunProgressSink progressSink;

        public void Configure(MonoBehaviour sinkBehaviour)
        {
            progressSinkBehaviour = sinkBehaviour;
            progressSink = sinkBehaviour as IRunProgressSink;
        }

        private void Awake()
        {
            progressSink = progressSinkBehaviour as IRunProgressSink;
        }

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            foreach (var behaviour in other.GetComponentsInParent<MonoBehaviour>(true))
            {
                if (behaviour is IRunObjective objective)
                {
                    progressSink?.TryExtract(objective);
                    return;
                }
            }
        }
    }
}
