using System;
using System.Collections.Generic;
using services;
using services.Seed;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ChaosLevel
{
    public static readonly int Border = 8;
    public static readonly Vector2 MapDimensions = new Vector2(18, 12);
    public List<ChaosTeam> teams = new List<ChaosTeam>();
    public List<ChaosEntity> entities = new List<ChaosEntity>();
    public ChaosBattleBasicScoreCounter ScoreCounter;
    public CTBrainMaker brainMaker;
    public IDictionary<string, NPCControllerBase> bots = new Dictionary<string, NPCControllerBase>();
    private float timeRemaining;

    public void ResetTimeRemaining()
    {
        timeRemaining = 15;
    }
    public void InitLevel()
    {
        ScoreCounter = new ChaosBattleBasicScoreCounter(this);
        teams.Add(new ChaosTeam("Team 1"));
        teams.Add(new ChaosTeam("Team 2"));
        ResetTimeRemaining();
        for (int x = 0 - Border; x < MapDimensions.x + Border; x++)
        {
            for (int y = 0 - Border; y < MapDimensions.y + Border; y++)
            {
                TileBase tile;
                Tilemap tilemap;
                Boolean isOutsideOfBattleField = (
                    x < 0 ||
                    x >= MapDimensions.x ||
                    y < 0 ||
                    y >= MapDimensions.y
                );
                Boolean drawWall = (
                    (
                        (
                            x == -1 || 
                            x == MapDimensions.x + 1
                        ) && 
                        (
                            y >= -2 &&
                            y <= MapDimensions.y + 2
                        )
                    ) ||
                    (
                        (
                            x >= -1 &&
                            x <= MapDimensions.x + 1
                        ) &&
                        (
                            y == -1 ||
                            y == -2 ||
                            y == MapDimensions.y + 1 ||
                            y == MapDimensions.y + 2
                        )
                    )
                );
                if (drawWall) {
                    tile = GameManager.instance.PrefabManager.GetTile("ChainFenceTile");
                    tilemap = GameManager.instance.wallTilemap;
                    
                    tilemap.SetTile(
                        new Vector3Int(x, y, -1),
                        tile
                    );
                }
                
                    tile = GameManager.instance.PrefabManager.GetTile("GrassTile");
                    tilemap = GameManager.instance.floorTilemap;
                    
                    tilemap.SetTile(
                        new Vector3Int(x, y, 0),
                        tile
                    );
                
            
                tilemap.SetTile(
                    new Vector3Int(x, y, 0),
                    tile
                );
                if (!isOutsideOfBattleField)
                {
                    ChaosTeam team = null;
                    int? placeY = null;
                    if (y == 0)
                    {
                        placeY = y;
                        team = teams[0];
                    }else if (y == MapDimensions.y - 1)
                    {
                        placeY = y;
                        team = teams[1];
                    }

                    if (team != null)
                    {
                        if ((x % 4) == 0)
                        {
                            SpawnNPC(x, y, team);
                        }
                    }
                    

                    ChaosSeed chaosSeed = GameManager.instance.ChaosSeed.Spawn("_" + x + "," + y);
                    if (y == MapDimensions.y / 2)
                    {
                        int val = chaosSeed.Next(0, 7);
                        if (val == 1)
                        {
                            SwordMeeleWeaponItem swordObject = GameManager.instance.PrefabManager
                                .Get("SwordMeeleWeaponItem").GetComponent<SwordMeeleWeaponItem>();

                            swordObject.transform.localPosition = new Vector3(x, y, -2);
                            swordObject.Init();
                            entities.Add(swordObject);
                        }
                        else if (val == 2)
                        {
                            MedkitItem medkitItem = GameManager.instance.PrefabManager.Get("MedkitItem")
                                .GetComponent<MedkitItem>();

                            medkitItem.transform.localPosition = new Vector3(x, y, -2);
                            medkitItem.Init();
                            entities.Add(medkitItem);
                        } else if (val == 3)
                        {
                            TireShieldItem chaosShieldItem = GameManager.instance.PrefabManager.Get("TireShieldItem")
                                .GetComponent<TireShieldItem>();

                            chaosShieldItem.transform.localPosition = new Vector3(x, y, -2);
                            chaosShieldItem.Init();
                            entities.Add(chaosShieldItem);
                        }
                    }
                }

            }
        }
        GameManager.instance.Camera.transform.localPosition = new Vector3(
            MapDimensions.x / 2,
            MapDimensions.y / 2,
            -10
        );
    }

    public ChaosNPCEntity SpawnNPC(int x, int y, ChaosTeam team)
    {
        
        // ((TrainBasicGameMode)GameManager.instance.gameMode).SpawnNPC(x, y, team);
        
        
        /*
        ChaosNPCEntity chaosNpcEntity = GameManager.instance.PrefabManager.Get("ChaosNPCEntity")
            .GetComponent<ChaosNPCEntity>();
                     */

        NPCNNetController npcnNetController = ((TrainBasicGameMode)GameManager.instance.gameMode).speciesManager.SpawnBot();
        //chaosNpcEntity.AttachBotController(npcnNetController);
        npcnNetController.entity.transform.localPosition = new Vector3(x, y, 0);
        npcnNetController.entity.Init();
        npcnNetController.entity.SetTeam(team);
        //boatObject.SetBoatData(boatData);
        entities.Add(npcnNetController.entity);
        return npcnNetController.entity;
    }

    public void Tick()
    {
        //TODO Move this somewhere better
        Dictionary<ChaosTeam, int> teamCounts = new Dictionary<ChaosTeam, int>();
        teams.ForEach((team =>
        {
            teamCounts.Add(team, 0);    
        }));
        timeRemaining -= Time.deltaTime;
        GameManager.instance.level.entities.ForEach((entity =>
        {
            ChaosNPCEntity testChaosNpcEntity = entity.GetComponent<ChaosNPCEntity>();
            if (!testChaosNpcEntity)
            {
                return;
            }

            if (!testChaosNpcEntity.IsAlive())
            {
                return;
            }

            int count = teamCounts[testChaosNpcEntity.GetTeam()];
            teamCounts[testChaosNpcEntity.GetTeam()] = count + 1;
        }));
        bool triggerReset = false;
        teams.ForEach((team =>
        {
            if (teamCounts[team] == 0)
            {
                triggerReset = true;
                Debug.Log(team.name + " LOSES");
                
            }  
        }));
        if (timeRemaining <= 0)
        {
            triggerReset = true;
        }
        if (triggerReset)
        {
            ((TrainBasicGameMode)GameManager.instance.gameMode).EndMatch();
            CleanUp();
            InitLevel();
        }
    }

    public void CleanUp()
    {
        entities.ForEach((entity) =>
        {
            entity.CleanUp();
           //  Object.Destroy(entity.gameObject);
           entity.gameObject.SetActive(false);
        });
        entities.Clear();
        teams.Clear();
    }

    public Vector3Int GetRandomFloorTileVec()
    {
      return new Vector3Int(
            (int) Random.Range(0, MapDimensions.x),
            (int) Random.Range(0, MapDimensions.y),
                0
        );
    }

    public ChaosEntity FindClosestEntity(Vector3 vector3, Func<ChaosEntity, bool> test)
    {
        ChaosEntity closestEntity = null;
        float closestEntityDist = 99999;
     
        entities.ForEach((entity => {
            if (!test(entity))
            {
                return;
            }
            float currEnemyDist = (entity.transform.position - vector3).sqrMagnitude;
            if (
                !closestEntity ||
                currEnemyDist < closestEntityDist  
            ) {
                closestEntity = entity;
                closestEntityDist = currEnemyDist;
            }
        })); 
        return closestEntity;
    }
    public NPCControllerBase InitNewBot(NPCNNetController parentBotController){
        return InitNewBot(parentBotController, true);
    }

    public NPCControllerBase InitNewBot(NPCNNetController parentBotController, bool spawnAsParent){
        

        NNet nNet = null;

        NPCNNetController npcController = null;
        
        if(!spawnAsParent){
            npcController = parentBotController;

            if(npcController.ChaosNpcEntity != null){
                npcController.ChaosNpcEntity.gameObject.SetActive(false);
                //Destroy(botController.entity);
                //botController.entity = null;
            }
            if (!bots.ContainsKey(npcController.id))
            {
                bots.Add(npcController.id, npcController);
            }


            GameManager.instance.gameMode.OnInitNPC(npcController);
            return npcController;
           
        }
        BrainMaker.Action brainMakerAction = null;
        if(parentBotController == null){

            int generation = 0;
            if(nNet != null){
              
                generation = nNet.generation;
            }else{
                nNet = new NNet();
            }
            string _id = "bot_" + generation + "_" + bots.Count;
            npcController = new NPCNNetController();
            npcController.Init(_id);
            brainMakerAction = new BrainMaker.Action();
            brainMakerAction.ParentNpcnNetController = parentBotController;
            brainMakerAction.resultNNet = nNet;
            brainMakerAction.mutationRateData = GameManager.instance.trainingRoomData.brainMakerConfigData.mutationRateData;
            brainMaker.Populate(brainMakerAction);


            /*npcController.botBiology = GameManager.instance.trainingRoomData.botBiologyData.MutateRandom();
            npcController.botBiology.PopulateRandom();
*/

            npcController.AttachNNet(brainMakerAction.resultNNet);
           
        }else{
            brainMakerAction = new BrainMaker.Action();
            NNetData nNetData = parentBotController.nNet.GetSerializer();

            string _id = "bot_" + (parentBotController.nNet.generation + 1) + "_" + bots.Count;
            npcController = new NPCNNetController();
            npcController.Init(_id);
            brainMakerAction.resultNNet = brainMaker.ParseNNetData(nNetData);
            brainMakerAction.resultNNet.generation += 1;
            brainMakerAction.mutationRateData = parentBotController.speciesObject.brainMakerMutationRateData;
            /* if(brainMakerAction.resultNNet.generation != (parentBotController.nNet.generation + 1)){
                Debug.LogError("1 - BotGeneration Missmatch");
            } */
          
            brainMakerAction.ParentNpcnNetController = parentBotController;
           
            brainMaker.Populate(brainMakerAction);
            brainMakerAction.resultNNet.generation = parentBotController.nNet.generation + 1;
            /* if (brainMakerAction.resultNNet.generation != (parentBotController.nNet.generation + 1))
            {
               Debug.LogError("2 - BotGeneration Missmatch");
            } */
            // npcController.botBiology = parentBotController.botBiology.MutateRandom();

            npcController.AttachNNet(brainMakerAction.resultNNet);

           
            //TODO: Add this to the original breed behavior
            BreedAction.CompareResult compareResult = npcController.CompareWithBot(parentBotController);
            brainMakerAction.parentNNetSimilarityScore = compareResult.score;
            if (compareResult.score.Equals(0))
            {
                brainMakerAction.flagAsFailedToEvolve = true;
               
                brainMaker.MutateNEATNNet(brainMakerAction);
                npcController.AttachNNet(brainMakerAction.resultNNet);
                compareResult = npcController.CompareWithBot(parentBotController);
                brainMakerAction.parentNNetSimilarityScore = compareResult.score;
            }


          


        }

        bots.Add(npcController.id, npcController);
     

        
        GameManager.instance.gameMode.OnInitNPC(npcController);
        if(
            parentBotController != null &&
            npcController.nNet.generation != (parentBotController.nNet.generation + 1)
        ){
            Debug.LogError("3 - BotGeneration Missmatch: " + npcController.nNet.generation + " != " + (parentBotController.nNet.generation + 1));
        }
        npcController.brainMakerAction = brainMakerAction;
        brainMakerAction = null;
        return npcController;
    }
}
