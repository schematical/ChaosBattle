using System;
using System.Collections;
using System.Collections.Generic;
using services.Seed;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    
    public const string V = "0.0.17"; 
    public static class GameModeType{
        public const string PAUSED = "PAUSED";
        public const string EDIT_TRAINING_ROOM_BASIC = "EDIT_TRAINING_ROOM_BASIC";
        public const string TRAIN_BASIC = "TRAIN_BASIC";
        public const string CHALLENGE_BASIC = "CHALLENGE_BASIC";
    }
    
    public static GameManager instance;
    public GarbageCollector garbageCollector = new GarbageCollector();
    public PrefabManager PrefabManager;
    public Camera Camera;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public RuleTile floorTile;
    public ChaosLevel level = new ChaosLevel();

    public ChaosSeed ChaosSeed;
    public MenuManager menuManager;

    public CameraManager cameraManager;

    public GameModeBase _gameMode;
    public GameConfigData gameConfigData;//TODO: Load this from the main config file

    public ChaosTrainerData chaosTrainerData;

    private bool _paused = false;
    public TrainingRoomData trainingRoomData;
    public InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        ChaosSeed = new ChaosSeed(DateTime.Now.ToString());//"x");
        PrefabManager.Init();
        inputManager = new InputManager();
        GameManager.instance.SetGameMode(new TrainBasicGameMode());
        GameManager.instance.Resume();
        
        level.InitLevel();
    }

    // Update is called once per frame
    void Update()
    {

        // level.Tick();
    }
    public GameModeBase gameMode{
        get{
            return _gameMode;
        }
    }
    public bool paused
    {
        get
        {
            return _paused;
        }
    }
    public void Pause()
    {

        _paused = true;
    }
    public void Resume()
    {

        _paused = false;
    }
    public void SetTrainingRoom(ChaosTrainerData.TrainingRoomBasicData trainingRoomBasicData){
        trainingRoomData = TrainingRoomData.LoadFromLocal(trainingRoomBasicData.fileLoc);

    }
    void InitGame()
    {
        // menuManager.debugPanel.Log("v" + GameManager.V);
       
        string saveDir = Application.persistentDataPath + "/save_data";
        string saveFileLoc = saveDir +"/chaos_trainer.json";
        // menuManager.debugPanel.Log("File Path: " + saveFileLoc);
        Debug.Log("Atempting to load chaosTrainer.json: " + saveFileLoc);

       
        chaosTrainerData = ChaosTrainerData.LoadFromLocal(saveFileLoc);

       



        if(chaosTrainerData == null){

            Debug.Log("`chaosTrainer.json` not found. Attemping to create save dir: " + saveDir);
            try
            {
                System.IO.Directory.CreateDirectory(saveDir);
            
                chaosTrainerData = new ChaosTrainerData();
                chaosTrainerData.saveFileLoc = saveFileLoc;
                chaosTrainerData.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error creating directory: " + e.Message + " _______ \n" + e.StackTrace.ToString() + "_______________\n\n");
                return;
            }
            Debug.Log("Creating New chaos_trainer.json");
        }else{
            Debug.Log("Successfully Loaded Existing: chaos_trainer.json");
        }
        chaosTrainerData.FancyImportTrainingRooms();
        chaosTrainerData.Save();
        gameConfigData = chaosTrainerData.gameConfigData;


    }
    public void SetGameMode(GameModeBase gameMode){
        if(_gameMode != null){
            _gameMode.Shutdown();
        }
        _gameMode = gameMode;
        _gameMode.Setup();
    }
    void FixedUpdate()
    {
        if(paused){
            return;
        }
        
        if (gameMode != null)
        {
            gameMode.Tick();
        }
                   
          
       
        
    }
}
