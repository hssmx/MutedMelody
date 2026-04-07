using UnityEngine;

namespace MutedMelody.Stats
{
    [CreateAssetMenu(fileName = "DefaultMelodyStaffData", menuName = "Muted Melody/Stats/Melody Staff Data")]
    public class MelodyStaffData : ScriptableObject
    {
        [Header("Core")]
        [Min(1f)] public float maxMelody = 100f;
        [Min(0f)] public float startingMelody = 50f;
        [Range(0f, 1f)] public float criticalThreshold = 0.25f;

        [Header("Beat Rewards")]
        [Min(0f)] public float perfectBeatReward = 10f;
        [Min(0f)] public float goodBeatReward = 5f;

        [Header("Combat")]
        [Min(0f)] public float enemyKillReward = 15f;
        [Min(0f)] public float damageAmount = 20f;

        [Header("Abilities")]
        [Min(0f)] public float tuneDrain = 15f;
        [Min(0f)] public float tuneHeal = 20f;
        [Min(0f)] public float dashCost = 25f;

        [Header("Respawn")]
        [Range(0f, 1f)] public float respawnPercent = 0.5f;
    }
}
