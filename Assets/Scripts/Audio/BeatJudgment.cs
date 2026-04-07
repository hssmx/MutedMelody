using UnityEngine;
using MutedMelody.Core.Events;

namespace MutedMelody.Audio
{
    public static class BeatJudgment
    {
        public static float HardwareLatencyMs = 0f; 

        public static (JudgmentResult result, float offsetMs) Judge(ConductorManager conductor, JudgmentConfig config)
        {
            if (conductor == null || config == null) 
                return (JudgmentResult.Miss, 0f);

            float timeSinceLast = conductor.GetTimeSinceLastBeat();
            float timeToNext = conductor.GetTimeToNextBeat();

            float offsetSeconds;
            if (timeSinceLast < timeToNext)
                offsetSeconds = timeSinceLast;
            else
                offsetSeconds = -timeToNext;

            float rawOffsetMs = offsetSeconds * 1000f;
            float calibratedOffsetMs = rawOffsetMs - HardwareLatencyMs;
            
            float absOffsetMs = Mathf.Abs(calibratedOffsetMs);

            JudgmentResult result;
            if (absOffsetMs <= config.perfectWindowMs)
                result = JudgmentResult.Perfect;
            else if (absOffsetMs <= config.goodWindowMs)
                result = JudgmentResult.Good;
            else
                result = JudgmentResult.Miss;

            return (result, calibratedOffsetMs);
        }
    }
}