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
            if (npcEntity.primaryHeldItem)
            {
                return; // They are already holding something.
            }
            npcEntity.SetPrimaryHeldItem(this);
            // GetComponent<Rigidbody2D>().simulated = false;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
   
        
      
        }

        public void SetHoldingEntity(NPCEntity npcEntity)
        {
            heldByNPCEntity = npcEntity;
        }
    }
}