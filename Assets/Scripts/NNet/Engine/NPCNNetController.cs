using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ActionCandidate
{
    public Dictionary<Type, float> TypeScores = new Dictionary<Type, float>();
    public ChaosEntity Entity { get; set; }
}
public class NPCNNetController : NPCControllerBase {
    public float totalNNetEvaluateMS = 0;
    public NNet nNet;
    public BotBiologyData botBiology;
    public int failedToMoveCount = 0;
    public SpeciesObject speciesObject;
    public List<List<Vector2>> debugPositionTracking = new List<List<Vector2>>();
    public int ticksSinceLastDebug = 0;
    public BotControllerScoreData botControllerScoreData = new BotControllerScoreData();
    public BotFitnessController botFitnessController;
    public int spawnCount = 0;
    public int maxSpawnCount = 1;
    public int maxLifeExpectancy;

    protected int _ageTicks = 0;
    public decimal secondsSinceLastTick = -1;
    public bool _botBiologyInited = false;
    public float totalTickDurationMs = 0;
   
    public IList<WorldEvent> tickEvents = new List<WorldEvent>();
    private bool _readyForCleanup = false;
    public bool keepAround = false;
    public DateTime birthDate;
    public decimal _realGameAge = 0;
    public float secondsSinceLastPing = 0;
    public BrainMaker.Action brainMakerAction;
    public Dictionary<ChaosEntity, ActionCandidate> ActionCandidates = new Dictionary<ChaosEntity, ActionCandidate>();
    public ActionCandidate CurrentActionCandidate;


   

    private void SelectAnAction()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        ActionCandidates.Clear();
        GameManager.instance.level.entities.ForEach((entity) =>
        {
            CurrentActionCandidate = new ActionCandidate();
            ActionCandidates.Add(entity, CurrentActionCandidate);
            CurrentActionCandidate.Entity = entity;
            List<OutputNeuron> firingOutputs = nNet.EvaluateNeurons();
            foreach(OutputNeuron outputNeuron in firingOutputs){
                outputNeuron.Execute(
                    outputNeuron.lastValue
                );
            }
        });
        float highestScore = -99999;
        /*Type  highestScoringActionType = null;
        ChaosEntity highestScoringEntity = null;*/
        BaseAction highestScoringAction = null;
        foreach (KeyValuePair<ChaosEntity,ActionCandidate> keyValuePair in ActionCandidates)
        {
            foreach (KeyValuePair<Type,float> valueTypeScore in keyValuePair.Value.TypeScores)
            {
                float score = valueTypeScore.Value;
                if (score > highestScore)
                {
                    
                    switch (valueTypeScore.Key.Name)
                    {
                        case("NavigateToAction"): 
                            NavigateToAction navigateToAction = new NavigateToAction(entity);
                            navigateToAction.SetTarget(keyValuePair.Key);
                            if (navigateToAction.isValid())
                            {
                                highestScore = score;
                                highestScoringAction = navigateToAction;
                            }

                          
                            break;
                        case("UsePrimaryItemAction"): 
                            UsePrimaryItemAction usePrimaryItemAction = new UsePrimaryItemAction(entity);
                            usePrimaryItemAction.SetTarget(keyValuePair.Key);
                            if (usePrimaryItemAction.isValid())
                            { 
                                highestScore = score;
                                highestScoringAction = usePrimaryItemAction;
                            }
                         
                            break;
                        default:
                            throw new Exception("Not sure what to do with `ActionBase`: " + valueTypeScore.Key.Name);
                    }
                
                }
            }
        }

