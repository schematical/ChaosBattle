using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    

[System.Serializable]
public class SpeciesData: ICTSerializedData  {

 

    public string fileLoc;

    public string id;
    public int startAge;
    public int speciesAge = 0;
    public int generationsSinceLastImprovment;
    public float currentHighSortableScore;
    public GenerationScoreResultData lastScoreResult;
    public FitnessSortingBlockData fitnessSortingBlock;// = new FitnessSortingBlockData();
    public FitnessSortingBlockData lastGenFitnessSortingBlock = null;//new FitnessSortingBlock();
    public BotControllerData firstBotController;
    public List<BotControllerData> botControllers = new List<BotControllerData>();
    public List<GenerationScoreResultData> historicalScoreInfo = new List<GenerationScoreResultData>();



    //TODO Store bot scores?

    public SpeciesData(){
        
    }

    public static TrainingRoomData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TrainingRoomData>(jsonString);
    }
    public static TrainingRoomData LoadFromLocal(string gameDataFileName){
        string filePath = gameDataFileName + "/species.json";

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot load game data: " + filePath);
            return null;
        }
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(filePath);
        TrainingRoomData trainingRoomData =  CreateFromJSON(dataAsJson);
        //trainingRoomData.fileLoc = filePath;
        return trainingRoomData;

    }

    public void Save()
    {

        string dataAsJson = JsonUtility.ToJson(this);

        if (this.fileLoc == null)
        {
            fileLoc = Application.persistentDataPath + "/save_data/species_data/" + id;
        }
        System.IO.Directory.CreateDirectory(fileLoc);
      
        File.WriteAllText(fileLoc+ "/species.json", dataAsJson);

    }
    public List<string> GetBotControllers(){
        string dirPath = fileLoc + "/bot_controllers";
        System.IO.Directory.CreateDirectory(fileLoc);
        DirectoryInfo dir = new DirectoryInfo(dirPath);
    
        FileInfo[] info = dir.GetFiles("*.*");
        List<string> saveStates = new List<string>();
        foreach (FileInfo f in info)
        {
            saveStates.Add(f.Name);//f.FullName);
        }
        return saveStates;
    }

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.
}