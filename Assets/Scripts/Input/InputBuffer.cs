using UnityEngine;
using MutedMelody.Core;

namespace MutedMelody.InputSystem
{
    public class InputBuffer : Singleton<InputBuffer>
    {
        // Configurable windows (0.1s, 0.15s, 0.1s)
        public float jumpBufferWindow = 0.1f;
        public float dashBufferWindow = 0.15f;
        public float platformBufferWindow = 0.1f;

        private float _jumpTime = -1f;
        private float _dashTime = -1f;
        private float _platformTime = -1f;

        // Queue methods
        public void QueueJump() => _jumpTime = Time.time;
        public void QueueDash() => _dashTime = Time.time;
        public void QueuePlatform() => _platformTime = Time.time;

        // Consume methods
        public bool ConsumeJump() { if (Time.time - _jumpTime <= jumpBufferWindow) { _jumpTime = -1f; return true; } return false; }
        public bool ConsumeDash() { if (Time.time - _dashTime <= dashBufferWindow) { _dashTime = -1f; return true; } return false; }
        public bool ConsumePlatform() { if (Time.time - _platformTime <= platformBufferWindow) { _platformTime = -1f; return true; } return false; }

        public void Tick()
        {
            // Optional: Implement frame-based cleanup if strict Time.time validation isn't enough
        }
    }
}
