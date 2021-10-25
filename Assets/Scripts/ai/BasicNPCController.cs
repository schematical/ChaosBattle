
using UnityEngine;
using UnityEngine.Tilemaps;

public class BasicNpcControllerV1 : NPCControllerBase
{
  
    public override void tick()
    {
        BaseAction currAction = ChaosNpcEntity.GetCurrentAction();
        if (
            currAction == null || 
            currAction.isFinished()
        ){
            SelectAnAction();
        }
    }

    private void SelectAnAction()
    {
        if (ChaosNpcEntity.primaryHeldItem)
        {
            // Find someone on the other team and attack
            
            float closesEnemyDist = 99999;
            ChaosNPCEntity closestEnemy = (ChaosNPCEntity)GameManager.instance.level.FindClosestEntity(ChaosNpcEntity.transform.position, (ChaosEntity chaosEntity) =>
            {
                ChaosNPCEntity testChaosNpcEntity = chaosEntity.GetComponent<ChaosNPCEntity>();
                if (!testChaosNpcEntity)
                {
                    return false;
                }
                if (!testChaosNpcEntity.IsAlive())
                {
                    return false;
                }

                /*if (testNPCEntity.Equals(NpcEntity))
                {
                    return false;
                }*/
                if (ChaosNpcEntity.primaryHeldItem is ChaosHealingItem)
                {
                    if (!testChaosNpcEntity.GetTeam().Equals(ChaosNpcEntity.GetTeam()))
                    {
                        return false;
                    }
                    
                    if (testChaosNpcEntity.GetStatVal(ChaosEntityStatType.Health) >=
                        testChaosNpcEntity.GetStatVal(ChaosEntityStatType.MaxHealth))
                    {
                        return false;
                    }
                }
                else
                {
                  

                    if (testChaosNpcEntity.GetTeam().Equals(ChaosNpcEntity.GetTeam()))
                    {
                        return false;
                    }
                    if (
                        ChaosNpcEntity.primaryHeldItem is ChaosShieldItem && 
                        testChaosNpcEntity.IsStunned()
                    ){
                        return false;
                    }
                }

                return true;
            });
           
            if (!closestEnemy)
            {
                Vector3Int targetVec2 = GameManager.instance.level.GetRandomFloorTileVec();
                NavigateToAction wonderAction2 = new NavigateToAction(ChaosNpcEntity);
                wonderAction2.SetTargetVec(targetVec2);
                ChaosNpcEntity.SetCurrentAction(wonderAction2);
                return;
            }
            UsePrimaryItemAction baseAction = new UsePrimaryItemAction(ChaosNpcEntity);
            baseAction.SetTarget(closestEnemy);
            ChaosNpcEntity.SetCurrentAction(baseAction);
            return;
        }
        ChaosItem nearestAvailableItem = (ChaosItem)GameManager.instance.level.FindClosestEntity(ChaosNpcEntity.transform.position, (ChaosEntity chaosEntity) =>
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
            NavigateToAction navToItemAction = new NavigateToAction(ChaosNpcEntity);
            navToItemAction.SetTarget(nearestAvailableItem);
            ChaosNpcEntity.SetCurrentAction(navToItemAction);
            return;
        }
        
        Vector3Int targetVec = GameManager.instance.level.GetRandomFloorTileVec();
        NavigateToAction wonderAction = new NavigateToAction(ChaosNpcEntity);
        wonderAction.SetTargetVec(targetVec);
        ChaosNpcEntity.SetCurrentAction(wonderAction);
        
    }
}