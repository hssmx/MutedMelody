#!/bin/bash

# Define Target Directories
INPUT_DIR="Assets/Input"
SCRIPTS_DIR="Assets/Scripts/Input"

echo "🚀 Starting Phase 2: Input System Automation..."

# Create directories
mkdir -p "$INPUT_DIR"
mkdir -p "$SCRIPTS_DIR"
echo "📁 Created directories: $INPUT_DIR and $SCRIPTS_DIR"

# ==========================================
# P02.01: Create Input Actions Asset (Stub)
# ==========================================
# Note: Unity uses a complex JSON structure for .inputactions. 
# We create an empty file so Unity recognizes it, but you must configure it in the Editor.
touch "$INPUT_DIR/MutedMelodyInput.inputactions"
echo "📄 Created MutedMelodyInput.inputactions placeholder."

# ==========================================
# P02.06 - P02.09: Create InputManager.cs
# ==========================================
cat << 'EOF' > "$SCRIPTS_DIR/InputManager.cs"
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
EOF
echo "📝 Generated InputManager.cs"

# ==========================================
# P02.11: Create InputBuffer Utility
# ==========================================
cat << 'EOF' > "$SCRIPTS_DIR/InputBuffer.cs"
using UnityEngine;
using MutedMelody.Core;

namespace MutedMelody.InputSystem
{
    public class InputBuffer : Singleton<InputBuffer>
    {
        // Configurable windows (0.1s, 0.15s, 0.1s)
        public float jumpBufferWindow = 0.1f;
        public float dashBufferWindow = 0.15f;
        public float platformBufferWindow = 0.1f;

        private float _jumpTime = -1f;
        private float _dashTime = -1f;
        private float _platformTime = -1f;

        // Queue methods
        public void QueueJump() => _jumpTime = Time.time;
        public void QueueDash() => _dashTime = Time.time;
        public void QueuePlatform() => _platformTime = Time.time;

        // Consume methods
        public bool ConsumeJump() { if (Time.time - _jumpTime <= jumpBufferWindow) { _jumpTime = -1f; return true; } return false; }
        public bool ConsumeDash() { if (Time.time - _dashTime <= dashBufferWindow) { _dashTime = -1f; return true; } return false; }
        public bool ConsumePlatform() { if (Time.time - _platformTime <= platformBufferWindow) { _platformTime = -1f; return true; } return false; }

        public void Tick()
        {
            // Optional: Implement frame-based cleanup if strict Time.time validation isn't enough
        }
    }
}
EOF
echo "📝 Generated InputBuffer.cs"

# ==========================================
# P02.13: Git Commit
# ==========================================
echo "📦 Staging files for Git..."
git add "$INPUT_DIR" "$SCRIPTS_DIR"

# We use a soft commit here. If it fails, the script warns you.
git commit -m "Phase 2 — Input System scaffolded" || echo "⚠️ Git commit failed. See possible errors below."

echo "✅ Phase 2 Automation Complete!"
