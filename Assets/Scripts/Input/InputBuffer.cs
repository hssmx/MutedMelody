using UnityEngine;
using MutedMelody.Core;

namespace MutedMelody.Input
{
    public class InputBuffer : Singleton<InputBuffer>
    {
        public float jumpBufferWindow = 0.1f;
        public float dashBufferWindow = 0.15f;
        public float platformBufferWindow = 0.1f;

        private float _jumpTime = -1f;
        private float _dashTime = -1f;
        private float _platformTime = -1f;

        public void QueueJump() => _jumpTime = Time.unscaledTime;
        public void QueueDash() => _dashTime = Time.unscaledTime;
        public void QueuePlatform() => _platformTime = Time.unscaledTime;

        public bool ConsumeJump() { if (Time.unscaledTime - _jumpTime <= jumpBufferWindow) { _jumpTime = -1f; return true; } return false; }
        public bool ConsumeDash() { if (Time.unscaledTime - _dashTime <= dashBufferWindow) { _dashTime = -1f; return true; } return false; }
        public bool ConsumePlatform() { if (Time.unscaledTime - _platformTime <= platformBufferWindow) { _platformTime = -1f; return true; } return false; }

        public void Tick() { }
    }
}