using UnityEngine;

namespace MutedMelody.Player
{
    [CreateAssetMenu(fileName = "DefaultPlayerMovementData", menuName = "Muted Melody/Player Movement Data")]
    public class PlayerMovementData : ScriptableObject
    {
        [Header("Run (P03.05)")]
        public float maxSpeed = 12f;
        public float accelerationTime = 0.03f;
        public float decelerationTime = 0.03f;

        [Header("Jump (P03.06)")]
        public float jumpForce = 22f;
        public float gravityScale = 3f;
        public float jumpCutMultiplier = 6f;
        public float fallMultiplier = 4f;
        public float maxFallSpeed = -30f;

        [Header("Assists (P03.07)")]
        public float coyoteTime = 0.12f;
        public float jumpBufferTime = 0.1f;
    }
}
