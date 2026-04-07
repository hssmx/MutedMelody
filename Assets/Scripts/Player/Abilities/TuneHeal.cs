using MutedMelody.Core.Events;
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

        private void Awake()
        {
            if (_melodyData == null)
            {
                Debug.LogError($"[TuneHeal] MelodyData is MISSING on {gameObject.name}! Disabling ability.");
                this.enabled = false;
                return;
            }

            _player = GetComponent<PlayerController>();
            _melodyStaff = GetComponent<MelodyStaff>();
        }

        private void Start()
        {
            InputManager.Instance.Gameplay.Tune.started += OnTuneInput;
            InputManager.Instance.Gameplay.Tune.canceled += OnTuneInput;
        }

        private void OnDestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.Gameplay.Tune.started -= OnTuneInput;
                InputManager.Instance.Gameplay.Tune.canceled -= OnTuneInput;
            }
        }

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
            float drainRate = _melodyData.tuneDrainPerSecond;
            
            if (_isHoldingInput && _player.IsGrounded)
            {
                if (!IsTuning)
                {
                    if (_player.IsMovementLocked) return; 
                    
                    if (_melodyStaff.CanAfford(drainRate * Time.deltaTime))
                    {
                        StartTuning();
                    }
                }
                
                if (IsTuning)
                {
                    float costThisFrame = drainRate * Time.deltaTime;
                    
                    if (!_melodyStaff.TrySpend(costThisFrame, MelodyChangeReason.TuneSpend))
                    {
                        if (_melodyStaff.CurrentMelody > 0)
                        {
                            _melodyStaff.TrySpend(_melodyStaff.CurrentMelody, MelodyChangeReason.TuneSpend);
                        }
                        
                        StopTuning(); 
                    }
                }
            }
            else if (IsTuning)
            {
                StopTuning();
            }
        }

        private void StartTuning()
        {
            IsTuning = true;
            _player.AddMovementLock(); 
            _player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        private void StopTuning()
        {
            IsTuning = false;
            _player.RemoveMovementLock();
        }
    }
}