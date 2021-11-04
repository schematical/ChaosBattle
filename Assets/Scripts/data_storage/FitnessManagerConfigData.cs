using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;    
[System.Serializable]
public class FitnessManagerConfigData{

    [System.Serializable]
    public class BotFitnessRuleAction{
        public const string COLLECT = "COLLECT";
        public const string TILE_ENTER = "TILE_ENTER";
        public const string HAS_NOT_MOVED = "HAS_NOT_MOVED";
        public const string HEALTH_CHANGE = "HEALTH_CHANGE";
    }

    public class GlobalSpeciesSortMode{
        public const string MIN = "MIN";
        public const string MAX = "MAX";
        public const string MEAN = "MEAN";
        public const string MEDIAN = "MEDIAN";
    }

    [System.Serializable]
    public class BotFitnessRuleData
    {
        public string eventType;
        //[SerializeField]
        public EntityFilterData entityFilterData;
        [SerializeField]
        public TileFilterData tileFilterData;
        public string name;
        public string tileBonusMode = null;
        public float scoreEffect = 0;
        public int maxLifeExpectancyEffect = 0;
        public int maxSpawnCountEffect = 0;
        //Fancy stuff
        public float yMultipier = -1;
        public override string ToString(){
            string data = eventType;
            if(tileFilterData != null){
                data += " - " + tileFilterData.ToString();
            }
            if(entityFilterData != null){
                data += " - " + entityFilterData.ToString();
            }
            if(tileBonusMode != null){
                data += " - " + tileBonusMode;
            }
            return data;
        }
        public BotFitnessRuleData()
        {
            if (GameManager.instance != null && GameManager.instance.garbageCollector != null)
            {
                GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
            }
        }

        ~BotFitnessRuleData()
        {

            GameManager.instance.garbageCollector.DeregisterObject(_class_name);
        }

        public string _class_name
        {
            get
            {
                return "FitnessManagerConfigData_BotFitnessRuleData";
            }
        }
    }



    public List<BotFitnessRuleData> botFitnessRules = new List<BotFitnessRuleData>();
    //Fittness
    public bool forceNewFitnessSortingBlockEachGeneration = false;


    public string globalSpeciesSortMode = GlobalSpeciesSortMode.MEDIAN;

    public int initialMaxLifeExpectancy = 5;
    public int bonusSpawnCountPerGeneration = 200;
    public int maxTopFittestSorageCountPerGeneration = 2;// 10;
    public int minSpeciesSpawnAllowence = 5; // 50;

    public int speciesMaxGenerationsSinceLastImprovment = 25;
    public int speciesMaxTurnsToOptimizeBeforeSubjectToExtinction = 5;

    public int minSpeciesCount = 5;// 50;
    public int maxTopFittestSpeciesSorageCountPerGeneration = 25;

    public int medianScoreRoundTo = 100;


    public bool randomizeSpawnPosition = false;
    public bool spawnSpeciesWeightByScore = true;

  
    void ToJSON(){
        
    }

    public void InitDefault(){
        botFitnessRules = new List<BotFitnessRuleData>{
            new BotFitnessRuleData{
                eventType = BotFitnessRuleAction.TILE_ENTER,
                yMultipier = 1.25f,
                tileFilterData =  new TileFilterData{
                    //uniqueObjects = true,
                    forceClimb = true,
                    tileTypes = new List<string>{
                        "Floor1"
                    }
                },
                scoreEffect = 10

            },
            new BotFitnessRuleData{
                eventType = BotFitnessRuleAction.TILE_ENTER,
                tileFilterData =  new TileFilterData{
                    //uniqueObjects = true,
                    forceClimb = true,
                    tileTypes = new List<string>{
                        "Floor1"
                    }
                },
                scoreEffect = 0,
                maxLifeExpectancyEffect = 3

            },
            /*new BotFitnessRuleData{
                eventType = BotFitnessRuleAction.TILE_ENTER,
                tileBonusMode = TileEnterFitnessRule.TileBonusModes.HAS_NOT_TOUCHED_BEFORE,
                tileFilterData =  new TileFilterData{
                    tileTypes = new List<string>{
                        "WaypointTile"
                    }
                },
                scoreEffect = 2000
            },*/
            new BotFitnessRuleData{
                eventType = BotFitnessRuleAction.TILE_ENTER,
                tileFilterData =  new TileFilterData{
                    tileTypes = new List<string>{
                        "LavaTile"
                    }
                },
                scoreEffect = -100
            },

             new BotFitnessRuleData{
                eventType = BotFitnessRuleAction.TILE_ENTER,
                tileFilterData =  new TileFilterData{
                    tileTypes = new List<string>{
                        "OuterWall1"
                    }
                },
                scoreEffect = -1
            }

        };
    }
    public BotFitnessController GetNewBotFitnessController(NPCNNetController npcnNetController)
    {
        BotFitnessController botFitnessController =new BotFitnessController();
        botFitnessController.Init(npcnNetController);
        foreach (FitnessManagerConfigData.BotFitnessRuleData botFitnessRuleData in botFitnessRules)
        {
            // BotFitnessController.BotFitnessRule botFitnessRule = GameManager.instance.serializerHelper.InitFitnessRule(botFitnessRuleData, botFitnessController);
            // botFitnessController.rules.Add(botFitnessRule);
        }
        return botFitnessController;
    }

    
}
