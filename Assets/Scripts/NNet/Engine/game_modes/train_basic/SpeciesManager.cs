using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class SpeciesManager{
    public TrainBasicGameMode gameMode;
    protected int _totalSpeciesCount = 0;
    public float globalHighScore = 0;
    public SpeciesFitnessSortingBlock speciesFitnessSortingBlock;
    public SpeciesFitnessSortingBlock lastGenerationSpeciesFitnessSortingBlock = null;
    public IDictionary<string, SpeciesObject> species = new Dictionary<string, SpeciesObject>();
    public int spawnCountThisGen = 0;
    //Create species?
    protected UnityEvent onSpeciesExtinct = new UnityEvent();

    protected List<SpeciesObject>  spawnSortedSpeciesObjects = new List<SpeciesObject>();
    protected int spawnSortIndex = 0;

    public int realMaxSpawnCount = 0;
    //Compare species
    public SpeciesManager(TrainBasicGameMode _gameMode){
        gameMode = _gameMode;
        speciesFitnessSortingBlock = new SpeciesFitnessSortingBlock();
        speciesFitnessSortingBlock.memberCapacity = gameMode.fitnessManagerConfigData.maxTopFittestSpeciesSorageCountPerGeneration;
    }
    public void OnSpeciesExtinct(UnityAction fun){
        onSpeciesExtinct.AddListener(fun);
    }
    public SpeciesObject CreateNewSpecies(){
        SpeciesObject speciesObject = new SpeciesObject();
        speciesObject.Init("species_" + _totalSpeciesCount, this);
        _totalSpeciesCount += 1;
        species.Add(speciesObject.id, speciesObject);
        return speciesObject;
    }
    public NPCNNetController SpawnBot()
    {
        SpeciesObject speciesObject = null;
        if (species.Count < gameMode.fitnessManagerConfigData.minSpeciesCount)
        {
            speciesObject = CreateNewSpecies();
            spawnCountThisGen += 1;
            return speciesObject.SpawnBot();
        }
        if (
            !gameMode.fitnessManagerConfigData.spawnSpeciesWeightByScore ||
            spawnSortedSpeciesObjects.Count == 0

        )
        {
            //Pick a random species
            speciesObject = GetRandom();
            //Tell it to spawn a new bot

        }
        else {

           
            if (spawnSortIndex > (spawnSortedSpeciesObjects.Count - 1))
            {
                //Debug.LogError/*throw new System.Exception*/(" spawnSortIndex out of bounds:" + spawnSortIndex + " >= " + spawnSortedSpeciesObjects.Count);
                speciesObject = GetRandom();
            }
            else
            {
                if (
                    spawnCountThisGen > spawnSortedSpeciesObjects[spawnSortIndex].spawnPosition
                )
                {
                    if(spawnSortedSpeciesObjects[spawnSortIndex].spawnCountThisGen < (gameMode.fitnessManagerConfigData.maxTopFittestSorageCountPerGeneration * 2)){
                        throw new System.Exception("species some how spawned < minSpawn count");
                    }
                    spawnSortIndex += 1;
                    if (spawnSortIndex >= spawnSortedSpeciesObjects.Count)
                    {

                        //Debug.LogError("LastSpawn Pos: " + spawnSortedSpeciesObjects[spawnSortedSpeciesObjects.Count - 1].spawnPosition + " - SpawnCount: " + spawnCountThisGen);
                        //throw new System.Exception("SpawnSortIndex out of range " + spawnSortIndex + " - Species Count: " + spawnSortedSpeciesObjects.Count);
                        speciesObject = GetRandom();


                    }
                    //Debug.Log("Setting Species Object: " + spawnSortedSpeciesObjects[spawnSortIndex].id + " - spawnSortIndex:" + spawnSortIndex + " spawnPosition:" + spawnSortedSpeciesObjects[spawnSortIndex].spawnPosition);
                }

               
                speciesObject = spawnSortedSpeciesObjects[spawnSortIndex];

            }

        }
        spawnCountThisGen += 1;
        //Debug.Log("spawnCountThisGen: " + spawnCountThisGen + " - spawnSortIndex: " + spawnSortIndex + " - SpeciesId: " + speciesObject.id);
        return speciesObject.SpawnBot();

    }
    public SpeciesObject GetRandom(){
        int index = UnityEngine.Random.Range(0, species.Values.Count);
        SpeciesObject speciesObject = System.Linq.Enumerable.ToList(species.Values)[index];
        return speciesObject;
    }
    /*
     * Check to see if species have not improved in a while
     */
    public void GenerationCheck(){
        //Debug.Log("----------------------------\n GENERATION CHECK \n -------------------------- \n");
       
        List<string> toRemove = new List<string>();
       
        foreach (SpeciesObject speciesObject in species.Values)
        {
            speciesObject.IterateGeneration();

            speciesFitnessSortingBlock.TestSpeciesStat(speciesObject,  speciesObject.LastSortableScore());
            

        }
       
        foreach (SpeciesObject speciesObject in species.Values)
        {



            float lastSortableScore = speciesObject.LastSortableScore();
            float score = speciesFitnessSortingBlock.GetSpeciesObjectScore(speciesObject);
           
            if (
                (score.Equals(-1f)) &&
                (speciesObject.speciesAge > gameMode.fitnessManagerConfigData.speciesMaxTurnsToOptimizeBeforeSubjectToExtinction)
            ){
                toRemove.Add(speciesObject.id);
            }else if (
                speciesObject.speciesAge <= 1 ||
                speciesObject.currentHighSortableScore < lastSortableScore
            ){
                
                speciesObject.currentHighSortableScore = lastSortableScore;
                speciesObject.generationsSinceLastImprovment = 0;
                speciesObject.ResetMutationRates();
                if(speciesObject.trendingDir <= 0){
                    //Change the trendingDir 
                    speciesObject.trendingDir = 1;
                    speciesObject.trendingDuration = 0;
                }
                speciesObject.trendingDuration += 1;


            } else {

                if (speciesObject.trendingDir >= 0)
                {
                    //Change the trendingDir 
                    speciesObject.trendingDir = -1;
                    speciesObject.trendingDuration = 0;
                }
                speciesObject.trendingDuration += 1;
                
                if (
                    !gameMode.fitnessManagerConfigData.speciesMaxGenerationsSinceLastImprovment.Equals(-1) &&
                    speciesObject.speciesAge > gameMode.fitnessManagerConfigData.speciesMaxTurnsToOptimizeBeforeSubjectToExtinction
                ){
                    speciesObject.generationsSinceLastImprovment += 1;
                    if (speciesObject.generationsSinceLastImprovment > gameMode.fitnessManagerConfigData.speciesMaxGenerationsSinceLastImprovment)
                    {
                        //Kill off the species
                        toRemove.Add(speciesObject.id);

                    }
                }
            }


            if(globalHighScore.Equals(0) || lastSortableScore > globalHighScore){
                
                globalHighScore = lastSortableScore;
                NPCNNetController npcnNetController = null;
                if (gameMode.fitnessManagerConfigData.forceNewFitnessSortingBlockEachGeneration)
                {
                    npcnNetController = speciesObject.lastGenFitnessSortingBlock.GetBotControllerByScore(globalHighScore);
                  
                }
                else
                {
                    npcnNetController = speciesObject.fitnessSortingBlock.GetBotControllerByScore(globalHighScore);
                }
                /*if(GameManager.instance.fitnessManager.globalLeaderBoard.TestBotStat(botController, globalHighScore)){
                    botController.keepAround = true;
                }*/
                if (npcnNetController != null)
                {
                    npcnNetController.maxSpawnCount += 1;
                    if (globalHighScore > 1000)
                    {
                        BotControllerData botControllerData = npcnNetController.ToData();
                        botControllerData.Save();
                    }
                    GameManager.instance.menuManager.chaosEntityDetailPanel.Show(npcnNetController.entity);
                }
               



                if(gameMode.trainBasicGameModeSaveStateData.generation > 20){
                    string saveStateName = "auto_" + gameMode.trainBasicGameModeSaveStateData.generation;
                    //Debug.Log("Saveing -  State Name: " + saveStateName);
                    //GameManager.instance.SaveStateData(saveStateName); 

                }
            }

        }




        foreach (string speciesId in toRemove)
        {
            
            speciesFitnessSortingBlock.RemoveById(speciesId);
            //UnityEngine.Object.Destroy(species[speciesId]);
            species.Remove(speciesId);
        }

        for (int i = species.Count; i < gameMode.fitnessManagerConfigData.minSpeciesCount; i++)
        {
            CreateNewSpecies();
        }

        if (toRemove.Count > 0)
        {
            onSpeciesExtinct.Invoke();
        }
        //UnityEngine.Object.Destroy(lastGenerationSpeciesFitnessSortingBlock);
        lastGenerationSpeciesFitnessSortingBlock = speciesFitnessSortingBlock;
        speciesFitnessSortingBlock = new SpeciesFitnessSortingBlock();
        speciesFitnessSortingBlock.memberCapacity = gameMode.fitnessManagerConfigData.maxTopFittestSpeciesSorageCountPerGeneration;

        spawnCountThisGen = 0;
        spawnSortIndex = 0;
        int minSpawnCount = (gameMode.fitnessManagerConfigData.minSpeciesSpawnAllowence);
        if (gameMode.fitnessManagerConfigData.spawnSpeciesWeightByScore)
        {
            spawnSortedSpeciesObjects.Clear();
            //TOtal up scores from species that are left
            float lowestScore = 0;
            foreach (SpeciesObject speciesObject in species.Values)
            {
                if (
                    speciesObject.LastSortableScore() < lowestScore)
                {
                    lowestScore = speciesObject.LastSortableScore();

                }
            }
            float totalScores = 0f;
            foreach (SpeciesObject speciesObject in species.Values)
            {
                totalScores += speciesObject.LastSortableScore() - lowestScore;
            }
            SpeciesObject lastSpeciesObject = null;
            List<SpeciesObject> xSpeciesObject = System.Linq.Enumerable.ToList(species.Values);
            xSpeciesObject.Sort((x, y) => y.LastSortableScore().CompareTo(x.LastSortableScore()));
            foreach (SpeciesObject speciesObject in xSpeciesObject)
            {
                float lastHighScore = speciesObject.LastSortableScore();

                float spawnChance = (lastHighScore - lowestScore) / totalScores;
                int posOffest = 0;
                if (lastSpeciesObject != null)
                {
                    posOffest = lastSpeciesObject.spawnPosition;
                }
                int spawnPosChance =  minSpawnCount + (int) Mathf.Round(spawnChance * gameMode.fitnessManagerConfigData.bonusSpawnCountPerGeneration);
                if(spawnPosChance < minSpawnCount){
                    spawnPosChance = minSpawnCount;
                }
                speciesObject.spawnPosition = (posOffest + spawnPosChance);
                lastSpeciesObject = speciesObject;
            }
            realMaxSpawnCount =  lastSpeciesObject.spawnPosition;
            spawnSortedSpeciesObjects = System.Linq.Enumerable.ToList(species.Values);
            spawnSortedSpeciesObjects.Sort((x, y) => x.spawnPosition.CompareTo(y.spawnPosition));
            string debugLog = "--------------------------\n";
            foreach(SpeciesObject speciesObject in spawnSortedSpeciesObjects){
                debugLog += speciesObject.id + " " + speciesObject.spawnPosition + "\n";
            }
            //Debug.Log(debugLog);
        }
       
        /*
        if (GameManager.instance.fitnessManager.currentGeneration > 5)
        {
            if (GameManager.instance.fitnessManager.fitnessManagerConfigData.minSpeciesCount > 3)
            {
                GameManager.instance.fitnessManager.fitnessManagerConfigData.minSpeciesCount -= 1;
                Debug.Log("Dropping MinSpeciesCount: " + GameManager.instance.fitnessManager.fitnessManagerConfigData.minSpeciesCount.ToString());
                //GameManager.instance.fitnessManager.fitnessManagerConfigData.maxTopFittestSpeciesSorageCountPerGeneration -= 1;
            }
        }
        */

    }

    public void AdjustSpeciesMutationRate(GenerationScoreResultData generationScoreResultData)
    {
        foreach(SpeciesObject speciesObject in species.Values){
            speciesObject.AdjustSpeciesMutationRate(generationScoreResultData);
        }
    }
}
