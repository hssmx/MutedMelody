using UnityEngine;
using MutedMelody.Core.Pooling;
using MutedMelody.Items;

namespace MutedMelody.Enemies
{
    public class EnemyPoolSetup : MonoBehaviour
    {
        public Shockwave shockwavePrefab;
        public MelodyPickup melodyPickupPrefab;
        public CannonProjectile projectilePrefab;

        private void Start()
        {
            PoolManager.Instance.CreatePool("Shockwave", shockwavePrefab, 10, transform);
            PoolManager.Instance.CreatePool("MelodyPickup", melodyPickupPrefab, 10, transform);
            PoolManager.Instance.CreatePool("CannonProjectile", projectilePrefab, 16, transform);
        }
    }
}