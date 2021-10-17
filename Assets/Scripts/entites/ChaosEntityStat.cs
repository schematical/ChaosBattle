
public class ChaosEntityStat
{
    private ChaosEntityStatType type;
    private float val;

    public ChaosEntityStat(ChaosEntityStatType chaosEntityStatType, float val)
    {
        this.type = chaosEntityStatType;
        this.val = val;
    }

    public void SetVal(float val)
    {
        this.val = val;
    }
    public float GetVal()
    {
        return val;
    }
}

public enum ChaosEntityStatType
{
    Health,
    Windup,
    Attack,
    Cooldown,
    Range,
    HealthRecovered,
    MaxHealth,
    StunDuration
}