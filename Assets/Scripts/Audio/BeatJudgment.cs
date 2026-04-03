using UnityEngine;
using MutedMelody.Core.Events;

namespace MutedMelody.Audio
{
    public static class BeatJudgment
    {
        public static (JudgmentResult result, float offsetMs) Judge(ConductorManager conductor, JudgmentConfig config)
        {
            if (conductor == null || config == null) 
                return (JudgmentResult.Miss, 0f);

            // Time since the last beat passed
            float timeSinceLast = conductor.GetTimeSinceLastBeat();
            // Time until the next beat arrives
            float timeToNext = conductor.GetTimeToNextBeat();

            // Find the closest beat (were they late to the last one, or early to the next one?)
            float offsetSeconds;
            if (timeSinceLast < timeToNext)
            {
                offsetSeconds = timeSinceLast; // Late tap (positive offset)
            }
            else
            {
                offsetSeconds = -timeToNext; // Early tap (negative offset)
            }

            float offsetMs = offsetSeconds * 1000f;
            float absOffsetMs = Mathf.Abs(offsetMs);

            JudgmentResult result;
            if (absOffsetMs <= config.perfectWindowMs)
            {
                result = JudgmentResult.Perfect;
            }
            else if (absOffsetMs <= config.goodWindowMs)
            {
                result = JudgmentResult.Good;
            }
            else
            {
                result = JudgmentResult.Miss;
            }

            return (result, offsetMs);
        }
    }
}
