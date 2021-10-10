public abstract class BrainBase
{
    private NPCEntity _npcEntity;

    public NPCEntity NpcEntity => _npcEntity;

    public BrainBase(NPCEntity npcEntity)
    {
        _npcEntity = npcEntity;
    }

    public abstract void tick();

}