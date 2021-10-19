
public class ChaosInteraction
{
    private ChaosInteractionType _interactionType;
    private float _amount;
    private NPCEntity _sender;
    private NPCEntity _reciver;

    public ChaosInteraction(ChaosInteractionType interactionType, float amount, NPCEntity sender, NPCEntity reciver)
    {
        _interactionType = interactionType;
        this._amount = amount;
        this._sender = sender;
        this._reciver = reciver;
    }
    public float Amount => _amount;
    public ChaosInteractionType InteractionType => _interactionType;
    public NPCEntity Sender => _sender;
    public NPCEntity Reciver => _reciver;
}

public enum ChaosInteractionType
{
    MeeleAttack,
    Heal,
    Stun
}
