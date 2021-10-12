
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
            NPCEntity closestEnemy = null;
            float closesEnemyDist = 99999;
            GameManager.instance.level.entities.ForEach((entity =>
            {
                NPCEntity testNPCEntity = entity.GetComponent<NPCEntity>();
                if (!testNPCEntity)
                {
                    return;
                }

                if (testNPCEntity.GetTeam().Equals(NpcEntity.GetTeam()))
                {
                    return;
                }
                if (!testNPCEntity.isAlive)
                {
                    return;
                }
                float currEnemyDist = (testNPCEntity.transform.position - NpcEntity.transform.position).sqrMagnitude;
                if (
                    !closestEnemy ||
                    currEnemyDist < closesEnemyDist  
                ) {
                    closestEnemy = testNPCEntity;
                    closesEnemyDist = currEnemyDist;
                }
                
            }));
            if (!closestEnemy)
            {
                return;
            }
            UsePrimaryItemAction baseAction = new UsePrimaryItemAction(NpcEntity);
            baseAction.SetTarget(closestEnemy);
  
            NpcEntity.SetCurrentAction(baseAction);
            return;
        }
        
        // By default search for the item or flee
        GameManager.instance.level.entities.ForEach((entity =>
        {
            SwordObject swordObject = entity.GetComponent<SwordObject>();
            if (!swordObject)
            {
                return;
            }

            NPCEntity heldByEntity = swordObject.GetHoldingEntity();
            if (!heldByEntity)
            {
                NavigateToAction baseAction = new NavigateToAction(NpcEntity);
                baseAction.SetTarget(swordObject);
                NpcEntity.SetCurrentAction(baseAction);
                return;
            }
            /*if (heldByEntity.GetTeam().Equals(NpcEntity.GetTeam()))
            {
                return;
            }*/
            // Flee
            
            Vector3Int targetVec = GameManager.instance.level.GetRandomFloorTileVec();
            NavigateToAction wonderAction = new NavigateToAction(NpcEntity);
            wonderAction.SetTargetVec(targetVec);
            NpcEntity.SetCurrentAction(wonderAction);
            return;
        }));
    }
}