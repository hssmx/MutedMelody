using UnityEngine;
using UnityEngine.UI;
using MutedMelody.Core;
using MutedMelody.Core.Events;
using MutedMelody.Stats;

namespace MutedMelody.UI
{
    public class MelodyStaffUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MelodyStaff _melodyStaff;
        [SerializeField] private Image _fillImage;
        [SerializeField] private RectTransform _pulseTarget;

        [Header("Visuals")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _criticalColor = Color.magenta;
        [SerializeField] private float _criticalPulseSpeed = 5f;
        [SerializeField] private float _beatPulseScale = 1.05f;
        [SerializeField] private float _beatReturnSpeed = 10f;

        private Vector3 _baseScale = Vector3.one;

        private void Awake()
        {
            if (_pulseTarget == null)
            {
                _pulseTarget = transform as RectTransform;
            }

            if (_pulseTarget != null)
            {
                _baseScale = _pulseTarget.localScale;
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<MelodyStaffChangedEvent>(OnMelodyStaffChanged);
            EventBus.Subscribe<BeatEvent>(OnBeat);

            if (_melodyStaff != null)
            {
                ApplyVisuals(_melodyStaff.CurrentMelody, _melodyStaff.NormalizedMelody, _melodyStaff.IsCritical);
            }
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<MelodyStaffChangedEvent>(OnMelodyStaffChanged);
            EventBus.Unsubscribe<BeatEvent>(OnBeat);
        }

        private void Update()
        {
            if (_pulseTarget != null)
            {
                _pulseTarget.localScale = Vector3.Lerp(_pulseTarget.localScale, _baseScale, _beatReturnSpeed * Time.unscaledDeltaTime);
            }

            if (_melodyStaff != null && _melodyStaff.IsCritical && _fillImage != null)
            {
                float t = 0.5f + (Mathf.Sin(Time.unscaledTime * _criticalPulseSpeed) * 0.5f);
                _fillImage.color = Color.Lerp(_normalColor, _criticalColor, t);
            }
        }

        private void OnMelodyStaffChanged(MelodyStaffChangedEvent evt)
        {
            ApplyVisuals(evt.CurrentMelody, evt.NormalizedMelody, evt.IsCritical);
        }

        private void OnBeat(BeatEvent evt)
        {
            if (_pulseTarget != null)
            {
                _pulseTarget.localScale = _baseScale * _beatPulseScale;
            }
        }

        private void ApplyVisuals(float currentMelody, float normalizedMelody, bool isCritical)
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = Mathf.Clamp01(normalizedMelody);
                _fillImage.color = isCritical ? _criticalColor : _normalColor;
            }
        }
    }
}
