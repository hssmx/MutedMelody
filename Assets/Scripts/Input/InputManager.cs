using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.InputSystem
{
    // P02.06: Create InputManager.cs extending Singleton
    public class InputManager : Singleton<InputManager>
    {
        // P02.06: Instantiate MutedMelodyInput
        private MutedMelodyInput _input;

        // P02.07: Expose Gameplay and UI action accessors
        public MutedMelodyInput.GameplayActions Gameplay => _input.Gameplay;
        public MutedMelodyInput.UIActions UI => _input.UI;

        protected override void Awake()
        {
            base.Awake();
            _input = new MutedMelodyInput();
        }

        private void OnEnable()
        {
            // P02.06: Enable Gameplay map in OnEnable()
            _input.Enable();
            SwitchToGameplay();
            
            // P02.09: Subscribe to GameStateChangedEvent
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            _input.Disable();
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        // P02.08: Implement Action Map Switching
        public void SwitchToGameplay()
        {
            _input.UI.Disable();
            _input.Gameplay.Enable();
        }

        public void SwitchToUI()
        {
            _input.Gameplay.Disable();
            _input.UI.Enable();
        }

        public void DisableAllInput()
        {
            _input.Gameplay.Disable();
            _input.UI.Disable();
        }

        // P02.09: Map states to appropriate input modes
        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            switch (evt.NewState)
            {
                case GameState.Playing:
                case GameState.BossFight:
                    SwitchToGameplay();
                    break;
                case GameState.Paused:
                case GameState.MainMenu:
                    SwitchToUI();
                    break;
                case GameState.Dead:
                case GameState.Loading:
                case GameState.Cutscene:
                case GameState.Respawning:
                    DisableAllInput();
                    break;
            }
        }
    }
}
