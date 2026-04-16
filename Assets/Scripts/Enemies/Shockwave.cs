using UnityEngine;
using MutedMelody.Core.Pooling;
using MutedMelody.Environment;

namespace MutedMelody.Enemies
{
    [RequireComponent(typeof(CircleCollider2D), typeof(DamageSource))]
    public class Shockwave : MonoBehaviour
    {
        public float expandSpeed = 8f;
        public float maxRadius = 12f;

        private CircleCollider2D _collider;
        private Transform _visualTransform;

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            _collider.isTrigger = true;
            
            // Assume the sprite is a child object to scale visually
            if (transform.childCount > 0)
            {
                _visualTransform = transform.GetChild(0);
            }
        }

        public void Activate(Vector2 startPosition)
        {
            transform.position = startPosition;
            _collider.radius = 0.1f;
            if (_visualTransform != null) _visualTransform.localScale = Vector3.zero;
            
            gameObject.SetActive(true);
        }

        private void Update()
        {
            _collider.radius += expandSpeed * Time.deltaTime;
            
            if (_visualTransform != null)
            {
                _visualTransform.localScale = Vector3.one * (_collider.radius * 2f);
            }

            if (_collider.radius >= maxRadius)
            {
                PoolManager.Instance.Return("Shockwave", this);
            }
        }
    }
}