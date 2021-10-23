using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChallengeBasicGameMode : GameModeBase, IGameModeWithFitnessConfig, IGameModeWithSpawnables
{
    public int _maxBotCount = 5;
    public FitnessManagerConfigData _fitnessManagerConfigData = null;
    public int maxIndividualBotSpawnCount = 50;//TODO: Move to savable config?
   
    public decimal lastSpawnSeconds = 0;
    public List<NPCNNetController> botsInPlay = new List<NPCNNetController>();

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
            return GameManager.GameModeType.CHALLENGE_BASIC;
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

        GameManager.instance.botManager.brainMaker = new CTBrainMaker();
        //GameManager.instance.botManager.brainMaker.Init(trainingRoomData.brainMakerConfigData, this);
        GameManager.instance.level.CleanUp();


    }
    public void StartChallenge(){

/*

        GameManager.instance.boardManager.SetupScene(trainingRoomData);


        GameManager.instance.menuManager.HideAllMenues();
        GameManager.instance.menuManager.botListPanel.destroyOnDeath = false;
        GameManager.instance.menuManager.botListPanel.Show();
        GameManager.instance.menuManager.menuBarPanel.Show();
        GameManager.instance.menuManager.footerPanel.Show();
        GameManager.instance.inputManager.ListenToKeyboardInput();*/

        GameManager.instance.cameraManager.paningEnabled = true;
    }
    public override void Tick(){
        if(GameManager.instance.paused){
            return;
        }
     
        //debugManager.ClearDebugLines();
      
        TickFitness();


        GameManager.instance.botManager.Tick();
        WorldEvent.CleanTick();
    }
    public override void Suspend(){
        
    }
    public override void Shutdown(){
      
    }
    public void TickFitness(){
        
   
        int aliveBotCount = 0;
        //bool hasLastGenBot = false;
        foreach (NPCNNetController botController in GameManager.instance.botManager.bots.Values)
        {
            bool hasAttachedEntity = botController.HasAttachedEntity();
         
            if(
                hasAttachedEntity && 
                botController.entity.isAlive()
            )
            {
                aliveBotCount += 1;
                if (botController.realGameAge > botController.maxLifeExpectancy)
                {
                    botController.entity.SleepMe();
                    //Debug.Log("BotController: " + botController.id + " - Death by timeout");
                }
               
            }
            /*if(hasAttachedEntity != isAlive){
                Debug.LogError("Confict..." + botController.id);
            }*/
        }
        GameManager.instance.botManager.CleanUpBotList();



        if (aliveBotCount < maxBotCount)
        {
            if(lastSpawnSeconds > GameManager.instance.gameConfigData.spawnThroddle){

                lastSpawnSeconds = 0;
                NPCNNetController npcnNetController = null;
                int botIndex = 0;
                int saftyCount = 0;
                while(
                    npcnNetController == null &&
                    saftyCount < 20
                ){
                    saftyCount += 1;
                    botIndex = Random.Range(0, botsInPlay.Count);
                    if (
                        (
                            !botsInPlay[botIndex].HasAttachedEntity() ||
                            !botsInPlay[botIndex].entity.isAlive()
                        ) &&
                        botsInPlay[botIndex].spawnCount < botsInPlay[botIndex].maxSpawnCount
                    
                    ){
                        npcnNetController = botsInPlay[botIndex];
                    }
                }
                if (npcnNetController != null)
                {
                    SpawnBot(npcnNetController, false);
                }
            }else{
                lastSpawnSeconds += (decimal)Time.fixedDeltaTime;
            }
        }
        bool hasMoreBotsToRun = false;
        foreach(NPCNNetController botController in botsInPlay){
            if(botController.spawnCount < botController.maxSpawnCount){
                hasMoreBotsToRun = true;
            }
        }
        if(!hasMoreBotsToRun){
            GameManager.instance.menuManager.challengeModeStatsPanel.Show();
            GameManager.instance.Pause();
        }
          
    }
   
    public override void OnInitNPC(NPCNNetController npcnNetController)
    {
        //Set its fitness controller
        npcnNetController.keepAround = true;
        npcnNetController.memory = new BotMemory();
        npcnNetController.maxSpawnCount = maxIndividualBotSpawnCount;
        npcnNetController.ResetAge();
        npcnNetController.maxLifeExpectancy = trainingRoomData.fitnessManagerConfigData.initialMaxLifeExpectancy;
        npcnNetController.botFitnessController = fitnessManagerConfigData.GetNewBotFitnessController(npcnNetController);
    }
    public override void OnDestroyBot(NPCNNetController npcnNetController)
    {
        
        if(npcnNetController.spawnCount < npcnNetController.maxSpawnCount){
            SpawnBot(npcnNetController, false);
            return;
        }

        npcnNetController.MarkReadyForCleanup();

    }
	public override void CheckInputs()
	{
        base.CheckInputs();


	}



    public NPCNNetController SpawnBot(NPCNNetController parentNpcnNetController)
    {
        return SpawnBot(parentNpcnNetController, true);
    }
    public NPCNNetController SpawnBot(NPCNNetController parentNpcnNetController, bool spawnAsParent)
    {
        NPCNNetController npcnNetController = null;

        if (parentNpcnNetController != null)
        {
            npcnNetController = GameManager.instance.botManager.InitNewBot(parentNpcnNetController, spawnAsParent);
        }
        else
        {
            foreach (NPCNNetController _botController in GameManager.instance.botManager.bots.Values)
            {
                if (!_botController.HasAttachedEntity())
                {
                    npcnNetController = _botController;
                }
            }
            if (npcnNetController == null)
            {
                npcnNetController = GameManager.instance.botManager.InitNewBot(null);
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
        if (fitnessManagerConfigData.randomizeSpawnPosition)
        {
            index = Random.Range(0, GameManager.instance.boardManager.spawnTiles.Count);
            newAngle = Random.Range(0, 360);//(Random.Range(0, 100) * .1f) * (Mathf.PI * 2);

        }
        SpawnTile spawnTile = GameManager.instance.boardManager.spawnTiles[index];


        GameObject botEntity = GameManager.instance.boardManager.AddEntity("Bot", spawnTile.gameObject.transform.position, Quaternion.Euler(0, 0, newAngle));
        botEntity.name = npcnNetController.id;

        Player entity = (Player)botEntity.GetComponent<EntityObject>();
        //Debug.Log("Attaching Entity: " + botEntity.name);
        entity.id = botEntity.name;
        npcnNetController.Attach(entity);

        BoxCollider2D _collider = entity.GetComponent<BoxCollider2D>();
        foreach (EntityObject entityObject in GameManager.instance.boardManager.entities.Values)
        {
            if (
                entityObject != null &&
                entityObject.gameObject != null &&
                //entityObject.isAlive() && 
                entityObject.GetType().Name == "Player"
            )
            {
                Physics2D.IgnoreCollision(
                    _collider,
                   entityObject.GetComponent<BoxCollider2D>()
                );
            }
        }
        if(npcnNetController.spawnCount == 0){
            GameManager.instance.menuManager.botListPanel.botListScrollView.AddGameObject(npcnNetController);
        }
       
        return npcnNetController;


    }

    /*
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
       GameManager.instance.DestroyEverything();
       trainBasicGameModeSaveStateData = _trainBasicGameModeSaveStateData;

       foreach (SpeciesData speciesData in trainBasicGameModeSaveStateData.species)
       {
           SpeciesObject speciesObject = ScriptableObject.CreateInstance<SpeciesObject>();
           speciesObject.Init(speciesData.id, speciesManager);
           speciesManager.species.Add(speciesObject.id, speciesObject);
           speciesObject.ParseData(speciesData);

       }



       //TODO: Load scores

       //TODO: Trigger some type of onLoad event

   }
   */
}
