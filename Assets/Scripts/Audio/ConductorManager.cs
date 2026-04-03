using UnityEngine;
using MutedMelody.Core;
using MutedMelody.Core.Events;

namespace MutedMelody.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class ConductorManager : Singleton<ConductorManager>
    {
        [SerializeField] private RoomTempoData _currentTempo;
        
        private AudioSource _audioSource;
        
        // P05.06: Public Read Properties
        public float SecondsPerBeat { get; private set; }
        public float SongPositionInBeats { get; private set; }
        public int CurrentBeat { get; private set; }
        public int CurrentMeasure { get; private set; }
        public float BPM => _currentTempo != null ? _currentTempo.bpm : 120f;
        public double DspTime => AudioSettings.dspTime;

        private double _dspSongStartTime;
        private float _songPositionInSeconds;
        private double _pauseDspTime;
        private bool _isPaused;

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void Start()
        {
            if (_currentTempo != null)
            {
                StartMusic();
            }
        }

        // P05.04: Implement StartMusic
        public void StartMusic()
        {
            SecondsPerBeat = 60f / _currentTempo.bpm;
            _dspSongStartTime = AudioSettings.dspTime + 0.5f; // Slight buffer to prevent stutter
            
            _audioSource.PlayScheduled(_dspSongStartTime);
            
            CurrentBeat = 0;
            CurrentMeasure = 0;
            _isPaused = false;
        }

        // P05.09: Implement SetTempo
        public void SetTempo(RoomTempoData newTempo)
        {
            _currentTempo = newTempo;
            SecondsPerBeat = 60f / _currentTempo.bpm;
        }

        private void Update()
        {
            if (_isPaused || !_audioSource.isPlaying) return;

            // P05.05: Implement Beat Tracking Loop
            _songPositionInSeconds = (float)(AudioSettings.dspTime - _dspSongStartTime);
            SongPositionInBeats = _songPositionInSeconds / SecondsPerBeat;

            // Check if we crossed a new beat threshold
            if (SongPositionInBeats >= CurrentBeat)
            {
                EventBus.Publish(new BeatEvent { BeatNumber = CurrentBeat, DspTime = AudioSettings.dspTime });
                CurrentBeat++;

                // Check for new measure
                if (CurrentBeat % _currentTempo.beatsPerMeasure == 0)
                {
                    CurrentMeasure++;
                    EventBus.Publish(new MeasureEvent { MeasureNumber = CurrentMeasure });
                }
            }
        }

        // P05.07: Implement GetTimeSinceLastBeat
        public float GetTimeSinceLastBeat()
        {
            float fractionalBeat = SongPositionInBeats - (CurrentBeat - 1);
            return fractionalBeat * SecondsPerBeat;
        }

        // P05.08: Implement GetTimeToNextBeat
        public float GetTimeToNextBeat()
        {
            float fractionalBeatToNext = CurrentBeat - SongPositionInBeats;
            return fractionalBeatToNext * SecondsPerBeat;
        }

        // P05.10: Implement Pause Support
        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            if (evt.NewState == GameState.Paused)
            {
                _isPaused = true;
                _audioSource.Pause();
                _pauseDspTime = AudioSettings.dspTime;
            }
            else if (evt.PreviousState == GameState.Paused && evt.NewState == GameState.Playing)
            {
                _isPaused = false;
                _audioSource.UnPause();
                // Shift the start time forward by the exact amount of time we spent paused
                _dspSongStartTime += (AudioSettings.dspTime - _pauseDspTime);
            }
        }
    }
}
