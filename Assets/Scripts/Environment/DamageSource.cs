using UnityEngine;

namespace MutedMelody.Environment
{
    public class DamageSource : MonoBehaviour
    {
        [Tooltip("How much Melody is drained when touching this hazard.")]
        public float damageAmount = 20f;
    }
}