        if (highestScoringAction != null)
        {
            entity.SetCurrentAction(highestScoringAction);
        }
        else
        {
            Vector3Int targetVec = GameManager.instance.level.GetRandomFloorTileVec();
            NavigateToAction wonderAction = new NavigateToAction(ChaosNpcEntity);
            wonderAction.SetTargetVec(targetVec);
            ChaosNpcEntity.SetCurrentAction(wonderAction);
            // throw new Exception("Could not find a viable action");
        }
       

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;
        totalNNetEvaluateMS += (float)ts.TotalMilliseconds;
    }
    /*public override string _class_name
    {
        get
        {
            return "BotController";
        }
    }*/

    public ChaosNPCEntity entity{
        get{
            return this.ChaosNpcEntity;
        }
    }
    public bool readyForCleanup{
        get{
            return !keepAround && _readyForCleanup;
        }
    }
    public void MarkReadyForCleanup(){
        _readyForCleanup = true;
    }

    public float avgTickDuration{
        get
        {
            return totalTickDurationMs / _ageTicks;
        }
    }
    public int ageTicks
    {
        get { return _ageTicks; }
    }
    public decimal realGameAge
    {
        get { return _realGameAge; }
    }
    public float ageSeconds
    {
        get
        {
            return (DateTime.Now.Ticks - birthDate.Ticks) / (TimeSpan.TicksPerMillisecond * 1000);//_ageTicks * GameManager.instance.gameConfigData.secondsBetweenTicks;
        }
    }
    public float avgNNetEvaluateDuration{
        get{
            return totalNNetEvaluateMS / _ageTicks;
        }
    }

   
    public void ResetAge(){
        _ageTicks = 0;
        _realGameAge = 0;
        totalNNetEvaluateMS = 0;
    }

   

  
    void OnWorldEvent(WorldEvent worldEvent)
    {
        tickEvents.Add(worldEvent);

        TestFittnessEvent(worldEvent);
    }


    public void AttachNNet(NNet _nNet){
        nNet = _nNet;
        nNet.AttachBotController(this);

    }
    /*
    public void Reset(){
        
        maxLifeExpectancy = simDurationSeconds;
        _ageTicks = 0;
        for (int i = 0; i < tickEvents.Count; i++){
            //UnityEngine.Object.Destroy(tickEvents[i]);
            tickEvents[i] = null;
        }
        tickEvents.Clear();

    }
    public void MarkAsUnfit(){
        //Remove debug
    }
    public void MarkAsFit(){
        
    }*/
    public override void tick()
    {



        _realGameAge += (decimal)Time.fixedDeltaTime;
        if(!Time.fixedDeltaTime.Equals(.02f)){
            UnityEngine.Debug.LogError("Time Delta Error:" + Time.fixedDeltaTime);
        }
        if(!(_realGameAge % (decimal)Time.fixedDeltaTime).Equals(0)){
            UnityEngine.Debug.LogError("Time Delta Error 2:" + (_realGameAge % (decimal)Time.fixedDeltaTime));
        }
       
        secondsSinceLastPing += Time.fixedDeltaTime;
        if(secondsSinceLastPing >= GameManager.instance.gameConfigData.secondsSinceLastPing){
            secondsSinceLastPing = 0;
            /*WorldEvent worldEvent = new WorldEvent();
            worldEvent.Init(WorldEvent.WorldEventTypes.PING, entity);
            entity.onPing.Invoke(worldEvent);
            entity.onWorldEvent.Invoke(worldEvent);*/
        }

        secondsSinceLastTick += (decimal) Time.fixedDeltaTime;
        if (secondsSinceLastTick < GameManager.instance.gameConfigData.secondsBetweenTicks || secondsSinceLastTick.Equals(-1))
        {
            return;
        }


        secondsSinceLastTick = 0;


        if (spawnCount > 1)
        {
            //Debug.Log("Tick Attempt: " + spawnCount);
        }
        /*if(!isSpawned){
            return;
        }*/
        if(!entity.IsAlive()){
            entity.SleepMe();
            return;
        }
        _ageTicks += 1;


        BaseAction currAction = ChaosNpcEntity.GetCurrentAction();
        if (
            currAction == null || 
            currAction.isFinished()
        ){
            SelectAnAction();
        }



    }
    void DebugLogPosition(){
        List<Vector2> x = debugPositionTracking[spawnCount - 1];
        x.Add(entity.transform.position);
    }
    public void TestFittnessEvent(WorldEvent worldEvent){
        botFitnessController.Test(worldEvent);
    }
    void EntityDestroyEvent(WorldEvent worldEvent)
    {

        DebugLogPosition();

        isSpawned = false;

    

        botFitnessController.SubmitFinalScoreForLife();
        GameManager.instance.gameMode.OnDestroyBot(this);


        GameManager.instance.level.entities.Remove(entity);
        //entity.DetachMe();
    }
    public void InitSpawned(){
        spawnCount += 1;
        secondsSinceLastPing = UnityEngine.Random.Range(0, 100) * .01f;
        isSpawned = true;
       
        debugPositionTracking.Add(new List<Vector2>());

        /*entity.onDestroy.AddListener(EntityDestroyEvent);
        entity.onWorldEvent.AddListener(OnWorldEvent);
        botFitnessController.onScoreEvent.AddListener(entity.ScoreEvent);
        entity.onPing.AddListener(entity.PingEvent);
        if (GameManager.instance.trainingRoomData.brainMakerConfigData.botTileMemoryActive)
        {
            entity.onEnterTile.AddListener(memory.RememberTile);
        }*/
        
        DebugLogPosition();
        /*
        WorldEvent worldEvent = new WorldEvent();//ScriptableObject.CreateInstance<WorldEvent>();//new WorldEvent(WorldEvent.WorldEventTypes.I_SPAWNED, entity);
        worldEvent.Init(WorldEvent.WorldEventTypes.I_SPAWNED, entity);
        TestFittnessEvent(worldEvent);
*/


        InitBotBiology();
    }
    public void InitBotBiology(){
        if(_botBiologyInited){
            return;
        }
        // botEyeManager = new BotEyeManager(this);
        _botBiologyInited = true;

    }
    public BotControllerData ToData(){
        BotControllerData botControllerData = new BotControllerData();
        botControllerData.id = id;
        botControllerData.nNetData = nNet.GetSerializer();
        botControllerData.botBiology = botBiology;
        if (speciesObject != null)
        {
            botControllerData.speciesId = speciesObject.id;
        }
        botControllerData.botControllerScoreData = botControllerScoreData;
        return botControllerData;
    }
    public void ParseData(BotControllerData botControllerData){
        id = botControllerData.id;

       
        botBiology = botControllerData.botBiology;

        NNet _nNet = GameManager.instance.level.brainMaker.ParseNNetData(botControllerData.nNetData);
        AttachNNet(_nNet);
        //TODO: Fix this hackyness
        if(GameManager.instance.gameMode.type == GameManager.GameModeType.TRAIN_BASIC){
            TrainBasicGameMode gameMode = ((TrainBasicGameMode)GameManager.instance.gameMode);
            if (gameMode.speciesManager.species.ContainsKey(botControllerData.speciesId))
            {
                speciesObject = gameMode.speciesManager.species[botControllerData.speciesId];
            }else{
                UnityEngine.Debug.LogError("Could not find species: " + botControllerData.speciesId + " while importing BotController:" + botControllerData.id);
            }
        }
       
        botControllerScoreData = botControllerData.botControllerScoreData;
    }
	internal void SetSpecies(SpeciesObject _speciesObject)
	{
        speciesObject = _speciesObject;
	}



    //public override void OnDestroy()
    ~NPCNNetController()
    {
        //base.OnDestroy();
        //UnityEngine.Object.Destroy(botFitnessController);
        //UnityEngine.Object.Destroy(nNet);
        //UnityEngine.Object.Destroy(memory);
        nNet = null;
        botFitnessController = null;
        speciesObject = null;
        _chaosNpcEntity = null;
        
        //Data Objects
        botBiology = null;
        botControllerScoreData = null;
        botBiology = null;
       
        for (int i = 0; i < tickEvents.Count; i++)
        {
            tickEvents[i].keepAlive = false;
            //UnityEngine.Object.Destroy(tickEvents[i]);
            tickEvents[i] = null;
        }
        tickEvents.Clear();
        tickEvents = null;
        debugPositionTracking.Clear();
        debugPositionTracking = null;
        /*for (int i = 0; i < bioInputs.Count; i++){
            UnityEngine.Object.Destroy(bioInputs[i]);
        }
        bioInputs.Clear();
        bioInputs = null;*/

        brainMakerAction = null;

      
    }
	/*public override void Dispose()
	{
        //base.Dispose();
        botFitnessController.Dispose();
        //botControllerScoreData.Dispose();
        //nNet.Dispose();
        memory.Dispose();
	}*/
    public BreedAction.CompareResult CompareWithBot(NPCNNetController npcnNetController){
        BreedAction breedAction = new BreedAction();
        breedAction.AddParent(this);
        breedAction.AddParent(npcnNetController);
        breedAction.Populate();
        BreedAction.CompareResult compareResult = breedAction.Compare();
        //ScriptableObject.Destroy(breedAction);
        breedAction = null;
        return compareResult;

    }

 
}
