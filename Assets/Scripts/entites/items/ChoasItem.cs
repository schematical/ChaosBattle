using UnityEngine;

namespace services
{
 
    public class ChoasItem: ChaosEntity
    {
        private NPCEntity heldByNPCEntity;

        private void OnCollisionEnter2D(Collision2D other)
        {
            NPCEntity npcEntity = other.collider.GetComponent<NPCEntity>();
            if (npcEntity != null)
            {
                OnCollisionEnterNPCEntity(npcEntity);
            }
        
        }

        private void OnCollisionEnterNPCEntity(NPCEntity npcEntity)
        {
            if (IsCurrentlyHeld())
            {
                return; // We are alreadying being held
            }
            if (npcEntity.primaryHeldItem)
            {
                return; // They are already holding something.
            }
            npcEntity.SetPrimaryHeldItem(this);
        }

        public bool IsCurrentlyHeld()
        {
            return !!heldByNPCEntity;
        }

        public void Init()
        {
            heldByNPCEntity = null;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
   
        
      
        }

        public void SetHoldingEntity(NPCEntity npcEntity)
        {
            heldByNPCEntity = npcEntity;
        }

        public NPCEntity GetHoldingEntity()
        {
            return heldByNPCEntity;
        }

        public virtual void ApplyActionAnimation(ActionPhase actionPhase)
        {
            
        }
    }
}