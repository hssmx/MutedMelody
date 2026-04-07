using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.Stats
{
    public class MelodyStaff : MonoBehaviour
    {
        [SerializeField] private MelodyStaffData _data;

        public float CurrentMelody { get; private set; }
        public float MaxMelody => _data != null ? _data.maxMelody : 0f;
        public float NormalizedMelody => MaxMelody <= 0f ? 0f : CurrentMelody / MaxMelody;
        public bool IsCritical => _data != null && NormalizedMelody <= _data.criticalThreshold;
        public bool IsDead { get; private set; }

        private bool _isInitialized;

        private void Awake()
        {
            if (_data == null)
            {
                Debug.LogError("[MelodyStaff] Missing MelodyStaffData reference.", this);
                enabled = false;
                return;
            }

            Initialize();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerPlatformJudgedEvent>(OnPlayerPlatformJudged);

            if (_isInitialized)
            {
                PublishState(CurrentMelody, MelodyChangeReason.Initialize);
            }
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerPlatformJudgedEvent>(OnPlayerPlatformJudged);
        }

        public void Initialize()
        {
            CurrentMelody = Mathf.Clamp(_data.startingMelody, 0f, _data.maxMelody);
            IsDead = CurrentMelody <= 0f;
            _isInitialized = true;
            PublishState(CurrentMelody, MelodyChangeReason.Initialize);
        }

        public bool CanAfford(float cost)
        {
            return !IsDead && cost > 0f && CurrentMelody >= cost;
        }

        public bool TrySpend(float amount, MelodyChangeReason reason)
        {
            if (!_isInitialized || IsDead || amount <= 0f)
            {
                return false;
            }

            if (CurrentMelody < amount)
            {
                return false;
            }

            SetMelodyInternal(CurrentMelody - amount, reason);
            return true;
        }

        public void Reward(float amount, MelodyChangeReason reason)
        {
            if (!_isInitialized || IsDead || amount <= 0f)
            {
                return;
            }

            SetMelodyInternal(CurrentMelody + amount, reason);
        }

        public void ApplyDamage(float amount)
        {
            if (!_isInitialized || IsDead || amount <= 0f)
            {
                return;
            }

            SetMelodyInternal(CurrentMelody - amount, MelodyChangeReason.Damage);
        }

        public void RewardEnemyKill()
        {
            Reward(_data.enemyKillReward, MelodyChangeReason.EnemyKill);
        }

        public void ResetForRespawn()
        {
            if (!_isInitialized)
            {
                return;
            }

            IsDead = false;
            SetMelodyInternal(_data.maxMelody * _data.respawnPercent, MelodyChangeReason.RespawnReset, false);
        }

        public void DebugSetValue(float value)
        {
            if (!_isInitialized)
            {
                return;
            }

            IsDead = false;
            SetMelodyInternal(value, MelodyChangeReason.DebugSet);
        }

        private void OnPlayerPlatformJudged(PlayerPlatformJudgedEvent evt)
        {
            switch (evt.Result)
            {
                case JudgmentResult.Perfect:
                    Reward(_data.perfectBeatReward, MelodyChangeReason.PerfectPlatformBeat);
                    break;

                case JudgmentResult.Good:
                    Reward(_data.goodBeatReward, MelodyChangeReason.GoodPlatformBeat);
                    break;
            }
        }

        private void SetMelodyInternal(float targetValue, MelodyChangeReason reason, bool allowDeathCheck = true)
        {
            float previous = CurrentMelody;
            CurrentMelody = Mathf.Clamp(targetValue, 0f, _data.maxMelody);

            if (Mathf.Approximately(previous, CurrentMelody))
            {
                return;
            }

            if (allowDeathCheck && !IsDead && CurrentMelody <= 0f)
            {
                IsDead = true;
                PublishState(previous, reason);
                EventBus.Publish(new PlayerDeathEvent());
                return;
            }

            PublishState(previous, reason);
        }

        private void PublishState(float previousValue, MelodyChangeReason reason)
        {
            EventBus.Publish(new MelodyStaffChangedEvent
            {
                PreviousMelody = previousValue,
                CurrentMelody = CurrentMelody,
                NormalizedMelody = NormalizedMelody,
                IsCritical = IsCritical,
                IsDead = IsDead,
                Reason = reason
            });
        }
    }
}
