public abstract class NPCControllerBase
{
    public string id;
    public bool isSpawned = false;
    protected ChaosNPCEntity _chaosNpcEntity;

    public ChaosNPCEntity ChaosNpcEntity => _chaosNpcEntity;
    /*
  * Sets the entity that this Bot will be controlling
  */
    public void Init(string _id){
        id = _id;
        // memory = new BotMemory();//ScriptableObject.CreateInstance<BotMemory>();

    }
    public void Attach(ChaosNPCEntity entity)
    {
        
        _chaosNpcEntity = entity;
        entity.AttachBotController(this);


    }
    public void DetachEntity(){
        /*foreach (Eye eye in botEyeManager.eyes.Values)
        {
            eye.ResetCache();
            eye.player = null;
            eye.gameObject.SetActive(false);
        }*/
        _chaosNpcEntity.CleanUp();
        _chaosNpcEntity.gameObject.SetActive(false);
        GameManager.instance.level.entities.Remove(_chaosNpcEntity);
        _chaosNpcEntity = null;
        isSpawned = false;
    }

    public bool HasAttachedEntity()
    {
        return (_chaosNpcEntity != null);
      
    }
    public abstract void tick();

}