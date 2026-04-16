using UnityEngine;
using MutedMelody.Core.Pooling;

namespace MutedMelody.Enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class CannonProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 4f;
        
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private float _lifeTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.isKinematic = true; 
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Launch(Vector2 direction)
        {
            _lifeTimer = lifetime;
            
            // CRITICAL FIX: Make sure the sprite is visible again when pulling from the pool
            if (_spriteRenderer != null) _spriteRenderer.enabled = true;
            
            _rb.linearVelocity = direction.normalized * speed;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            gameObject.SetActive(true);
        }

        private void Update()
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy") || (col.isTrigger && !col.CompareTag("Player"))) return;

            // RACE CONDITION FIX: Let Kinetis process the damage before pooling!
            if (col.CompareTag("Player"))
            {
                // Stop the projectile and turn it invisible instantly
                _rb.linearVelocity = Vector2.zero;
                if (_spriteRenderer != null) _spriteRenderer.enabled = false;
                
                // Delay returning to the pool by just enough time for Kinetis to take damage
                Invoke(nameof(ReturnToPool), 0.05f);
            }
            else
            {
                // If we hit a wall/floor, just pool it instantly
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            _rb.linearVelocity = Vector2.zero;
            PoolManager.Instance.Return("CannonProjectile", this);
        }
    }
}