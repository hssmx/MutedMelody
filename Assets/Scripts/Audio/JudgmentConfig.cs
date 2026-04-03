using UnityEngine;

namespace MutedMelody.Audio
{
    [CreateAssetMenu(fileName = "DefaultJudgmentConfig", menuName = "Muted Melody/Audio/Judgment Config")]
    public class JudgmentConfig : ScriptableObject
    {
        [Tooltip("Half-window in milliseconds (e.g. 30 means +/- 30ms)")]
        public float perfectWindowMs = 30f;
        public float goodWindowMs = 80f;
        
        [Header("Rewards")]
        public int perfectEnergyReward = 10;
        public int goodEnergyReward = 5;
    }
}
