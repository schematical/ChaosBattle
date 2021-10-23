
public abstract class ChaosScoreCounterBase
{
    private ChaosLevel _chaosLevel;
    public ChaosScoreCounterBase(ChaosLevel chaosLevel)
    {
        _chaosLevel = chaosLevel;
    }

    public abstract int GetEntityScore(ChaosNPCEntity chaosNpcEntity);
}
