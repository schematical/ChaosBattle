using System;
using services;
using UnityEngine;

public class UsePrimaryItemActionPhase : ActionPhase
{
    public static ActionPhase Navigating = new ActionPhase("Navigating");
    public static ActionPhase Windup = new ActionPhase("Windup");
    public static ActionPhase Acting = new ActionPhase("Acting");
    public static ActionPhase Cooldown = new ActionPhase("Cooldown");

    public UsePrimaryItemActionPhase(string name) : base(name)
    {
    }
}

public class UsePrimaryItemAction : NavigateToAction
{
    private float windupRemainingDuration;
    private float cooldownRemainingDuration;

    public UsePrimaryItemAction(NPCEntity npcEntity) : base(npcEntity)
    {
        windupRemainingDuration = actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Windup);
        cooldownRemainingDuration = actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Windup);
        SetRangeGoal(actingNPCEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Range));
    }


    public override void tick()
    {
        base.tick();
        if (_phase.Equals(UsePrimaryItemActionPhase.Initializing))
        {
            TransitionPhase(UsePrimaryItemActionPhase.Navigating);
        }

        
        if (_phase.Equals(UsePrimaryItemActionPhase.Windup))
        {
            // Debug.Log("Windup: " + (actingNPCEntity.transform.position - Target.transform.position).sqrMagnitude);
            windupRemainingDuration -= Time.deltaTime;
            actingNPCEntity.primaryHeldItem.ApplyActionAnimation(_phase);
            if (windupRemainingDuration <= 0)
            {
                TransitionPhase(UsePrimaryItemActionPhase.Acting);
            }
        }  else if (_phase.Equals(UsePrimaryItemActionPhase.Cooldown))
        {
            // Debug.Log("Cooldown: " + (actingNPCEntity.transform.position - Target.transform.position).sqrMagnitude);
            cooldownRemainingDuration -= Time.deltaTime;
            actingNPCEntity.primaryHeldItem.ApplyActionAnimation(_phase);
            if (cooldownRemainingDuration <= 0)
            {
                TransitionPhase(ActionPhase.Finished);
            }
        }
    }

    public override void EndNavigation()
    {
        TransitionPhase(UsePrimaryItemActionPhase.Windup);
    }

    protected override void TransitionPhase(ActionPhase actionPhase)
    {
        base.TransitionPhase(actionPhase);
        if (_phase.Equals(UsePrimaryItemActionPhase.Acting))
        {
            fire();
        }
    }

    private void fire()
    {
        actingNPCEntity.primaryHeldItem.ApplyActionAnimation(_phase);
        Debug.Log("UsePrimartItemAction.Use on item: " + actingNPCEntity.primaryHeldItem.name);
        actingNPCEntity.primaryHeldItem.Use(Target);
        TransitionPhase(UsePrimaryItemActionPhase.Cooldown);
       
    }

    public override bool isFinished()
    {
        return _phase.Equals(ActionPhase.Finished);
    }
}