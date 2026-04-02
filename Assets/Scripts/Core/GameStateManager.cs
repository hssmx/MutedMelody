using UnityEngine;
using MutedMelody.Core.Events;

namespace MutedMelody.Core
{
    public class GameStateManager : Singleton<GameStateManager>
    {
        // P01.07 Requirement: Add _currentState field
        private GameState _currentState = GameState.MainMenu;

        // P01.07 Requirement: Add CurrentState property
        public GameState CurrentState => _currentState;

        // P01.07 Requirement: Implement ChangeState() with event publishing
        public void ChangeState(GameState newState)
        {
            if (_currentState == newState) return;

            // P01.08 Requirement: Log warning and reject invalid transitions
            if (!IsValidTransition(_currentState, newState))
            {
                Debug.LogWarning($"[GameStateManager] Invalid state transition blocked: {_currentState} -> {newState}.");
                return;
            }

            GameState previousState = _currentState;
            _currentState = newState;

            Debug.Log($"[GameStateManager] State changed: {previousState} -> {newState}");

            EventBus.Publish(new GameStateChangedEvent 
            { 
                PreviousState = previousState, 
                NewState = _currentState 
            });
        }

        // P01.08 Requirement: Create IsValidTransition() method
        private bool IsValidTransition(GameState current, GameState next)
        {
            // P01.08 Requirement: Define all legal transitions in switch
            switch (current)
            {
                case GameState.MainMenu:
                    return next == GameState.Loading;
                
                case GameState.Loading:
                    return next == GameState.Playing || next == GameState.Cutscene || next == GameState.MainMenu;
                
                case GameState.Playing:
                    return next == GameState.Paused || next == GameState.Cutscene || 
                           next == GameState.BossFight || next == GameState.Dead || 
                           next == GameState.MainMenu;
                
                case GameState.Paused:
                    return next == GameState.Playing || next == GameState.MainMenu;
                
                case GameState.Cutscene:
                    return next == GameState.Playing || next == GameState.BossFight || next == GameState.MainMenu;
                
                case GameState.BossFight:
                    return next == GameState.Paused || next == GameState.Dead || 
                           next == GameState.Cutscene || next == GameState.Playing;
                
                case GameState.Dead:
                    return next == GameState.Respawning || next == GameState.MainMenu;
                
                case GameState.Respawning:
                    return next == GameState.Playing || next == GameState.BossFight || next == GameState.MainMenu;
                
                default:
                    return false;
            }
        }
    }
}