using UnityEngine;
using UnityEngine.UI;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.UI
{
    public class MelodyStaffUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private RectTransform _container;
        
        [Header("Settings")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _criticalColor = Color.magenta;
        [SerializeField] private float _criticalPulseSpeed = 5f;
        
        [Header("Beat Pulse")]
        [SerializeField] private float _beatPulseScale = 1.05f;
        [SerializeField] private float _beatReturnSpeed = 10f;

        private Vector3 _originalScale;
        private bool _isCritical;

        private void Awake()
        {
            if (_container != null) _originalScale = _container.localScale;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<MelodyStaffChangedEvent>(OnStaffChanged);
            EventBus.Subscribe<BeatEvent>(OnBeat);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<MelodyStaffChangedEvent>(OnStaffChanged);
            EventBus.Unsubscribe<BeatEvent>(OnBeat);
        }

        private void Update()
        {
            if (_container == null || _fillImage == null) return;

            // P07.14: Return scale to normal smoothly
            if (_container.localScale.x > _originalScale.x)
            {
                _container.localScale = Vector3.Lerp(_container.localScale, _originalScale, Time.deltaTime * _beatReturnSpeed);
            }

            // P07.13: Pulse color if critical
            if (_isCritical)
            {
                float sine = (Mathf.Sin(Time.time * _criticalPulseSpeed) + 1f) / 2f;
                _fillImage.color = Color.Lerp(_normalColor, _criticalColor, sine);
            }
            else
            {
                _fillImage.color = _normalColor;
            }
        }

        private void OnStaffChanged(MelodyStaffChangedEvent evt)
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = evt.NormalizedMelody;
            }
            _isCritical = evt.IsCritical;
        }

        private void OnBeat(BeatEvent evt)
        {
            if (_container != null)
            {
                _container.localScale = _originalScale * _beatPulseScale;
            }
        }
    }
}
