using UnityEngine;
using UnityEngine.InputSystem;
using MutedMelody.Stats;
using MutedMelody.Input;

namespace MutedMelody.Player.Abilities
{
    [RequireComponent(typeof(PlayerController), typeof(MelodyStaff))]
    public class TuneHeal : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MelodyStaffData _melodyData;
        
        private PlayerController _player;
        private MelodyStaff _melodyStaff;
        
        public bool IsTuning { get; private set; }
        private bool _isHoldingInput;
        
        private void OnEnable()
        {
            InputManager.Instance.Gameplay.Tune.started += OnTuneInput;
            InputManager.Instance.Gameplay.Tune.canceled += OnTuneInput;
        }

        private void OnDisable()
        {
            InputManager.Instance.Gameplay.Tune.started -= OnTuneInput;
            InputManager.Instance.Gameplay.Tune.canceled -= OnTuneInput;
        }

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _melodyStaff = GetComponent<MelodyStaff>();
        }

        // Call this from your Input Manager or UnityEvent
        public void OnTuneInput(InputAction.CallbackContext context)
        {
            if (context.started) _isHoldingInput = true;
            if (context.canceled)
            {
                _isHoldingInput = false;
                if (IsTuning) StopTuning();
            }
        }

        private void Update()
        {
            float drainRate = _melodyData != null ? _melodyData.tuneDrain : 15f;
            
            if (_isHoldingInput && _player.State.IsGrounded && _melodyStaff.CanAfford(drainRate * Time.deltaTime))
            {
                if (!IsTuning) StartTuning();
                
                // Drain Melody over time
                _melodyStaff.RemoveMelody(drainRate * Time.deltaTime);
                
                // TODO (Phase 9): Publish PlayerHealEvent based on _melodyData.tuneHeal
            }
            else if (IsTuning)
            {
                // Force stop if they run out of melody, let go, or fall off a ledge
                StopTuning();
            }
        }

        private void StartTuning()
        {
            IsTuning = true;
            _player.IsMovementLocked = true; // Freeze player in place
            _player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        private void StopTuning()
        {
            IsTuning = false;
            _player.IsMovementLocked = false;
        }
    }
}
