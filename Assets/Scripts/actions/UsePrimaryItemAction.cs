
using System;
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
public class UsePrimaryItemAction: NavigateToAction
{

    private ActionPhase _phase = ActionPhase.Navigating;
    private float windupRemainingDuration;
    private float cooldownRemainingDuration;
    public UsePrimaryItemAction(NPCEntity npcEntity) : base(npcEntity)
    {
        windupRemainingDuration = actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Windup);
        cooldownRemainingDuration= actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Windup);
        SetRangeGoal(actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.MeleeRange));
    }


    public override void tick()
    {
        switch (_phase)
        {
            case(ActionPhase.Navigating):
                base.tick();
                break;
            case(ActionPhase.Windup): 
                windupRemainingDuration -= Time.deltaTime;
                actingNPCEntity.primaryHeldItem.ApplyActionAnimation(_phase);
                if (windupRemainingDuration <= 0)
                {
                    TransitionPhase(ActionPhase.Acting);
                }
                break;
            case(ActionPhase.Cooldown): 
                cooldownRemainingDuration -= Time.deltaTime;
                actingNPCEntity.primaryHeldItem.ApplyActionAnimation(_phase);
                if (cooldownRemainingDuration <= 0)
                {
                    TransitionPhase(ActionPhase.Finished);
                }
                break;

        }
     
    }

    public override void EndNavigation()
    {
    
        TransitionPhase(ActionPhase.Windup);
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
        actingNPCEntity.primaryHeldItem.ApplyActionAnimation(_phase);
        TransitionPhase(ActionPhase.Cooldown);
        if (Target is NPCEntity)
        {
            Target.GetComponent<Rigidbody2D>().velocity = (Target.transform.position - actingNPCEntity.transform.position) * 10;
            ((NPCEntity)Target).TakeDamage((int)actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Attack));
        }
     
    }
    public override bool isFinished()
    {
        return _phase.Equals(ActionPhase.Finished);
    }
}
