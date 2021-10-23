using UnityEngine;
using System.Collections.Generic;

public class FitnessManager
{

    public static class RespawnMode
    {
        public const string ALL = "ALL";
        public const string INDIVIDUAL = "INDIVIDUAL";
    }


    public FitnessManagerConfigData fitnessManagerConfigData = new FitnessManagerConfigData();
    //public int frameCount = 0;
    //public int simDurationSeconds = 5;
    //public int maxBotCount = 5;
    //public string respawnMode = RespawnMode.INDIVIDUAL;
    //public int globalGeneration = 0;
    //public int spawnCountThisGeneration = 0;
   
    //public FitnessSortingBlock globalLeaderBoard;

    public FitnessManager()
    {

    }
    /*
    public int currentGeneration{
        get{
            return GameManager.instance.trainingRoomSaveStateData.generation;
        }
    }
    public void Tick()
    {

        switch (respawnMode)
        {
            
            case(RespawnMode.ALL):
                TestAll();
            break;

            case (RespawnMode.INDIVIDUAL):
                TestIndividual();
                break;

        }

    }

    public void TestAll(){
        List<BotController> aliveBots = GetAliveBots();
        if(GetSimDurationInSeconds() > simDurationSeconds || aliveBots.Count == 0){
            
            //Trigger score count and spawn
            foreach (string botId in GameManager.instance.botManager.bots.Keys)
            {
                BotController botController = GameManager.instance.botManager.bots[botId];
                if (botController.entity.isAlive()){
                    botController.entity.ChangeHealth(-100);
                }
            }
            for (int i = 0; i < maxBotCount; i++)
            {
                GameManager.instance.botManager.SpawnNextBot();
            }
            frameCount = 0;
            globalGeneration += 1;
        }
    }

    public void TestIndividual()
    {
        //Iterate through each bot
        if (GameManager.instance.speciesManager.realMaxSpawnCount == 0)
        {
            GameManager.instance.speciesManager.realMaxSpawnCount = fitnessManagerConfigData.maxSpawnCountPerGeneration;
        }
        int aliveBotCount = 0;
        //bool hasLastGenBot = false;
        foreach (BotController botController in GameManager.instance.botManager.bots.Values)
        {
            bool hasAttachedEntity = botController.HasAttachedEntity();
         
            if(
                hasAttachedEntity && 
                botController.entity.isAlive()
            )
            {
                aliveBotCount += 1;
                if (botController.gameAgeSeconds > botController.maxLifeExpectancy)
                {
                    botController.entity.ChangeHealth(-100);
                    //Debug.Log("BotController: " + botController.id + " - Death by timeout");
                }
               
            }
           
        }
        GameManager.instance.botManager.CleanUpBotList();

    
        if (spawnCountThisGeneration > GameManager.instance.speciesManager.realMaxSpawnCount)
        {
            if (aliveBotCount > 0)
            {
                return;
            }
            //Iterate generations
            IterateGenerations();
            return;
        }

        if (aliveBotCount < maxBotCount)
        {
            GameManager.instance.botManager.SpawnNextBots();
        }
          

      
       
         
    }



    public void SubmitFinalScoreForLife(BotFitnessController botFitnessController){
        
       

        switch (respawnMode)
        {
            case (RespawnMode.ALL):
                //Hold off and test achivments later

                break;
            case (RespawnMode.INDIVIDUAL):
                //Test achivments now
                BotController bot = botFitnessController.botController;

               

                bool madeItInTop = bot.speciesObject.fitnessSortingBlock.TestBotStat(bot, botFitnessController.cumulativeScore);
                if(!madeItInTop){
                    //bot.MarkReadyForCleanup();
                }
                bot.speciesObject.LogScore(bot, botFitnessController.cumulativeScore);


            break;

        }
         
    }

    public void IterateGenerations(){
        if(globalLeaderBoard == null){
            globalLeaderBoard = ScriptableObject.CreateInstance <FitnessSortingBlock>();
        }
       


        float totalScore = 0;
        float medianScoreRoundTo = GameManager.instance.trainingRoomData.fitnessManagerConfigData.medianScoreRoundTo;
        IDictionary<int, int> median = new Dictionary<int, int>();
        GenerationScoreResultData generationScoreResultData = new GenerationScoreResultData();
        generationScoreResultData.generation = currentGeneration;
        foreach(SpeciesObject speciesObject in GameManager.instance.speciesManager.species.Values){
            foreach (float lastScore in speciesObject.botScores.Values) {
                   
                totalScore += lastScore;
                int medianScore = (int)Mathf.Round(Mathf.Round(lastScore / medianScoreRoundTo) * medianScoreRoundTo);
                if (!median.ContainsKey(medianScore))
                {
                    median.Add(medianScore, 0);
                }
                median[medianScore] += 1;

                if (lastScore > generationScoreResultData.max)
                {
                    generationScoreResultData.max = lastScore;
                }
                if (
                    generationScoreResultData.min.Equals(0) ||
                    lastScore < generationScoreResultData.min
                )
                {
                    generationScoreResultData.min = lastScore;
                }


            }
        }
        int highestCount = 0;
        int highestScoreIndex = 0;
        foreach(int key in median.Keys){
            if(median[key] > highestCount){
                highestScoreIndex = key;
                highestCount = median[key];
            }
        }
        //Debug.Log("Median Score:" + highestScoreIndex);
        generationScoreResultData.median = highestScoreIndex;
        generationScoreResultData.mean = totalScore / spawnCountThisGeneration;
        GameManager.instance.trainingRoomSaveStateData.generationScores.Add(generationScoreResultData);
        GameManager.instance.speciesManager.GenerationCheck(); 



        spawnCountThisGeneration = 0;
        GameManager.instance.botManager.CleanUpLongTermBotList();
        WorldEvent.CleanAll();
        GameManager.instance.trainingRoomSaveStateData.generation += 1;
        Resources.UnloadUnusedAssets();
    }
   */
  





   

}