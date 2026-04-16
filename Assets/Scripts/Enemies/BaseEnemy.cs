using System.Collections;
using UnityEngine;
using MutedMelody.Core.Pooling;
using MutedMelody.Items;

namespace MutedMelody.Enemies
{
    public enum EnemyState { Idle, Attacking, Dying }

    public abstract class BaseEnemy : MonoBehaviour
    {
        public EnemyData data;
        public bool IsActive { get; protected set; }
        
        [SerializeField] protected float currentHP;
        protected EnemyState state;
        protected SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected virtual void OnEnable()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            if (data != null) currentHP = data.maxHP;
            state = EnemyState.Idle;
            IsActive = false;
        }

        public virtual void SetActive(bool active)
        {
            if (state == EnemyState.Dying) return;
            IsActive = active;
        }

        public virtual void TakeDamage(float amount)
        {
            if (state == EnemyState.Dying || !IsActive) return;

            currentHP -= amount;
            StartCoroutine(DamageFlashRoutine());

            if (currentHP <= 0)
            {
                Die();
            }
        }
        
        // --- TEMPORARY DEBUG BUTTON ---
        [ContextMenu("Debug Kill")]
        public void DebugKill()
        {
            TakeDamage(999f);
        }

        protected virtual void Die()
        {
            state = EnemyState.Dying;
            IsActive = false;

            MelodyPickup pickup = PoolManager.Instance.Get<MelodyPickup>("MelodyPickup");
            if (pickup != null)
            {
                pickup.transform.position = transform.position;
                if (data != null) pickup.SetReward(data.melodyDropAmount);
            }

            // TODO (Phase 14): Spawn Death VFX from PoolManager here
            gameObject.SetActive(false);
        }

        private IEnumerator DamageFlashRoutine()
        {
            if (spriteRenderer != null) spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
        }
    }
}