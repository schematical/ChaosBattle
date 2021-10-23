using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class BotFitnessController: CTBaseObject
{

    public class BotFitnessScoreEventListener: UnityEvent<BotFitnessScoreEvent, NPCNNetController>{}
    public abstract class BotFitnessRule: CTBaseObject
    {
        protected BotFitnessController botFitnessController;
        protected FitnessManagerConfigData.BotFitnessRuleData botFitnessRuleData;
        public virtual void Init(BotFitnessController _botFitnessController, FitnessManagerConfigData.BotFitnessRuleData _botFitnessRuleData)
        {
            botFitnessController = _botFitnessController;
            botFitnessRuleData = _botFitnessRuleData;
        }
        public abstract BotFitnessScoreEvent Test(WorldEvent worldEvent, NPCNNetController npcnNetController);
        public override string _class_name
        {
            get
            {
                return "BotFitnessController_BotFitnessRule";
            }
        }
		//public override void OnDestroy()
        ~BotFitnessRule()
		{
            //base.OnDestroy();
            botFitnessRuleData = null;
            botFitnessController = null;
		}
	}

    public class BotFitnessScoreEvent//:CTBaseObject
    {
        protected float _scoreEffect = 0;
        public int maxSpawnCountEffect = 0;
        public int maxLifeExpectancyEffect = 0;
        public string notes = "";
       
        public /*override*/ string _class_name
        {
            get
            {
                return "BotFitnessController_BotFitnessScoreEvent";
            }
        }

        public BotFitnessScoreEvent()
        {
            GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
        }


        ~BotFitnessScoreEvent()
        {
            //System.Threading.Interlocked.Decrement(ref counter);
            GameManager.instance.garbageCollector.DeregisterObject(_class_name);
        }
        public float scoreEffect
        {
            get
            {
                return _scoreEffect;
            }
            set {
                _scoreEffect = value;
            }
        }

    }

    public override string _class_name
    {
        get
        {
            return "BotFitnessController";
        }
    }
    public List<BotFitnessScoreEvent> events = new List<BotFitnessScoreEvent>();
    public List<BotFitnessRule> rules = new List<BotFitnessRule>();
    public NPCNNetController NpcnNetController;
    public BotFitnessScoreEventListener onScoreEvent = new BotFitnessScoreEventListener();

    public void Init(NPCNNetController npcnNetController){
        NpcnNetController = npcnNetController;
    }


    public float _cumulativeScore = 0;
    public float cumulativeScore
    {
        get
        {
            return _cumulativeScore;
        }
    }
    public void Test(WorldEvent worldEvent)
    {
        switch (worldEvent.eventType){
            case(WorldEvent.WorldEventTypes.I_DIED):

                break;
            default:
                /*if (!worldEvent.self || !worldEvent.self.isAlive())
                {
                  
                    return;
                }*/
                break;
        }
        foreach(BotFitnessRule botFitnessRule in rules){
            BotFitnessScoreEvent botFitnessScoreEvent = botFitnessRule.Test(worldEvent, NpcnNetController);
            if(botFitnessScoreEvent != null){
                _cumulativeScore += botFitnessScoreEvent.scoreEffect;
                NpcnNetController.maxSpawnCount += botFitnessScoreEvent.maxSpawnCountEffect;
                NpcnNetController.maxLifeExpectancy += botFitnessScoreEvent.maxLifeExpectancyEffect;

                //events.Add(botFitnessScoreEvent);

                onScoreEvent.Invoke(botFitnessScoreEvent, NpcnNetController);
            }
        }

    }

    public void SubmitFinalScoreForLife(){
        NpcnNetController.botControllerScoreData.scores.Add(_cumulativeScore);
       
    }

    public float TallyScore(){
        float totalScore = 0;
        foreach (BotFitnessScoreEvent botFitnessScoreEvent in events)
        {
            totalScore += botFitnessScoreEvent.scoreEffect;
        }

        return totalScore;
        
    }
	//public override void OnDestroy()
    ~BotFitnessController()
	{
        //base.OnDestroy();
        for (int i = 0; i < events.Count; i++)
        {
            //UnityEngine.Object.Destroy(events[i]);
            events[i] = null;
        }
        events.Clear();
        events = null;
        /*
        for (int i = 0; i < rules.Count; i++)
        {
            UnityEngine.Object.Destroy(rules[i]);
        }*/
        rules.Clear();
        rules = null;
        NpcnNetController = null;
        onScoreEvent.RemoveAllListeners();
        onScoreEvent = null;
	}
	/*
     *  
	public override void Dispose()
	{
        base.Dispose();
        for (int i = 0; i < events.Count; i++){
            events[i].Dispose();
        }
        for (int i = 0; i < rules.Count; i++)
        {
            rules[i].Dispose();
        }
	}
	*/
}
