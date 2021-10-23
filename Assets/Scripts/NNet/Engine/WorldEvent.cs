using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldEvent
{

    public static class WorldEventTypes{
        public const string I_SPAWNED = "I_SPAWNED"; 
        public const string I_DIED = "I_DIED"; 
        public const string TILE_ENTER = "TILE_ENTER";   
        public const string TILE_STAY = "TILE_STAY";
        public const string TILE_EXIT = "TILE_EXIT";
        public const string ENTITY_ENTER = "ENTITY_ENTER";
        public const string ENTITY_EXIT = "ENTITY_EXIT";
        public const string NOISE_ENTER = "NOISE_ENTER";
        public const string HEALTH_CHANGE = "HEALTH_CHANGE";
        public const string COLLECT = "COLLECT";
        public const string PING = "PING";
    }

    protected static List<WorldEvent> worldEvents = new List<WorldEvent>();
    public static void CleanTick()
    {
        
        for (int i = 0; i < worldEvents.Count; i++)
        {
            if(worldEvents[i] != null && !worldEvents[i].keepAlive){
                //UnityEngine.Object.Destroy(worldEvents[i]);
                //System.Object.Destroy(worldEvents[i]);
                worldEvents.RemoveAt(i);
            }

        }
        worldEvents.Clear();
    }

    public static void CleanAll(){
        for (int i = 0; i < worldEvents.Count; i++)
        {
            //UnityEngine.Object.Destroy(worldEvents[i]);
            worldEvents[i] = null;
        }
        worldEvents.Clear();
    }

    public /*override*/ string _class_name
    {
        get
        {
            return "WorldEvent";
        }
    }
    public Collider2D colider2d;
    public Collision2D collision2D;
    public string eventType;
    /*public TileObject tileObject;
    public EntityObject entityObject;
    public EntityObject self;*/
    public float value = 0;
    public bool keepAlive = false;
    public WorldEvent(){
        GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
    }


    ~WorldEvent()
    {
        //System.Threading.Interlocked.Decrement(ref counter);
        GameManager.instance.garbageCollector.DeregisterObject(_class_name);
    }
    /*public virtual void Init(string _eventType, EntityObject _self)
    {
        eventType = _eventType;
        self = _self;

    }*/

	/*
	public override void OnEnable()
	{
        base.OnEnable();
        worldEvents.Add(this);
	}
	public override void OnDestroy()
	{
        base.OnDestroy();
        collision2D = null;
        colider2d = null;
        tileObject = null;
        entityObject = null;
        self = null;
	}
	*/

}
