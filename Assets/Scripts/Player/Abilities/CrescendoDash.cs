using MutedMelody.Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using MutedMelody.Stats;
using MutedMelody.Input; 

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
        [SerializeField] private MelodyStaffData _melodyData;

        private PlayerController _player;
        private MelodyStaff _melodyStaff;
        private Rigidbody2D _rb;
        
        private float _dashTimer;
        private float _cooldownTimer;
        private float _originalGravity;
        
        public bool IsDashing { get; private set; }

        private void Awake()
        {
            if (_melodyData == null)
            {
                Debug.LogError($"[CrescendoDash] MelodyData is MISSING on {gameObject.name}! Disabling ability.");
                this.enabled = false;
                return;
            }

            _player = GetComponent<PlayerController>();
            _melodyStaff = GetComponent<MelodyStaff>();
            _rb = GetComponent<Rigidbody2D>();
            _originalGravity = _rb.gravityScale;
        }

        private void Start() => InputManager.Instance.Gameplay.Dash.performed += OnDashInput;

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
                InputManager.Instance.Gameplay.Dash.performed -= OnDashInput;
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

        public void OnDashInput(InputAction.CallbackContext context)
        {
            if (context.performed) TryDash();
        }

        public void TryDash()
        {
            if (_player.IsMovementLocked || IsDashing || _cooldownTimer > 0) return;
            
            float cost = _melodyData.dashCost;
            if (!_melodyStaff.CanAfford(cost)) return;

            _melodyStaff.TrySpend(cost, MelodyChangeReason.DashSpend);

            IsDashing = true;
            _dashTimer = _dashDuration;
            
            _player.AddMovementLock(); 
            _player.SetInvincible(_dashDuration);

            _rb.gravityScale = 0f;
            _rb.linearVelocity = new Vector2((_player.IsFacingRight? 1 : -1) * _dashSpeed, 0f);
        }

        private void EndDash()
        {
            IsDashing = false;
            _cooldownTimer = _dashCooldown;
            _player.RemoveMovementLock();
            _rb.gravityScale = _originalGravity;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * 0.3f, _rb.linearVelocity.y);
        }
    }
}