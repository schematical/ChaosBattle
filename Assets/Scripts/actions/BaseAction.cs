using System;

namespace services.actions
{
    public abstract class BaseAction
    {
        protected NPCEntity actingNPCEntity;

        public BaseAction(NPCEntity npcEntity)
        {
            actingNPCEntity = npcEntity;
        }
        public abstract void tick();
        public abstract Boolean isFinished();
    }
}