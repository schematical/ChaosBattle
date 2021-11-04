using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpeciesObject: CTBaseObject
{
    
    
    private string _id;
    public int startAge;
    public List<NPCNNetController> organisims;
    protected int _speciesAge;
    public int generationsSinceLastImprovment;
    public float currentHighSortableScore = -99999;
   
    public FitnessSortingBlock fitnessSortingBlock;
    public FitnessSortingBlock lastGenFitnessSortingBlock;
    public NPCNNetController FirstNpcnNetController;
    public IDictionary<string, float> lastGenBotScores;
    public IDictionary<string, float> botScores;
    public GenerationScoreResultData lastScoreResult;
    public List<GenerationScoreResultData> historicalScoreInfo = new List<GenerationScoreResultData>();
    public int spawnPosition = 0;
    public int spawnCountThisGen = 0;
    public SpeciesManager speciesManager;
    public BrainMakerMutationRateData brainMakerMutationRateData;
    public int failedToEvolveCount = 0;
    public int trendingDir = 0;
    public int trendingDuration = 0;


    public void Init(string id, SpeciesManager _speciesManager){
        _id = id;
        _speciesAge = 0;
        speciesManager = _speciesManager;
        startAge = speciesManager.gameMode.trainBasicGameModeSaveStateData.generation;

        fitnessSortingBlock = new FitnessSortingBlock();
       
        fitnessSortingBlock.memberCapacity = speciesManager.gameMode.fitnessManagerConfigData.maxTopFittestSorageCountPerGeneration;

        lastGenBotScores = new Dictionary<string, float>();
        botScores = new Dictionary<string, float>();
        historicalScoreInfo = new List<GenerationScoreResultData>();
        organisims = new List<NPCNNetController>();

        ResetMutationRates();
    }
    public override string _class_name
    {
        get
        {
            return "SpeciesObject";
        }
    }
    public string id
    {
        get { return _id; }
    }
    public int speciesAge
    {
        get { return _speciesAge; }
    }
    public float failedToEvolveRate{
        get{
            return (((float)failedToEvolveCount) / ((float)spawnCountThisGen));
        }
    }

    public float LastSortableScore(){
        if(speciesAge == 0){
            return 1;
        }
        switch(speciesManager.gameMode.fitnessManagerConfigData.globalSpeciesSortMode){
            case(FitnessManagerConfigData.GlobalSpeciesSortMode.MAX):
                return lastScoreResult.max;
            case (FitnessManagerConfigData.GlobalSpeciesSortMode.MIN):
                return lastScoreResult.min;
            case (FitnessManagerConfigData.GlobalSpeciesSortMode.MEDIAN):
                return lastScoreResult.median;
            case (FitnessManagerConfigData.GlobalSpeciesSortMode.MEAN):
                return lastScoreResult.mean;
            default:
                throw new System.Exception("Invalid `fitnessManagerConfigData.globalSpeciesSortMode`: " + speciesManager.gameMode.fitnessManagerConfigData.globalSpeciesSortMode);

        }
    }
    public NPCNNetController SpawnBot(){
        NPCNNetController childNpcnNetController = null;
        NPCNNetController randomNpcnNetController = null;
        spawnCountThisGen += 1;
        //Pick someone from the top X  
        if (speciesAge == 0)
        {
            if (FirstNpcnNetController == null)
            {
                FirstNpcnNetController = speciesManager.gameMode.SpawnNPC(null);
                FirstNpcnNetController.keepAround = true;
                FirstNpcnNetController.SetSpecies(this);
                FirstNpcnNetController.nNet.generation = -1;
                childNpcnNetController = FirstNpcnNetController;
            }else{
                childNpcnNetController = speciesManager.gameMode.SpawnNPC(FirstNpcnNetController);
            }


        }
        else
        {

            randomNpcnNetController = GetRandomHighScoringBotController();
            if(randomNpcnNetController == null){
                Debug.LogError("No HighScoringBotController Found: " + id);
                if(FirstNpcnNetController == null){
                    throw new System.Exception("And there is not FIrstBotController: " + id);
                }
                randomNpcnNetController = FirstNpcnNetController;
            }

            if (
                speciesManager == null ||
                speciesManager.gameMode == null ||
                speciesManager.gameMode.fitnessManagerConfigData == null 
            )
            {
                throw new System.Exception("This is silly and pretty much imposable");
            }

            if (
                speciesManager.gameMode.fitnessManagerConfigData.forceNewFitnessSortingBlockEachGeneration &&
                randomNpcnNetController != null &&
                randomNpcnNetController.nNet.generation != (this.speciesAge - 1) && 
                randomNpcnNetController.nNet.generation != -1
            ){
                throw new System.Exception("SpeciesAge - BotController Generation mix up 1: " + randomNpcnNetController.nNet.generation + " != " + (this.speciesAge - 1));
            }
            
            childNpcnNetController = speciesManager.gameMode.SpawnNPC(randomNpcnNetController);
            if(randomNpcnNetController.nNet.generation == -1){
                childNpcnNetController.nNet.generation = speciesAge;
            }
             if (
               speciesManager.gameMode.fitnessManagerConfigData.forceNewFitnessSortingBlockEachGeneration &&
                childNpcnNetController.nNet.generation != this.speciesAge
            )
            {
                throw new System.Exception("SpeciesAge - BotController Generation mix up 3: " + childNpcnNetController.nNet.generation + " != " + this.speciesAge);
            }
        }

        childNpcnNetController.SetSpecies(this);
        organisims.Add(childNpcnNetController);
        childNpcnNetController.nNet.OrigenCheck();

        if (
            speciesManager.gameMode.fitnessManagerConfigData.forceNewFitnessSortingBlockEachGeneration &&
            childNpcnNetController.nNet.generation != this.speciesAge &&
            childNpcnNetController.nNet.generation != -1
        )
        {
            if(randomNpcnNetController!= null){
                Debug.LogError("ParentGen Fail: " + randomNpcnNetController.nNet.generation);
            }
            throw new System.Exception("SpeciesAge - BotController Generation mix up 2: " + childNpcnNetController.nNet.generation + " != " + this.speciesAge);
        }
        if(childNpcnNetController.brainMakerAction.flagAsFailedToEvolve){
            failedToEvolveCount += 1;
        }
        return childNpcnNetController;
    }
    public NPCNNetController GetRandomHighScoringBotController(){
        NPCNNetController randomNpcnNetController = null;
       
        if (speciesManager.gameMode.fitnessManagerConfigData.forceNewFitnessSortingBlockEachGeneration)
        {
            if (lastGenFitnessSortingBlock == null)
            {
                
                return FirstNpcnNetController;
            }
            if(lastGenFitnessSortingBlock.storedStats.Count == 0){
                throw new System.Exception("Some how the lastGenFitnessSortingBlock.storedStats.Count == 0");
            }
            randomNpcnNetController = lastGenFitnessSortingBlock.GetRandom();
        }
        else
        {
            if (speciesAge == 0)
            {
                return FirstNpcnNetController;
            }
            randomNpcnNetController = fitnessSortingBlock.GetRandom();
        }
        return randomNpcnNetController;
    }
    public void LogScore(NPCNNetController npcnNetController, float score){
        if(!botScores.ContainsKey(npcnNetController.id)){
            botScores.Add(npcnNetController.id, score);
        }else{
            float origScore = botScores[npcnNetController.id];
            botScores[npcnNetController.id] = origScore + score / npcnNetController.spawnCount;
        }
       
    }

 
    public void IterateGeneration(){
       
        //Debug.Log("Iterating Generation:" + _speciesAge + "/" + GameManager.instance.fitnessManager.currentGeneration + " - " + id);
        if (spawnCountThisGen == 0)
        {
            Debug.LogError("Species Object Spawn Count is 0: " + id);
        }

       





        float preCull_totalScore = 0;
        float totalScore = 0;
        float medianScoreRoundTo = GameManager.instance.trainingRoomData.fitnessManagerConfigData.medianScoreRoundTo;
        IDictionary<int, int> preCull_median = new Dictionary<int, int>();
        lastScoreResult = new GenerationScoreResultData();
        lastScoreResult.generation = speciesManager.gameMode.trainBasicGameModeSaveStateData.generation;

        lastScoreResult._species = id;
        lastScoreResult._speciesAge =  _speciesAge;


        foreach (float lastScore in botScores.Values)
        {

            preCull_totalScore += lastScore;
            int medianScore = (int)Mathf.Round(Mathf.Round(lastScore / medianScoreRoundTo) * medianScoreRoundTo);
            if (!preCull_median.ContainsKey(medianScore))
            {
                preCull_median.Add(medianScore, 0);
            }
            preCull_median[medianScore] += 1;

            if (
                lastScoreResult.preCull_max.Equals(0) ||
                lastScore > lastScoreResult.preCull_max
            ){
                lastScoreResult.preCull_max = lastScore;
            }
            if (
                lastScoreResult.preCull_min.Equals(0) ||
                lastScore < lastScoreResult.preCull_min
            )
            {
                lastScoreResult.preCull_min = lastScore;
            }


        }

        int preCull_highestCount = 0;
        int preCull_highestScoreIndex = 0;
        foreach (int key in preCull_median.Keys)
        {
            if (preCull_median[key] > preCull_highestCount)
            {
                preCull_highestScoreIndex = key;
                preCull_highestCount = preCull_median[key];
            }
        }
        //Debug.Log("Median Score:" + highestScoreIndex);
        lastScoreResult.preCull_median = preCull_highestScoreIndex;
        lastScoreResult.preCull_mean = preCull_totalScore / botScores.Count;





        IDictionary<int, int> median = new Dictionary<int, int>();
        List<NPCNNetController> toRemove = new List<NPCNNetController>();
        int botsInTop = 0;
        foreach (NPCNNetController botController in organisims)
        {
            if (!fitnessSortingBlock.IsBotInTopStored(botController)){
                botController.MarkReadyForCleanup();
                toRemove.Add(botController);

            }else {
                float lastScore = GameManager.instance.level.ScoreCounter.GetEntityScore(botController.ChaosNpcEntity);// botController.botControllerScoreData.scores[0];//TODO: Make this better

                totalScore += lastScore;
                botsInTop += 1;
                int medianScore = (int)Mathf.Round(Mathf.Round(lastScore / medianScoreRoundTo) * medianScoreRoundTo);
                if (!median.ContainsKey(medianScore))
                {
                    median.Add(medianScore, 0);
                }
                median[medianScore] += 1;


                if (
                    lastScoreResult.max.Equals(0) ||
                    lastScore > lastScoreResult.max)
                {
                    lastScoreResult.max = lastScore;
                }
                if (
                    lastScoreResult.min.Equals(0) ||
                    lastScore < lastScoreResult.min
                )
                {
                    lastScoreResult.min = lastScore;
                }

            }
        }

       


        int highestCount = 0;
        int highestScoreIndex = 0;
        foreach (int key in median.Keys)
        {
            if (median[key] > highestCount)
            {
                highestScoreIndex = key;
                highestCount = median[key];
            }
        }
        //Debug.Log("Median Score:" + highestScoreIndex);
        lastScoreResult.median = highestScoreIndex;
        lastScoreResult.mean = totalScore / botsInTop;



        if (speciesManager.gameMode.fitnessManagerConfigData.forceNewFitnessSortingBlockEachGeneration)
        {
            if (speciesAge > 0)
            {
                foreach (FitnessSortingBlock.ScoreBlock scoreBlock in lastGenFitnessSortingBlock.storedStats)
                {
                    scoreBlock.NpcnNetController.MarkReadyForCleanup();
                    toRemove.Add(scoreBlock.NpcnNetController);
                }
                //UnityEngine.Object.Destroy(lastGenFitnessSortingBlock);
                lastGenFitnessSortingBlock = null;
            }
            lastGenFitnessSortingBlock = fitnessSortingBlock;
            fitnessSortingBlock = new FitnessSortingBlock();
            fitnessSortingBlock.memberCapacity = speciesManager.gameMode.fitnessManagerConfigData.maxTopFittestSorageCountPerGeneration;
            //.Dispose();

            if (lastGenFitnessSortingBlock.storedStats.Count == 0){
                throw new System.Exception("Some how the lastGenFitnessSortingBlock.storedStats.Count == 0");
            }

        }










        historicalScoreInfo.Add(lastScoreResult);


        lastGenBotScores = botScores;

        spawnCountThisGen = 0;
        failedToEvolveCount = 0;
        botScores = new Dictionary<string, float>();


        foreach (NPCNNetController botController in toRemove)
        {
            organisims.Remove(botController);
        }

        //organisims.Clear();
        _speciesAge += 1;

    }
    public void ResetMutationRates(){
        brainMakerMutationRateData = speciesManager.gameMode.trainingRoomData.brainMakerConfigData.GetNewSpeciesMutationRateData();
    }
	public void AdjustSpeciesMutationRate(GenerationScoreResultData generationScoreResultData)
	{
        //Do some shit...

        //Mutation goes up with the score flattens out
        //or
        //if the

        if(generationsSinceLastImprovment == 0){
            return;
        }



        float drasticMeasuteRate = (float)(trendingDuration) / (float)(speciesManager.gameMode.trainingRoomData.fitnessManagerConfigData.speciesMaxGenerationsSinceLastImprovment);// (float)(generationsSinceLastImprovment) / (float)(speciesManager.gameMode.trainingRoomData.fitnessManagerConfigData.speciesMaxGenerationsSinceLastImprovment);
        float globalScoreDiff = generationScoreResultData.max - generationScoreResultData.min;
        float speciesScoreDiff = lastScoreResult.max - lastScoreResult.min;
        float mutationRate = (1 - speciesScoreDiff / globalScoreDiff) * drasticMeasuteRate;

        if (trendingDir < 0)
        {
            brainMakerMutationRateData.addNewNeuronChance += (int)Math.Ceiling(mutationRate * brainMakerMutationRateData.addNewNeuronChance);
            brainMakerMutationRateData.addNewNeuronDepChance += (int)Math.Ceiling(mutationRate * brainMakerMutationRateData.addNewNeuronDepChance);
            //brainMakerMutationRateData.adjustBiasNeuronChance -= (int)Math.Ceiling(mutationRate * brainMakerMutationRateData.adjustBiasNeuronChance);
            brainMakerMutationRateData.BREED_ACTION_interSpecies += (int)Math.Ceiling(mutationRate * brainMakerMutationRateData.BREED_ACTION_interSpecies);
            //brainMakerMutationRateData.EVOLVE_ACTION_breedChance -= (int)Math.Ceiling(mutationRate * brainMakerMutationRateData.EVOLVE_ACTION_breedChance);
            //brainMakerMutationRateData.EVOLVE_ACTION_mutateChance += (int)Math.Ceiling(mutationRate * brainMakerMutationRateData.EVOLVE_ACTION_mutateChance);
            brainMakerMutationRateData.RefreshOdds();
        }else{
            brainMakerMutationRateData.adjustBiasNeuronRate -= (int)Math.Ceiling(drasticMeasuteRate * brainMakerMutationRateData.adjustBiasNeuronRate);
            brainMakerMutationRateData.adjustNeuronDepRate -= (int)Math.Ceiling(drasticMeasuteRate * brainMakerMutationRateData.adjustNeuronDepRate);
            brainMakerMutationRateData.setNewNeuronDepWeightRate -= (int)Math.Ceiling(drasticMeasuteRate * brainMakerMutationRateData.setNewNeuronDepWeightRate);

        }



	}

	public SpeciesData ToData(){
        SpeciesData speciesData = new SpeciesData();
        speciesData.id = _id;
        speciesData.speciesAge = speciesAge;
        speciesData.startAge = startAge;
        speciesData.generationsSinceLastImprovment = generationsSinceLastImprovment;
        speciesData.currentHighSortableScore = currentHighSortableScore;
        speciesData.lastScoreResult = lastScoreResult;
        speciesData.historicalScoreInfo = historicalScoreInfo;
        if(fitnessSortingBlock != null){
            speciesData.fitnessSortingBlock = fitnessSortingBlock.ToData();
        }

        if(lastGenFitnessSortingBlock != null){
            speciesData.lastGenFitnessSortingBlock = lastGenFitnessSortingBlock.ToData();
        }
       
        foreach(NPCNNetController botController in organisims){
            if (botController != null)
            {
                speciesData.botControllers.Add(botController.ToData());
            }
        }
       
        return speciesData;
    }
    public void ParseData(SpeciesData speciesData){
   
        _id = speciesData.id;
        _speciesAge = speciesData.speciesAge;
        startAge = speciesData.startAge;
        generationsSinceLastImprovment = speciesData.generationsSinceLastImprovment;
        currentHighSortableScore = speciesData.currentHighSortableScore;
        lastScoreResult = speciesData.lastScoreResult;
        historicalScoreInfo = speciesData.historicalScoreInfo;
        foreach (BotControllerData botControllerData in speciesData.botControllers)
        {
            NPCNNetController npcnNetController = new NPCNNetController();
            npcnNetController.Init(botControllerData.id);
            npcnNetController.ParseData(botControllerData);
            //GameManager.instance.botManager.InitNewBot(botController, false);
            npcnNetController.botFitnessController = speciesManager.gameMode.fitnessManagerConfigData.GetNewBotFitnessController(npcnNetController);
            organisims.Add(npcnNetController);
        }

       
        fitnessSortingBlock = new FitnessSortingBlock();
        if (speciesData.fitnessSortingBlock != null)
        {
            fitnessSortingBlock.ParseData(speciesData.fitnessSortingBlock, this);
        }else{
            Debug.LogError("Missing `fitnessSortingBlock`");
        }
        if (speciesData.lastGenFitnessSortingBlock != null)
        {
            lastGenFitnessSortingBlock = new FitnessSortingBlock();
            lastGenFitnessSortingBlock.ParseData(speciesData.lastGenFitnessSortingBlock, this);
        }
       
    }
    //public override void OnDestroy()
    ~SpeciesObject()
	{
        //base.OnDestroy();
        if (lastGenFitnessSortingBlock != null)
        {
            //UnityEngine.Object.Destroy(lastGenFitnessSortingBlock);
            lastGenFitnessSortingBlock = null;
        }
        //UnityEngine.Object.Destroy(fitnessSortingBlock);
        fitnessSortingBlock = null;
        //UnityEngine.Object.Destroy(firstBotController);
        FirstNpcnNetController = null;
        foreach(NPCNNetController botController in organisims){
            botController.MarkReadyForCleanup();
        }
        organisims.Clear();
        FirstNpcnNetController.keepAround = false;
        FirstNpcnNetController.MarkReadyForCleanup();
        FirstNpcnNetController = null;
        fitnessSortingBlock = null;
        speciesManager = null;

	}

    public float GetTrendingScore(){
        float score = trendingDir * trendingDuration;
        return score;
    }

}
