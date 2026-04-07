using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.Environment
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                EventBus.Publish(new PlayerDeathEvent());
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            if (TryGetComponent(out BoxCollider2D col))
                Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.size);
        }
    }
}
