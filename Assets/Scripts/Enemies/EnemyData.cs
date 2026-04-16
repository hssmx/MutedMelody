using UnityEngine;

namespace MutedMelody.Enemies
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Muted Melody/Enemies/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public float maxHP = 30f;
        public float contactDamage = 20f;
        public float melodyDropAmount = 15f;
    }
}