using UnityEngine;
using UnityEngine.UI;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.UI
{
    public class MelodyStaffUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private RectTransform _pulseTarget;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _criticalColor = new Color(1f, 0.2f, 0.6f, 1f);
        [SerializeField] private float _beatPulseScale = 1.04f;
        [SerializeField] private float _recoverSpeed = 8f;
        [SerializeField] private float _criticalPulseSpeed = 6f;
        [SerializeField] private float _criticalPulseAmount = 0.15f;

        private Vector3 _baseScale = Vector3.one;
        private float _targetFill;
        private bool _isCritical;

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
            EventBus.Subscribe<MelodyStaffChangedEvent>(OnMelodyChanged);
            EventBus.Subscribe<BeatEvent>(OnBeat);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<MelodyStaffChangedEvent>(OnMelodyChanged);
            EventBus.Unsubscribe<BeatEvent>(OnBeat);
        }

        private void Update()
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = Mathf.MoveTowards(_fillImage.fillAmount, _targetFill, _recoverSpeed * Time.unscaledDeltaTime);

                if (_isCritical)
                {
                    float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * _criticalPulseSpeed);
                    _fillImage.color = Color.Lerp(_normalColor, _criticalColor, pulse * _criticalPulseAmount + (1f - _criticalPulseAmount));
                }
                else
                {
                    _fillImage.color = _normalColor;
                }
            }

            if (_pulseTarget != null)
            {
                _pulseTarget.localScale = Vector3.Lerp(_pulseTarget.localScale, _baseScale, _recoverSpeed * Time.unscaledDeltaTime);
            }
        }

        private void OnMelodyChanged(MelodyStaffChangedEvent evt)
        {
            _targetFill = evt.NormalizedMelody;
            _isCritical = evt.IsCritical;
        }

        private void OnBeat(BeatEvent evt)
        {
            if (_pulseTarget != null)
            {
                _pulseTarget.localScale = _baseScale * _beatPulseScale;
            }
        }
    }
}
