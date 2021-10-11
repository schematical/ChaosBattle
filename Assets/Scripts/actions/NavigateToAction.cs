
using services;
using services.actions;
using UnityEngine;

public class NavigateToAction: BaseAction
{
    private ChaosEntity target;
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

    public void SetRangeGoal(float rangeGoal)
    {
        this.rangeGoal = rangeGoal;
    }
    public override void tick()
    {

            float dist = (target.transform.position - actingNPCEntity.transform.position).sqrMagnitude;
           
            if (dist < rangeGoal)
            {
                EndNavigation();
            }
    }

    public virtual  void EndNavigation()
    {
        hasArrived = true;
    }

    public override bool isFinished()
    {
        return hasArrived;
    }
}
