using System.Collections;
using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;
using MutedMelody.Stats;
using MutedMelody.Environment;

namespace MutedMelody.Player
{
    [RequireComponent(typeof(PlayerController), typeof(MelodyStaff))]
    public class PlayerDeathHandler : MonoBehaviour
    {
        private PlayerController _player;
        private MelodyStaff _melodyStaff;
        private bool _isDead;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _melodyStaff = GetComponent<MelodyStaff>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDeathEvent>(OnKillZoneDeath);
            EventBus.Subscribe<MelodyStaffChangedEvent>(OnMelodyChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDeathEvent>(OnKillZoneDeath);
            EventBus.Unsubscribe<MelodyStaffChangedEvent>(OnMelodyChanged);
        }

        private void OnKillZoneDeath(PlayerDeathEvent evt) => TriggerDeath();

        private void OnMelodyChanged(MelodyStaffChangedEvent evt)
        {
            if (evt.IsEmpty && !_isDead) TriggerDeath();
        }

        private void TriggerDeath()
        {
            if (_isDead) return;
            _isDead = true;
            StartCoroutine(DeathSequenceRoutine());
        }

        private IEnumerator DeathSequenceRoutine()
        {
            //GameStateManager.Instance.ChangeState(GameState.Dead);
            _player.AddMovementLock();
            _player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // TODO: Trigger Death VFX / SFX here

            yield return new WaitForSeconds(0.5f);

            //GameStateManager.Instance.ChangeState(GameState.Respawning);

            if (Checkpoint.LastCheckpointPosition != Vector2.zero)
                transform.position = Checkpoint.LastCheckpointPosition;

            _melodyStaff.Respawn();

            _player.SetInvincible(2f);

            _player.RemoveMovementLock();
            _isDead = false;
            //GameStateManager.Instance.ChangeState(GameState.Playing);
        }
    }
}
