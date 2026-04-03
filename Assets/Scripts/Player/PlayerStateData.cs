using UnityEngine;

namespace MutedMelody.Player
{
    [System.Serializable]
    public class PlayerStateData
    {
        public bool IsGrounded;
        public bool IsJumping;
        public bool IsDashing;
        public bool IsTuning;
        public bool IsAlive = true;
        public Vector2 CurrentMoveInput;
        public Vector2 Velocity;
    }
}