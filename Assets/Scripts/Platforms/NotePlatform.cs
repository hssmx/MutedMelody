using MutedMelody.Core;
using UnityEngine;

namespace MutedMelody.Platforms
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class NotePlatform : MonoBehaviour
    {
        public int QueueIndex { get; private set; }
        public bool IsActive { get; private set; }

        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        public void Activate(Vector2 position, int queueIndex, Vector2 size)
        {
            transform.position = position;
            QueueIndex = queueIndex;
            IsActive = true;
            
            _collider.size = size;
            gameObject.SetActive(true);

            // TODO (Phase 14): Trigger Activation VFX/SFX
            // Debug.Log($"[NotePlatform] Activated at index {QueueIndex}");
        }

        public void Shatter()
        {
            IsActive = false;
            
            // TODO (Phase 14): Trigger Shatter VFX/SFX/Shake here before disabling
            // Debug.Log($"[NotePlatform] Shattered index {QueueIndex}");
            
            EventBus.Publish(new MutedMelody.Core.Events.PlatformShatteredEvent());
            
            gameObject.SetActive(false);
        }
    }
}
