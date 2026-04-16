using MutedMelody.Core.Events;
using UnityEngine;
using MutedMelody.Stats;
using MutedMelody.Core.Pooling;

namespace MutedMelody.Items
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class MelodyPickup : MonoBehaviour
    {
        private float _rewardAmount = 15f;

        public void SetReward(float amount)
        {
            _rewardAmount = amount;
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && col.TryGetComponent(out MelodyStaff staff))
            {
                staff.Reward(_rewardAmount, MelodyChangeReason.EnemyKill);
                
                // TODO (Phase 14): Spawn Pickup VFX from PoolManager
                PoolManager.Instance.Return("MelodyPickup", this);
            }
        }
    }
}