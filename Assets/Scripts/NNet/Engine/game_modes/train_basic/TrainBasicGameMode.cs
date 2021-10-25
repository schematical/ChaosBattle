using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
public class TrainBasicGameMode : GameModeBase, IGameModeWithFitnessConfig, IGameModeWithSpawnables
{
    
    public int _maxBotCount = 5;
    public FitnessManagerConfigData _fitnessManagerConfigData = null;
    public List<GenerationPerformanceData> generationPerformances = new List<GenerationPerformanceData>();
    public GenerationPerformanceData currentGenerationPerformance;
    public int spawnCountThisGeneration = 0;

    public FitnessSortingBlock globalLeaderBoard;
    public TrainBasicGameModeSaveStateData trainBasicGameModeSaveStateData;
    public SpeciesManager speciesManager;
    public decimal lastSpawnSeconds = 0;
    public UnityEvent onGenerationChange = new UnityEvent();
    public int maxBotCount
    {
        get
        {
            return _maxBotCount;
        }
        set
        {
            _maxBotCount = value;
        }

    }
    public override string type
    {
        get
        {
            return GameManager.GameModeType.TRAIN_BASIC;
        }

    }
    public FitnessManagerConfigData fitnessManagerConfigData
    {
        get
        {
            return _fitnessManagerConfigData;
        }
        set{
            _fitnessManagerConfigData = value;
        }

    }
    public override void Setup(){
        

        //TODO: Make sure this updates correctly
        fitnessManagerConfigData = trainingRoomData.fitnessManagerConfigData;

        trainBasicGameModeSaveStateData = new TrainBasicGameModeSaveStateData();
        GameManager.instance.level.brainMaker = new CTBrainMaker();
        GameManager.instance.level.brainMaker.Init(trainingRoomData.brainMakerConfigData, this);


        speciesManager = new SpeciesManager(this);
        GameManager.instance.level.CleanUp();
        // GameManager.instance.level.SetupScene(trainingRoomData);
       
        GameManager.instance.menuManager.HideAllMenues();
/*        GameManager.instance.menuManager.botListPanel.Show();
        GameManager.instance.menuManager.botListPanel.destroyOnDeath = true;
        GameManager.instance.menuManager.menuBarPanel.Show();
        GameManager.instance.menuManager.footerPanel.Show();*/

        GameManager.instance.cameraManager.paningEnabled = true;
        GameManager.instance.inputManager.ListenToKeyboardInput();

        currentGenerationPerformance = new GenerationPerformanceData();

 

    }
    public override void Tick(){
        if (GameManager.instance.paused)
        {
            return;
        }

      
        //debugManager.ClearDebugLines();
      
        // TickFitness();


        GameManager.instance.level.Tick();
       //  WorldEvent.CleanTick();

        if (currentGenerationPerformance != null)
        {
            //currentGenerationPerformance.
        }
    }
    public override void Suspend(){
        
    }
    public override void Shutdown(){
        speciesManager.species.Clear();
    }
    public void TickFitness(){

        //switch (respawnMode){
            /*
            case(RespawnMode.ALL):
                TestAll();
            break;
            */
            //case (RespawnMode.INDIVIDUAL):
                TickTestIndividual();
               // break;

        //}
    }
    public void TickTestIndividual(){
        //Iterate through each bot
        if (speciesManager.realMaxSpawnCount == 0)
        {
            speciesManager.realMaxSpawnCount = trainingRoomData.fitnessManagerConfigData.minSpeciesSpawnAllowence * trainingRoomData.fitnessManagerConfigData.minSpeciesCount;
        }
        int aliveBotCount = 0;
        //bool hasLastGenBot = false;
        foreach (NPCControllerBase botController in GameManager.instance.level.bots.Values)
        {
            bool hasAttachedEntity = botController.HasAttachedEntity();
         
            if(
                hasAttachedEntity

            )
            {
                /*if (botController.ChaosNpcEntity.IsAlive())
                {
                    aliveBotCount += 1;
                    if (botController.realGameAge > botController.maxLifeExpectancy)
                    {
                        botController.entity.SleepMe();
                    }
                }else{
                    if (botController.realGameAge > botController.maxLifeExpectancy)
                    {
                        botController.entity.SleepMe();

                    }
                }*/
               
            }
           
        }
        // GameManager.instance.level.CleanUpBotList();

    
        if (spawnCountThisGeneration > speciesManager.realMaxSpawnCount)
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
            SpawnNextBots();
        }
          
    }

    public void IterateGenerations()
    {
       
        //Debug.Log("Iterating Generations: " + trainBasicGameModeSaveStateData.generation + " - " + speciesManager.spawnCountThisGen);
        if (globalLeaderBoard == null)
        {
            globalLeaderBoard = new FitnessSortingBlock();
        }



        float totalScore = 0;
        float medianScoreRoundTo = GameManager.instance.trainingRoomData.fitnessManagerConfigData.medianScoreRoundTo;
        IDictionary<int, int> median = new Dictionary<int, int>();
        GenerationScoreResultData generationScoreResultData = new GenerationScoreResultData();
        generationScoreResultData.generation = trainBasicGameModeSaveStateData.generation;
        /*
        foreach (SpeciesObject speciesObject in speciesManager.species.Values)
        {
            foreach (float lastScore in speciesObject.botScores.Values)
            {

                totalScore += lastScore;
                int medianScore = (int)Mathf.Round(Mathf.Round(lastScore / medianScoreRoundTo) * medianScoreRoundTo);
                if (!median.ContainsKey(medianScore))
                {
                    median.Add(medianScore, 0);
                }
                median[medianScore] += 1;

                if (
                    lastScore > generationScoreResultData.preCull_max
                ){
                    generationScoreResultData.preCull_max = lastScore;
                }
                if (
                    generationScoreResultData.preCull_min.Equals(0) ||
                    lastScore < generationScoreResultData.preCull_min
                )
                {
                    generationScoreResultData.preCull_min = lastScore;
                }


            }
        }*/
       
        //Debug.Log("Median Score:" + highestScoreIndex);
  
        trainBasicGameModeSaveStateData.generationScores.Add(generationScoreResultData);
        speciesManager.GenerationCheck();


        foreach (SpeciesObject speciesObject in speciesManager.species.Values)
        {
            if (speciesObject.lastScoreResult != null)
            {
                float lastScore = speciesObject.LastSortableScore();
                totalScore += lastScore;
                int medianScore = (int)Mathf.Round(Mathf.Round(lastScore / medianScoreRoundTo) * medianScoreRoundTo);
                if (!median.ContainsKey(medianScore))
                {
                    median.Add(medianScore, 0);
                }


                median[medianScore] += 1;



                if (
                    speciesObject.lastScoreResult.preCull_max > generationScoreResultData.preCull_max
                )
                {
                    generationScoreResultData.preCull_max = speciesObject.lastScoreResult.preCull_max;
                }

                if (
                    lastScore > generationScoreResultData.max
                )
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
                if (
                    generationScoreResultData.min.Equals(0) ||
                    speciesObject.lastScoreResult.preCull_min < generationScoreResultData.preCull_min
                )
                {
                    generationScoreResultData.preCull_min = speciesObject.lastScoreResult.preCull_min;
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
        generationScoreResultData.median = highestScoreIndex;
        generationScoreResultData.mean = totalScore / spawnCountThisGeneration;

        //Reshuffle species mutation rates
        speciesManager.AdjustSpeciesMutationRate(generationScoreResultData);
       
        if (currentGenerationPerformance != null)
        {
            currentGenerationPerformance.generation = trainBasicGameModeSaveStateData.generation;

            currentGenerationPerformance.meanNNetProcessingTime = currentGenerationPerformance.totalNNetTime / spawnCountThisGeneration;
            int highestNNetDurationCount = 0;
            float highestNNetDurationIndex = 0;
            foreach (float key in currentGenerationPerformance.medianNNetHolder.Keys)
            {
                if (currentGenerationPerformance.medianNNetHolder[key] > highestNNetDurationCount)
                {
                    highestNNetDurationIndex = key;
                    highestNNetDurationCount = currentGenerationPerformance.medianNNetHolder[key];
                }
            }
            currentGenerationPerformance.medianNNetProcessingTime = highestNNetDurationIndex;
            currentGenerationPerformance = new GenerationPerformanceData();
           
        }

        spawnCountThisGeneration = 0;
        /*GameManager.instance.botManager.CleanUpLongTermBotList();
        WorldEvent.CleanAll();*/
        trainBasicGameModeSaveStateData.generation += 1;

      

        onGenerationChange.Invoke();
        Resources.UnloadUnusedAssets();

    }

    public TrainBasicGameModeSaveStateData SaveStateData(string saveStateName)
    {
        string saveStateDir = GameManager.instance.trainingRoomData.fileLoc + "/save_states/" + saveStateName;
        //TrainingRoomSaveStateData trainingRoomSaveStateData = new TrainingRoomSaveStateData();
        trainBasicGameModeSaveStateData.fileLoc = saveStateDir;


        foreach (SpeciesObject speciesObject in speciesManager.species.Values)
        {
            SpeciesData speciesData = speciesObject.ToData();
            trainBasicGameModeSaveStateData.species.Add(speciesData);
        }

        trainBasicGameModeSaveStateData.Save();
        return trainBasicGameModeSaveStateData;
    }
    public void LoadStateData(TrainBasicGameModeSaveStateData _trainBasicGameModeSaveStateData)
    {
        GameManager.instance.level.CleanUp();
        GameManager.instance.level.InitLevel();
        trainBasicGameModeSaveStateData = _trainBasicGameModeSaveStateData;

        foreach (SpeciesData speciesData in trainBasicGameModeSaveStateData.species)
        {
            SpeciesObject speciesObject = new SpeciesObject();
            speciesObject.Init(speciesData.id, speciesManager);
            speciesManager.species.Add(speciesObject.id, speciesObject);
            speciesObject.ParseData(speciesData);

        }



        //TODO: Load scores

        //TODO: Trigger some type of onLoad event

    }
    public override void OnInitNPC(NPCControllerBase npcnNetController)
    {
        //Set its fitness controller

        /*npcnNetController.maxLifeExpectancy = trainingRoomData.fitnessManagerConfigData.initialMaxLifeExpectancy;
        npcnNetController.botFitnessController = fitnessManagerConfigData.GetNewBotFitnessController(npcnNetController);
        if(!GameManager.instance.trainingRoomData.brainMakerConfigData.backPropigationRate.Equals(0)){
            npcnNetController.botFitnessController.onScoreEvent.AddListener(BackPropigate);
        }*/
    }
    void BackPropigate(BotFitnessController.BotFitnessScoreEvent scoreEvent, NPCNNetController npcnNetController){
        if(scoreEvent.scoreEffect.Equals(0) || Mathf.Abs(scoreEvent.scoreEffect) < 100){//TODO: Change this number some how
            return;
        }
     

        //Iterate through each output neuron
        foreach(OutputNeuron outputNeuron in npcnNetController.nNet.outputNeurons.Values){
          
            //Move scores whos Math.Abs(last weight) > 0 back twards 0
            foreach(NeuronDep neuronDep in outputNeuron.dependencies){
                if (neuronDep.enabled &&  !neuronDep.lastValue.Equals(0))
                {
                    bool matchesOutput = false;

                    if(
                        (neuronDep.lastValue > 0 && outputNeuron.lastValue > 0) || 
                        (neuronDep.lastValue < 0 && outputNeuron.lastValue < 0)
                    ){
                        matchesOutput = true;
                    }

                  
                    bool reward = false;
                    if(matchesOutput){
                        if (scoreEvent.scoreEffect > 0) {
                            //Reward
                            reward = true;
                        }else{
                            //Penalise 
                            reward = false;
                        }
                    }else{
                        if (scoreEvent.scoreEffect > 0)
                        {
                            //Penalise 
                            reward = false;
                        }
                        else
                        {
                            //Reward
                            reward = true;
                        }
                    }
                    decimal neuronWeightAdjustment = 0;
                    if (reward) {
                        if(neuronDep.weight > 0){
                            neuronWeightAdjustment = GameManager.instance.trainingRoomData.brainMakerConfigData.backPropigationRate;
                        }else{
                            neuronWeightAdjustment = GameManager.instance.trainingRoomData.brainMakerConfigData.backPropigationRate * -1;
                        }
                    }else{
                        if (neuronDep.weight > 0)
                        {
                            neuronWeightAdjustment = GameManager.instance.trainingRoomData.brainMakerConfigData.backPropigationRate * -1;
                        }
                        else
                        {
                            neuronWeightAdjustment = GameManager.instance.trainingRoomData.brainMakerConfigData.backPropigationRate;
                        }
                    }
                    //If matches penalize it

                    //If it doesnt match then reward it?

                 

                    neuronDep.weight = neuronDep.weight + neuronWeightAdjustment;
                }
            }
        }


       


    }
    public override void OnDestroyBot(NPCControllerBase controller)
    {
        if ((controller is NPCNNetController))
        {
            return;
        }

        NPCNNetController npcnNetController = (NPCNNetController) controller;
        if (
            fitnessManagerConfigData.forceNewFitnessSortingBlockEachGeneration &&
            npcnNetController.speciesObject != null &&
            npcnNetController.speciesObject.speciesAge != npcnNetController.nNet.generation &&
            !(
                npcnNetController.nNet.generation == -1 &&
                npcnNetController.speciesObject.speciesAge == 0
            )


        ){
            Debug.Log("Generation SpeciesAge Mixup");
        }



        npcnNetController.speciesObject.LogScore(npcnNetController, npcnNetController.botFitnessController.cumulativeScore);

        if(currentGenerationPerformance != null){
            if(
                npcnNetController.avgNNetEvaluateDuration > currentGenerationPerformance.maxNNetProcessingTime ||
                currentGenerationPerformance.maxNNetProcessingTime.Equals(0)
            ){
                currentGenerationPerformance.maxNNetProcessingTime = npcnNetController.avgNNetEvaluateDuration;
            }
            if (
                npcnNetController.avgNNetEvaluateDuration < currentGenerationPerformance.minNNetProcessingTime ||
                currentGenerationPerformance.minNNetProcessingTime.Equals(0)
            )
            {
                currentGenerationPerformance.minNNetProcessingTime = npcnNetController.avgNNetEvaluateDuration;
            }
            currentGenerationPerformance.totalNNetTime += npcnNetController.avgNNetEvaluateDuration;
            float medianRoundedScore = Mathf.Round(npcnNetController.avgNNetEvaluateDuration * GenerationPerformanceData.medianRoundTo) / GenerationPerformanceData.medianRoundTo; 
            if(!currentGenerationPerformance.medianNNetHolder.ContainsKey(medianRoundedScore)){
                currentGenerationPerformance.medianNNetHolder.Add(medianRoundedScore, 0);
            }
            currentGenerationPerformance.medianNNetHolder[medianRoundedScore] += 1;
        }


        bool madeItInTop = npcnNetController.speciesObject.fitnessSortingBlock.TestBotStat(npcnNetController, npcnNetController.botFitnessController.cumulativeScore);
        if (!madeItInTop)
        {
            npcnNetController.MarkReadyForCleanup();
        }


    }
	public override void CheckInputs()
	{
        base.CheckInputs();

       
	}


    public void SpawnNextBots()
    {

        /*if (
            lastSpawnSeconds.Equals(0) ||
            (GameManager.instance.realTimeSeconds - lastSpawnSeconds) > GameManager.instance.gameConfigData.spawnThroddle
        )
        {
            speciesManager.SpawnBot();
            lastSpawnSeconds = GameManager.instance.realTimeSeconds;

        }*/
    }

    public NPCNNetController SpawnNPC(NPCNNetController parentNpcnNetController)
    {
        return SpawnNPC(parentNpcnNetController, true);
    }
    public NPCNNetController SpawnNPC(NPCNNetController parentNpcnNetController, bool spawnAsParent)
    {
        NPCNNetController npcnNetController = null;

        if (parentNpcnNetController != null)
        {
            npcnNetController = (NPCNNetController)GameManager.instance.level.InitNewBot(parentNpcnNetController, spawnAsParent);
            if (
                spawnAsParent &&
                parentNpcnNetController.nNet.generation + 1 != npcnNetController.nNet.generation
            )
            {
                throw new System.Exception("Parent Child Bot Mismatch");
            }
        }
        else
        {
            foreach (NPCNNetController _botController in GameManager.instance.level.bots.Values)
            {
                if (!_botController.HasAttachedEntity())
                {
                    npcnNetController = _botController;
                }
            }
            if (npcnNetController == null)
            {
                npcnNetController = (NPCNNetController)GameManager.instance.level.InitNewBot(null);
            }
        }
      
        if (npcnNetController.entity != null)
        {
            npcnNetController.DetachEntity();
            //Destroy(botController.entity);
            //botController.entity.gameObject.SetActive(false);

            //botController.entity = null;
            //TODO: Detatch
        }

        int index = 0;
        float newAngle = 90;
        /*if (fitnessManagerConfigData.randomizeSpawnPosition)
        {
            index = Random.Range(0, GameManager.instance.boardManager.spawnTiles.Count);
            newAngle = Random.Range(0, 360);//(Random.Range(0, 100) * .1f) * (Mathf.PI * 2);

        }
        SpawnTile spawnTile = GameManager.instance.boardManager.spawnTiles[index];*/


        GameObject botEntity = GameManager.instance.PrefabManager.Get("ChaosNPCEntity");
        // spawnTile.gameObject.transform.position, Quaternion.Euler(0, 0, newAngle)
        botEntity.name = npcnNetController.id;

        ChaosNPCEntity entity = (ChaosNPCEntity)botEntity.GetComponent<ChaosNPCEntity>();
        //Debug.Log("Attaching Entity: " + botEntity.name);
        entity.id = botEntity.name;
        npcnNetController.Attach(entity);

        BoxCollider2D _collider = entity.GetComponent<BoxCollider2D>();
        foreach (ChaosEntity entityObject in GameManager.instance.level.entities)
        {
            if (
                entityObject != null &&
                entityObject.gameObject != null &&
                //entityObject.isAlive() && 
                entityObject.GetType().Name == "ChaosNPCEntity"
            )
            {
                
                Physics2D.IgnoreCollision(
                    _collider,
                   entityObject.GetComponent<BoxCollider2D>()
                );
            }
        }
        if (
            !spawnAsParent &&
            npcnNetController.nNet.generation != npcnNetController.speciesObject.speciesAge//GameManager.instance.fitnessManager.currentGeneration
        )
        {
            Debug.LogError("BotSpawnGenCheckFail: " + npcnNetController.speciesObject.id + "=" + npcnNetController.nNet.generation);
        }
        // GameManager.instance.menuManager.botListPanel.botListScrollView.AddGameObject(npcnNetController);
        spawnCountThisGeneration += 1;
        return npcnNetController;


    }
}
