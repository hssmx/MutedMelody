using UnityEngine;
using MutedMelody.Stats;
using MutedMelody.Core.Events;
using MutedMelody.Environment;
using System.Collections;

namespace MutedMelody.Player
{
    [RequireComponent(typeof(PlayerController), typeof(MelodyStaff), typeof(SpriteRenderer))]
    public class PlayerDamageReceiver : MonoBehaviour
    {
        [SerializeField] private float _iFrameDuration = 1.5f;
        [SerializeField] private float _flashInterval = 0.1f;
        
        private PlayerController _player;
        private MelodyStaff _melodyStaff;
        private SpriteRenderer _sprite;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _melodyStaff = GetComponent<MelodyStaff>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        // Use OnTriggerStay2D so if they sit in spikes and iFrames run out, they get hit again
        private void OnTriggerStay2D(Collider2D col)
        {
            if (_player.IsInvincible || _melodyStaff.IsEmpty) return;

            if (col.TryGetComponent(out DamageSource source))
            {
                TakeDamage(source.damageAmount);
            }
        }

        private void TakeDamage(float amount)
        {
            _melodyStaff.ApplyDamage(amount);
            _player.SetInvincible(_iFrameDuration);
            
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            while (_player.IsInvincible)
            {
                _sprite.enabled = !_sprite.enabled;
                yield return new WaitForSeconds(_flashInterval);
            }
            _sprite.enabled = true;
        }
    }
}
