using UnityEngine;
using Unity.Cinemachine;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.Cameras
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraBeatPulse : MonoBehaviour
    {
        [SerializeField] private float _pulseAmount = 0.05f;
        [SerializeField] private float _returnSpeed = 5f;

        private CinemachineCamera _vcam;
        private float _baseOrthoSize;

        private void Awake()
        {
            _vcam = GetComponent<CinemachineCamera>();
            _baseOrthoSize = _vcam.Lens.OrthographicSize;
        }

        private void OnEnable() => EventBus.Subscribe<BeatEvent>(OnBeat);
        private void OnDisable() => EventBus.Unsubscribe<BeatEvent>(OnBeat);

        private void OnBeat(BeatEvent evt)
        {
            _vcam.Lens.OrthographicSize = _baseOrthoSize - _pulseAmount;
        }

        private void Update()
        {
            if (_vcam.Lens.OrthographicSize < _baseOrthoSize)
            {
                _vcam.Lens.OrthographicSize = Mathf.MoveTowards(
                    _vcam.Lens.OrthographicSize, 
                    _baseOrthoSize, 
                    _returnSpeed * Time.deltaTime
                );
            }
        }
    }
}
