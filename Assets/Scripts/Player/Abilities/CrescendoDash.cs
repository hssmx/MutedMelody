using MutedMelody.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using MutedMelody.Stats;

namespace MutedMelody.Player.Abilities
{
    [RequireComponent(typeof(PlayerController), typeof(MelodyStaff), typeof(Rigidbody2D))]
    public class CrescendoDash : MonoBehaviour
    {
        [Header("Dash Settings")]
        [SerializeField] private float _dashDuration = 0.18f;
        [SerializeField] private float _dashSpeed = 35f;
        [SerializeField] private float _dashCooldown = 0.5f;
        
        [Header("References")]
        [SerializeField] private MelodyStaffData _melodyData; // For dashCost

        private PlayerController _player;
        private MelodyStaff _melodyStaff;
        private Rigidbody2D _rb;
        
        private float _dashTimer;
        private float _cooldownTimer;
        private float _originalGravity;
        
        public bool IsDashing { get; private set; }
        
        private void Start()
        {
            // Make sure the action map name ("Gameplay") and action name ("Dash") match exactly what you typed in the Input Actions window!
            InputManager.Instance.Gameplay.Dash.performed += OnDashInput;
        }

        private void OnDisable()
        {
            InputManager.Instance.Gameplay.Dash.performed -= OnDashInput;
        }
        
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _melodyStaff = GetComponent<MelodyStaff>();
            _rb = GetComponent<Rigidbody2D>();
            _originalGravity = _rb.gravityScale;
        }

        private void Update()
        {
            if (_cooldownTimer > 0) _cooldownTimer -= Time.deltaTime;

            if (IsDashing)
            {
                _dashTimer -= Time.deltaTime;
                if (_dashTimer <= 0) EndDash();
            }
        }

        // Call this from your Input Manager or UnityEvent
        public void OnDashInput(InputAction.CallbackContext context)
        {
            Debug.Log($"Dash Input Heard! Phase: {context.phase}");
            
            if (context.performed) TryDash();
        }

        public void TryDash()
        {
            if (IsDashing || _cooldownTimer > 0) return;
            
            float cost = _melodyData != null ? _melodyData.dashCost : 25f;
            if (!_melodyStaff.CanAfford(cost)) return;

            // Consume resource
            _melodyStaff.RemoveMelody(cost);

            // Start Dash
            IsDashing = true;
            _dashTimer = _dashDuration;
            _player.IsMovementLocked = true; // Stop normal running
            _player.SetInvincible(_dashDuration); // iFrames!

            // Physics override
            _rb.gravityScale = 0f;
            _rb.linearVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * _dashSpeed, 0f);
        }

        private void EndDash()
        {
            IsDashing = false;
            _cooldownTimer = _dashCooldown;
            _player.IsMovementLocked = false;
            
            // Restore physics and bleed momentum
            _rb.gravityScale = _originalGravity;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * 0.3f, _rb.linearVelocity.y);
        }
    }
}
