using UnityEngine;
using System.Collections.Generic;
using MutedMelody.Core;
using MutedMelody.Core.Events;
using MutedMelody.Audio;
using MutedMelody.Core.Pooling; // New Include!

namespace MutedMelody.Platforms
{
    public class NotePlatformManager : MonoBehaviour
    {
        public const string PoolName = "NotePlatforms"; // ID for the PoolManager

        [SerializeField] private NotePlatform _platformPrefab;
        [SerializeField] private NotePlatformData _data;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private JudgmentConfig _judgmentConfig;

        [Header("Anti-Clipping")]
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private Vector2 _platformSize = new Vector2(2f, 0.5f);

        private Queue<NotePlatform> _activeQueue = new Queue<NotePlatform>();

        private void Start()
        {
            // Register and warm up the pool globally using Start to ensure PoolManager is loaded
            PoolManager.Instance.CreatePool(PoolName, _platformPrefab, _data.maxActivePlatforms, transform);
        }

        public void SpawnPlatform()
        {
            if (ConductorManager.Instance != null && _judgmentConfig != null)
            {
                var (result, offsetMs) = BeatJudgment.Judge(ConductorManager.Instance, _judgmentConfig);
                EventBus.Publish(new PlayerPlatformJudgedEvent { Result = result, OffsetMs = offsetMs });
                Debug.Log($"[Beat Judgment] {result}! Offset: {offsetMs:F1}ms");
            }

            if (_activeQueue.Count >= _data.maxActivePlatforms)
            {
                NotePlatform oldestPlatform = _activeQueue.Dequeue();
                oldestPlatform.Shatter();
                
                // --- Return to global pool instead of just letting it sit inactive ---
                PoolManager.Instance.Return(PoolName, oldestPlatform);
            }

            Vector2 spawnPos = _playerTransform.position + Vector3.down * 0.5f;
            Collider2D hit = Physics2D.OverlapBox(spawnPos, _platformSize, 0f, _obstacleLayer);
            
            if (hit != null)
            {
                float shiftDir = spawnPos.x > hit.bounds.center.x ? 1f : -1f;
                spawnPos.x += shiftDir * (_platformSize.x / 2f);
            }

            // Fetch a fresh platform from the global pool
            NotePlatform platform = PoolManager.Instance.Get<NotePlatform>(PoolName);

            platform.Activate(spawnPos, _activeQueue.Count, _platformSize);
            _activeQueue.Enqueue(platform);
        }
    }
}