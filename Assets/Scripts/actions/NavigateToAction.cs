using services;
using UnityEngine;
public class NavigateToActionPhase : ActionPhase
{
    public static ActionPhase Navigating = new ActionPhase("Navigating");


    public NavigateToActionPhase(string name) : base(name)
    {
    }
}
public class NavigateToAction : BaseAction
{
    private ChaosEntity target;
    private Vector3 targetVec;
    private bool hasArrived = false;
    private float rangeGoal = 5;

    public NavigateToAction(NPCEntity npcEntity) : base(npcEntity)
    {
    }

    public ChaosEntity Target => target;

    public void SetTarget(ChaosEntity target)
    {
        this.target = target;
        actingNPCEntity.PathFinder.navigateTo(this.target.gameObject);
    }
    public void SetTargetVec(Vector3 vector3)
    {
        this.targetVec = vector3;
        actingNPCEntity.PathFinder.navigateToVec(this.targetVec);
    }

    public void SetRangeGoal(float rangeGoal)
    {
        this.rangeGoal = rangeGoal;
    }

    public override void tick()
    {
        base.tick();
        if (_phase.Equals(UsePrimaryItemActionPhase.Initializing))
        {
            TransitionPhase(UsePrimaryItemActionPhase.Navigating);
        } else if (_phase.Equals(UsePrimaryItemActionPhase.Navigating))
        {
            if (target)
            {
                targetVec = target.transform.position;
            }

            float dist = (targetVec - actingNPCEntity.transform.position).sqrMagnitude;

            if (dist < rangeGoal)
            {
                EndNavigation();
            }

            if (
                target &&
                target is ChaosItem &&
                ((ChaosItem) target).HeldByNpcEntity
            )
            {
                EndNavigation();
            }
        }
    }

    public virtual void EndNavigation()
    {
       TransitionPhase(ActionPhase.Finished);
    }

   
}