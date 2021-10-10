
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
        Debug.Log("Ticking UsePrimaryItemAction: " + _phase.ToString());
        switch (_phase)
        {
            case(ActionPhase.Navigating):
                Debug.Log("Navigating");
                base.tick();
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
                if (cooldownRemainingDuration <= 0)
                {
                    TransitionPhase(ActionPhase.Finished);
                }
                break;

        }
     
    }

    public override void EndNavigation()
    {
        Debug.Log("Ending Nav");
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
        Debug.Log("Firing");
        TransitionPhase(ActionPhase.Cooldown);
    }
    public override bool isFinished()
    {
        return _phase.Equals(ActionPhase.Finished);
    }
}
