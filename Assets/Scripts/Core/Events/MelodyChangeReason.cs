using MutedMelody.Core;

namespace MutedMelody.Core.Events
{
    public enum MelodyChangeReason
    {
        None = 0,
        Initialize = 1,
        PerfectPlatformBeat = 2,
        GoodPlatformBeat = 3,
        EnemyKill = 4,
        Damage = 5,
        DashSpend = 6,
        TuneSpend = 7,
        RespawnReset = 8,
        DebugSet = 9
    }
}
