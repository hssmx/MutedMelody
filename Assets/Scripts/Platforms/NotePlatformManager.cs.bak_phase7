using System.Collections.Generic;
using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;
using MutedMelody.Audio;

namespace MutedMelody.Platforms
{
    public class NotePlatformManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private NotePlatformData _data;
        [SerializeField] private NotePlatform _platformPrefab;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private JudgmentConfig _judgmentConfig;

        private Queue<NotePlatform> _activeQueue = new Queue<NotePlatform>();
        private List<NotePlatform> _pool = new List<NotePlatform>();
        
        private int _totalSpawnedCount = 0;

        private void Start()
        {
            WarmupPool();
        }

        private void WarmupPool()
        {
            int poolSize = _data.maxActivePlatforms + 5;
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewPlatformForPool();
            }
        }

        private NotePlatform CreateNewPlatformForPool()
        {
            NotePlatform newPlatform = Instantiate(_platformPrefab); 
            newPlatform.gameObject.SetActive(false);
            _pool.Add(newPlatform);
            return newPlatform;
        }

        private NotePlatform GetFromPool()
        {
            foreach (var platform in _pool)
            {
                if (!platform.IsActive)
                {
                    return platform;
                }
            }

            Debug.LogWarning("[NotePlatformManager] Pool exhausted! Growing on demand. Consider increasing initial pool size.");
            return CreateNewPlatformForPool();
        }

        public void SpawnPlatform()
        {
            if (ConductorManager.Instance != null && _judgmentConfig != null)
            {
                var (result, offsetMs) = BeatJudgment.Judge(ConductorManager.Instance, _judgmentConfig);
                
                // Publish the result so UI/VFX/Score systems can react later
                EventBus.Publish(new JudgmentEvent { Result = result, OffsetMs = offsetMs });
                
                // Temporary debug log so you can see your timing in the console!
                Debug.Log($"[Beat Judgment] {result}! Offset: {offsetMs:F1}ms");
            }
            
            // FIFO Overflow Check
            if (_activeQueue.Count >= _data.maxActivePlatforms)
            {
                NotePlatform oldest = _activeQueue.Dequeue();
                oldest.Shatter();
            }

            // Get and Activate
            NotePlatform nextPlatform = GetFromPool();
            _totalSpawnedCount++;
            
            Vector2 spawnPos = (Vector2)_playerTransform.position + new Vector2(0, _data.spawnOffsetY);
            nextPlatform.Activate(spawnPos, _totalSpawnedCount, _data.platformSize);
            
            _activeQueue.Enqueue(nextPlatform);

            // Broadcast Event
            EventBus.Publish(new NoteSpawnedEvent 
            { 
                Position = spawnPos, 
                ActiveCount = _activeQueue.Count,
                SpawnDspTime = AudioSettings.dspTime 
            });
        }

        public void ClearAllPlatforms()
        {
            while (_activeQueue.Count > 0)
            {
                NotePlatform platform = _activeQueue.Dequeue();
                platform.Shatter();
            }
        }

        // Used to detect if the player is standing on the platform that is about to shatter
        public bool IsPlayerOnPlatform(NotePlatform platform)
        {
            // Implementation requires reference to Player's grounded state or physics check
            // For now, returning false until Player integration is tighter.
            return false; 
        }
    }
}
