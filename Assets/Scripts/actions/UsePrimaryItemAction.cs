using System;
using services;
using UnityEngine;

public class UsePrimaryItemActionPhase : NavigateToActionPhase
{
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

    public UsePrimaryItemAction(ChaosNPCEntity chaosNpcEntity) : base(chaosNpcEntity)
    {
        windupRemainingDuration = ActingChaosNpcEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Windup);
        cooldownRemainingDuration = ActingChaosNpcEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Cooldown);
        SetRangeGoal(ActingChaosNpcEntity.primaryHeldItem.GetStatVal(ChaosEntityStatType.Range));
    }


    public override void tick()
    {
        base.tick();

        if (ActingChaosNpcEntity.primaryHeldItem == null)
        {
            TransitionPhase(ActionPhase.Finished);
            return;
        }
        
        if (_phase.Equals(UsePrimaryItemActionPhase.Windup))
        {
            // Debug.Log("Windup: " + (actingNPCEntity.transform.position - Target.transform.position).sqrMagnitude);
            windupRemainingDuration -= Time.deltaTime;
            ActingChaosNpcEntity.primaryHeldItem.ApplyActionAnimation(_phase);
            if (windupRemainingDuration <= 0)
            {
                TransitionPhase(UsePrimaryItemActionPhase.Acting);
            }
        }  else if (_phase.Equals(UsePrimaryItemActionPhase.Cooldown))
        {
            // Debug.Log("Cooldown: " + (actingNPCEntity.transform.position - Target.transform.position).sqrMagnitude);
            cooldownRemainingDuration -= Time.deltaTime;
            ActingChaosNpcEntity.primaryHeldItem.ApplyActionAnimation(_phase);
            if (cooldownRemainingDuration <= 0)
            {
                TransitionPhase(ActionPhase.Finished);
            }
        }
    }

    public override void EndNavigation()
    {
        if (_phase.Equals(UsePrimaryItemActionPhase.Navigating))
        {
            TransitionPhase(UsePrimaryItemActionPhase.Windup);
        }
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
        ActingChaosNpcEntity.primaryHeldItem.ApplyActionAnimation(_phase);
        ActingChaosNpcEntity.primaryHeldItem.Use(Target);
        TransitionPhase(UsePrimaryItemActionPhase.Cooldown);
       
    }


    public override string GetDebugString()
    {
        string className = GetType().Name;
        return className + "(Phase: " + _phase.Name + "," + "Item: " + ActingChaosNpcEntity.primaryHeldItem.GetType().FullName + ")";
    }
}