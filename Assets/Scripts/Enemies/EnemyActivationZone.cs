using UnityEngine;

namespace MutedMelody.Enemies
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemyActivationZone : MonoBehaviour
    {
        [SerializeField] private BaseEnemy _parentEnemy;

        private void Awake()
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && _parentEnemy != null)
            {
                _parentEnemy.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player") && _parentEnemy != null)
            {
                _parentEnemy.SetActive(false);
            }
        }
    }
}