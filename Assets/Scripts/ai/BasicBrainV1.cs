
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
                if (!testNPCEntity.isAlive)
                {
                    return false;
                }

                /*if (testNPCEntity.Equals(NpcEntity))
                {
                    return false;
                }*/
                if (NpcEntity.primaryHeldItem is ChaosHealingItem)
                {
                    if (!testNPCEntity.GetTeam().Equals(NpcEntity.GetTeam()))
                    {
                        return false;
                    }
                    
                    if (testNPCEntity.GetStatVal(ChaosEntityStatType.Health) >=
                        testNPCEntity.GetStatVal(ChaosEntityStatType.MaxHealth))
                    {
                        return false;
                    }
                }
                else
                {
                  

                    if (testNPCEntity.GetTeam().Equals(NpcEntity.GetTeam()))
                    {
                        return false;
                    }
                    if (
                        NpcEntity.primaryHeldItem is ChaosShieldItem && 
                        testNPCEntity.IsStunned()
                    ){
                        return false;
                    }
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
        ChaosItem nearestAvailableItem = (ChaosItem)GameManager.instance.level.FindClosestEntity(NpcEntity.transform.position, (ChaosEntity chaosEntity) =>
        {
            ChaosItem testItem = chaosEntity.GetComponent<ChaosItem>();
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