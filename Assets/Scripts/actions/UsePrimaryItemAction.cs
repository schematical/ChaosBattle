
using services;
using services.actions;
using UnityEngine;

public enum ActionPhase
{
    Navigating,
    Windup,
    Acting,
    Cooldown,
    Finished
}
public class UsePrimaryItemAction: BaseAction
{
    private ChaosEntity target;
    private ActionPhase _phase = ActionPhase.Windup;
    private float windupRemainingDuration;
    private float cooldownRemainingDuration;
    public UsePrimaryItemAction(NPCEntity npcEntity) : base(npcEntity)
    {
        windupRemainingDuration = actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Windup);
        cooldownRemainingDuration= actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Windup);
    }

    public void SetTarget(ChaosEntity target)
    {
        this.target = target;
    }

    public override void tick()
    {
        switch (_phase)
        {
            case(ActionPhase.Navigating):
                float dist = (target.transform.position - actingNPCEntity.transform.position).sqrMagnitude;
                if (dist < actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.MeleeRange))
                {
                    TransitionPhase(ActionPhase.Windup);
                }
                break;
            case(ActionPhase.Windup): 
                windupRemainingDuration -= Time.deltaTime;
                if (windupRemainingDuration <= 0)
                {
                    TransitionPhase(ActionPhase.Acting);
                }
                break;
            case(ActionPhase.Cooldown): 
                cooldownRemainingDuration -= Time.deltaTime;
                if (windupRemainingDuration <= 0)
                {
                    TransitionPhase(ActionPhase.Finished);
                }
                break;

        }
     
    }

    private void TransitionPhase(ActionPhase actionPhase)
    {
        _phase = actionPhase;
        if (_phase.Equals(ActionPhase.Acting))
        {
            fire();
        }
    }

    private void fire()
    {
        
    }
    public override bool isFinished()
    {
        return _phase.Equals(ActionPhase.Finished);
    }
}
