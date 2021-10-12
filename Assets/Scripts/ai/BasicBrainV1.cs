
using UnityEngine;
using UnityEngine.Tilemaps;

public class BasicBrainV1 : BrainBase
{
    public BasicBrainV1(NPCEntity npcEntity) : base(npcEntity)
    {
    }

    public override void tick()
    {
        BaseAction currAction = NpcEntity.GetCurrentAction();
        if (
            currAction == null || 
            currAction.isFinished()
        ){
            SelectAnAction();
        }
    }

    private void SelectAnAction()
    {
        if (NpcEntity.primaryHeldItem)
        {
            // Find someone on the other team and attack
            
            float closesEnemyDist = 99999;
            NPCEntity closestEnemy = (NPCEntity)GameManager.instance.level.FindClosestEntity(NpcEntity.transform.position, (ChaosEntity chaosEntity) =>
            {
                NPCEntity testNPCEntity = chaosEntity.GetComponent<NPCEntity>();
                if (!testNPCEntity)
                {
                    return false;
                }

                if (testNPCEntity.GetTeam().Equals(NpcEntity.GetTeam()))
                {
                    return false;
                }
                if (!testNPCEntity.isAlive)
                {
                    return false;
                }
                return true;
            });
           
            if (!closestEnemy)
            {
                return;
            }
            UsePrimaryItemAction baseAction = new UsePrimaryItemAction(NpcEntity);
            baseAction.SetTarget(closestEnemy);
            NpcEntity.SetCurrentAction(baseAction);
            return;
        }
        ChoasItem nearestAvailableItem = (ChoasItem)GameManager.instance.level.FindClosestEntity(NpcEntity.transform.position, (ChaosEntity chaosEntity) =>
        {
            ChoasItem testItem = chaosEntity.GetComponent<ChoasItem>();
            if (!testItem)
            {
                return false;
            }
            
            if (testItem.GetHoldingEntity())
            {
                return false;
            }
            return true;
        });
        if (nearestAvailableItem)
        {
            NavigateToAction navToItemAction = new NavigateToAction(NpcEntity);
            navToItemAction.SetTarget(nearestAvailableItem);
            NpcEntity.SetCurrentAction(navToItemAction);
            return;
        }
        
        Vector3Int targetVec = GameManager.instance.level.GetRandomFloorTileVec();
        NavigateToAction wonderAction = new NavigateToAction(NpcEntity);
        wonderAction.SetTargetVec(targetVec);
        NpcEntity.SetCurrentAction(wonderAction);
        
    }
}