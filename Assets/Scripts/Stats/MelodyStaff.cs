using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.Stats
{
    public class MelodyStaff : MonoBehaviour
    {
        [SerializeField] private MelodyStaffData _data;

        public float CurrentMelody { get; private set; }
        public float NormalizedMelody => CurrentMelody / _data.maxMelody;
        public bool IsCritical => NormalizedMelody <= 0.25f;
        public bool IsDead { get; private set; }

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<JudgmentEvent>(OnBeatJudgment);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<JudgmentEvent>(OnBeatJudgment);
        }

        public void Initialize()
        {
            CurrentMelody = _data.startingMelody;
            IsDead = false;
            PublishState();
        }

        public void AddMelody(float amount)
        {
            if (IsDead) return;
            
            CurrentMelody = Mathf.Clamp(CurrentMelody + amount, 0, _data.maxMelody);
            PublishState();
        }

        public void RemoveMelody(float amount)
        {
            if (IsDead) return;

            CurrentMelody = Mathf.Clamp(CurrentMelody - amount, 0, _data.maxMelody);
            PublishState();

            if (CurrentMelody <= 0)
            {
                IsDead = true;
                EventBus.Publish(new PlayerDeathEvent());
            }
        }

        public bool CanAfford(float cost)
        {
            return CurrentMelody >= cost;
        }

        public void ResetForRespawn()
        {
            CurrentMelody = _data.maxMelody * _data.respawnPercent;
            IsDead = false;
            PublishState();
        }

        private void OnBeatJudgment(JudgmentEvent evt)
        {
            if (evt.Result == MutedMelody.Core.Events.JudgmentResult.Perfect)
            {
                AddMelody(_data.perfectBeatReward);
            }
            else if (evt.Result == MutedMelody.Core.Events.JudgmentResult.Good)
            {
                AddMelody(_data.goodBeatReward);
            }
        }

        private void PublishState()
        {
            EventBus.Publish(new MelodyStaffChangedEvent
            {
                CurrentMelody = CurrentMelody,
                NormalizedMelody = NormalizedMelody,
                IsCritical = IsCritical
            });
        }
    }
}
