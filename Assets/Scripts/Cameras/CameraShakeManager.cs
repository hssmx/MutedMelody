using UnityEngine;
using Unity.Cinemachine;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.Cameras
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraShakeManager : MonoBehaviour
    {
        private CinemachineImpulseSource _impulseSource;

        private void Awake()
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        private void OnEnable() => EventBus.Subscribe<PlatformShatteredEvent>(OnPlatformShattered);
        private void OnDisable() => EventBus.Unsubscribe<PlatformShatteredEvent>(OnPlatformShattered);

        private void OnPlatformShattered(PlatformShatteredEvent evt)
        {
            _impulseSource.GenerateImpulse();
        }
    }
}
