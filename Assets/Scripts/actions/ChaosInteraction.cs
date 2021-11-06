
public class ChaosInteraction
{
    private ChaosInteractionType _interactionType;
    private float _amount;
    private ChaosNPCEntity _sender;
    private ChaosNPCEntity _reciver;

    public ChaosInteraction(ChaosInteractionType interactionType, float amount, ChaosNPCEntity sender, ChaosNPCEntity reciver)
    {
        _interactionType = interactionType;
        this._amount = amount;
        this._sender = sender;
        this._reciver = reciver;
    }
    public float Amount => _amount;
    public ChaosInteractionType InteractionType => _interactionType;
    public ChaosNPCEntity Sender => _sender;
    public ChaosNPCEntity Reciver => _reciver;
}

public enum ChaosInteractionType
{
    MeeleAttack,
    Heal,
    Stun,
    Purchase
}
