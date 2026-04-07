using System.Collections.Generic;
using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;
using MutedMelody.Audio;

namespace MutedMelody.Platforms
{
    public class NotePlatformManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NotePlatformData _platformData;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private ConductorManager _conductorManager;
        [SerializeField] private SFXPoolManager _sfxPoolManager;

        [Header("Audio")]
        [SerializeField] private AudioClip _perfectSfx;
        [SerializeField] private AudioClip _goodSfx;
        [SerializeField] private AudioClip _missSfx;

        private readonly Queue<NotePlatform> _activePlatforms = new();
        private readonly Queue<NotePlatform> _platformPool = new();

        private void Awake()
        {
            if (_platformData == null)
            {
                Debug.LogError("[NotePlatformManager] Missing NotePlatformData.", this);
                enabled = false;
                return;
            }

            WarmPool();
        }

        private void WarmPool()
        {
            int warmCount = Mathf.Max(_platformData.maxActivePlatforms + 2, _platformData.poolSize);

            for (int i = 0; i < warmCount; i++)
            {
                NotePlatform platform = Instantiate(_platformData.platformPrefab, transform);
                platform.gameObject.SetActive(false);
                _platformPool.Enqueue(platform);
            }
        }

        public void SpawnPlatform()
        {
            if (_playerTransform == null)
            {
                Debug.LogWarning("[NotePlatformManager] No player transform assigned.", this);
                return;
            }

            if (_activePlatforms.Count >= _platformData.maxActivePlatforms)
            {
                ShatterOldestPlatform();
            }

            NotePlatform platform = GetPlatformFromPool();
            Vector3 spawnPosition = _playerTransform.position + (Vector3)_platformData.spawnOffset;

            platform.transform.position = spawnPosition;
            platform.Initialize(this);
            platform.gameObject.SetActive(true);

            _activePlatforms.Enqueue(platform);
            JudgeSpawnTiming();
        }

        public void ReturnToPool(NotePlatform platform)
        {
            if (platform == null)
            {
                return;
            }

            platform.gameObject.SetActive(false);
            _platformPool.Enqueue(platform);
        }

        private NotePlatform GetPlatformFromPool()
        {
            if (_platformPool.Count > 0)
            {
                return _platformPool.Dequeue();
            }

            Debug.LogWarning("[NotePlatformManager] Platform pool exhausted. Growing pool.", this);
            NotePlatform platform = Instantiate(_platformData.platformPrefab, transform);
            platform.gameObject.SetActive(false);
            return platform;
        }

        private void ShatterOldestPlatform()
        {
            if (_activePlatforms.Count == 0)
            {
                return;
            }

            NotePlatform oldest = _activePlatforms.Dequeue();

            if (oldest != null && oldest.gameObject.activeSelf)
            {
                oldest.Shatter();
            }
        }

        private void JudgeSpawnTiming()
        {
            if (_conductorManager == null)
            {
                return;
            }

            JudgmentResult result = BeatJudgment.Judge(_conductorManager.GetTimeSinceLastBeat(), out float offsetMs);

            EventBus.Publish(new PlayerPlatformJudgedEvent
            {
                Result = result,
                OffsetMs = offsetMs
            });

            AudioClip clipToPlay = result switch
            {
                JudgmentResult.Perfect => _perfectSfx,
                JudgmentResult.Good => _goodSfx,
                _ => _missSfx
            };

            if (_sfxPoolManager != null && clipToPlay != null)
            {
                _sfxPoolManager.PlaySFX(clipToPlay, transform.position);
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"[Beat Judgment] {result} | {offsetMs:F1}ms");
#endif
        }
    }
}
