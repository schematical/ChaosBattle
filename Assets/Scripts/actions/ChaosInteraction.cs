
public class ChaosInteraction
{
    private ChaosInteractionType _interactionType;
    private float _amount;
    private ChaosEntity _sender;
    private ChaosEntity _reciver;

    public ChaosInteraction(ChaosInteractionType interactionType, float amount, ChaosEntity sender, ChaosEntity reciver)
    {
        _interactionType = interactionType;
        this._amount = amount;
        this._sender = sender;
        this._reciver = reciver;
    }
    public float Amount { get; }
}

public enum ChaosInteractionType
{
    MeeleAttack,
    Heal,
    Stun
}
