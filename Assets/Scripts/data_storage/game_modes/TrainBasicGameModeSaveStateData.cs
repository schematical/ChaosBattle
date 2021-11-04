using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    

[System.Serializable]
public class TrainBasicGameModeSaveStateData: ICTSerializedData  {


    public class FitnessRule{
        public int score = 1;
        public string stat;

    }

    public string fileLoc;
    public string name;
    public List<SpeciesData> species = new List<SpeciesData>();
    public List<GenerationScoreResultData> generationScores = new List<GenerationScoreResultData>();
    public int generation;

    //TODO: Move game time to here :)

    public TrainBasicGameModeSaveStateData(){
        
    }

    public static TrainBasicGameModeSaveStateData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TrainBasicGameModeSaveStateData>(jsonString);
    }
    public static TrainBasicGameModeSaveStateData LoadFromLocal(string gameDataFileName){
        string filePath = gameDataFileName + "/saveState.json";

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot load game data: " + filePath);
            return null;
        }
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(filePath);
        TrainBasicGameModeSaveStateData trainingRoomData =  CreateFromJSON(dataAsJson);
        //trainingRoomData.fileLoc = filePath;
        return trainingRoomData;

    }

    public void Save()
    {

        string dataAsJson = JsonUtility.ToJson(this);

        System.IO.Directory.CreateDirectory(this.fileLoc);
        File.WriteAllText(this.fileLoc + "/saveState.json", dataAsJson);

    }
    /*
    public List<string> GetSaveStates(){
        string dirPath = fileLoc + "/save_states";
        System.IO.Directory.CreateDirectory(dirPath);
        DirectoryInfo dir = new DirectoryInfo(dirPath);

        FileInfo[] info = dir.GetFiles("*.*");
        List<string> saveStates = new List<string>();
        foreach (FileInfo f in info)
        {
            saveStates.Add(f.Name);//f.FullName);
        }
        return saveStates;
    }*/

}