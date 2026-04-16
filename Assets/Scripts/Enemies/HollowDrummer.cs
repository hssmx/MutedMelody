using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;
using MutedMelody.Core.Pooling;

namespace MutedMelody.Enemies
{
    public class HollowDrummer : BaseEnemy
    {
        [Header("Drummer Settings")]
        public int strikeEveryNBeats = 2;
        private int _beatCount = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus.Subscribe<BeatEvent>(OnBeat);
        }

        protected void OnDisable()
        {
            EventBus.Unsubscribe<BeatEvent>(OnBeat);
        }

        private void OnBeat(BeatEvent evt)
        {
            if (!IsActive || state == EnemyState.Dying) return;

            _beatCount++;
            if (_beatCount >= strikeEveryNBeats)
            {
                _beatCount = 0;
                PerformStrike();
            }
        }

        private void PerformStrike()
        {
            state = EnemyState.Attacking;

            Shockwave wave = PoolManager.Instance.Get<Shockwave>("Shockwave");
            if (wave != null)
            {
                wave.Activate(transform.position);
            }

            // After attacking, return to idle
            state = EnemyState.Idle;
        }
    }
}