using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;
using MutedMelody.Core.Pooling;

namespace MutedMelody.Enemies
{
    public class RhythmCannon : BaseEnemy
    {
        [Header("Cannon Settings")]
        public int fireEveryNBeats = 4;
        public float trackingSpeed = 5f;
        
        [Header("References")]
        public Transform barrelPivot;
        public Transform firePoint;
        public SpriteRenderer chargeIndicator; // Lights up right before firing

        private Transform _playerTransform;
        private int _beatCount = 0;

        protected override void Awake()
        {
            base.Awake();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _playerTransform = player.transform;
            
            if (chargeIndicator != null) chargeIndicator.color = Color.clear;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus.Subscribe<BeatEvent>(OnBeat);
            _beatCount = 0;
            if (chargeIndicator != null) chargeIndicator.color = Color.clear;
        }

        protected void OnDisable()
        {
            EventBus.Unsubscribe<BeatEvent>(OnBeat);
        }

        private void Update()
        {
            if (!IsActive || state == EnemyState.Dying || _playerTransform == null) return;

            TrackPlayer();
        }

        private void TrackPlayer()
        {
            // Calculate angle to player
            Vector2 direction = _playerTransform.position - barrelPivot.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Smoothly rotate the barrel toward the player
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            barrelPivot.rotation = Quaternion.Lerp(barrelPivot.rotation, targetRotation, trackingSpeed * Time.deltaTime);
        }

        private void OnBeat(BeatEvent evt)
        {
            if (!IsActive || state == EnemyState.Dying) return;

            _beatCount++;

            // Charge up on the beat right before firing
            if (_beatCount == fireEveryNBeats - 1)
            {
                StartCharge();
            }
            // Fire!
            else if (_beatCount >= fireEveryNBeats)
            {
                _beatCount = 0;
                Fire();
            }
        }

        private void StartCharge()
        {
            if (chargeIndicator != null) chargeIndicator.color = Color.yellow; // Warning visual
            // TODO: Play charge sound effect here
        }

        private void Fire()
        {
            if (chargeIndicator != null) chargeIndicator.color = Color.clear;
            state = EnemyState.Attacking;

            CannonProjectile projectile = PoolManager.Instance.Get<CannonProjectile>("CannonProjectile");
            if (projectile != null)
            {
                projectile.transform.position = firePoint.position;
                // Launch in the exact direction the barrel is pointing
                projectile.Launch(barrelPivot.right);
            }

            state = EnemyState.Idle;
        }
    }
}