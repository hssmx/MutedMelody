using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.Environment
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Checkpoint : MonoBehaviour
    {
        public string checkpointID;
        public static Vector2 LastCheckpointPosition;

        private bool _isActivated;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!_isActivated && col.CompareTag("Player"))
            {
                _isActivated = true;
                LastCheckpointPosition = transform.position;
                EventBus.Publish(new CheckpointActivatedEvent { CheckpointID = checkpointID, Position = transform.position });
                Debug.Log($"[Checkpoint] {checkpointID} Activated!");
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
            if (TryGetComponent(out BoxCollider2D col))
                Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.size);
        }
    }
}
