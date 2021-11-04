using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    

[System.Serializable]
public class ChaosTrainerData: BaseData  {


    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [System.Serializable]
    public class TrainingRoomBasicData
    {
        public string name;
        public string fileLoc;
        public string trainingRoomNamespace;

        //Assignment constructor.
        public TrainingRoomBasicData(string _trainingRoomNamespace)
        {
            trainingRoomNamespace = _trainingRoomNamespace;
        }
    }

    public string saveFileLoc;
    public string name;
    public GameConfigData gameConfigData = new GameConfigData();
    public List<TrainingRoomBasicData> trainingRooms = new List<TrainingRoomBasicData>();

    public ChaosTrainerData():base(){
        
    }
    public override string _class_name
    {
        get
        {
            return "BotControllerScoreData";
        }
    }

    public static ChaosTrainerData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ChaosTrainerData>(jsonString);
    }
    public static ChaosTrainerData LoadFromLocal(string gameDataFileName){
       
        if (!File.Exists(gameDataFileName))
        {
            Debug.LogError("Cannot load game data: " + gameDataFileName);
            return null;
            // Read the json from the file into a string
          
        }
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(gameDataFileName);
        ChaosTrainerData chaosTrainerData =  CreateFromJSON(dataAsJson);
        chaosTrainerData.saveFileLoc = gameDataFileName;
        return chaosTrainerData;

    }

    public void Save()
    {
     
        string dataAsJson = JsonUtility.ToJson(this);

        File.WriteAllText(this.saveFileLoc, dataAsJson);

    }

    /*
    public List<TrainingRoomData> LoadTrainingRooms(){
        List<TrainingRoomData> trainingRoomDatas = new List<TrainingRoomData>();
        for (int i = 0; i < trainingRooms.Count; i++){
            TrainingRoomData trainingRoomData = TrainingRoomData.LoadFromLocal(trainingRooms[i].fileLoc);
            trainingRoomDatas.Add(trainingRoomData);
        }
        return trainingRoomDatas;
    }
    */

    public TrainingRoomData CreateTraingingRoom(string trainingRoomNamespace, string name){
        
        TrainingRoomData trainingRoomData = new TrainingRoomData();
        TrainingRoomBasicData trainingRoomBasicData = new TrainingRoomBasicData(trainingRoomNamespace);
        string dirName = Application.persistentDataPath + "/save_data/" + trainingRoomNamespace;
        System.IO.Directory.CreateDirectory(dirName);
        trainingRoomData.fileLoc = dirName;
        System.IO.Directory.CreateDirectory(dirName);
        trainingRoomBasicData.fileLoc = trainingRoomData.fileLoc;
        trainingRoomData.name = name;
        trainingRoomBasicData.name = name;
        trainingRooms.Add(trainingRoomBasicData);
        Save();
        return trainingRoomData;
    }
    public void FancyImportTrainingRooms(){
        GameManager.instance.menuManager.debugPanel.Log("FancyImportTrainingRooms Started: " + Application.dataPath);
        DirectoryInfo parentDir = Directory.GetParent(Application.dataPath);
        while (parentDir != null)
        {
            DirectoryInfo[] saveDataDir = parentDir.GetDirectories("save_data");
            if (saveDataDir.Length > 0)
            {
                DirectoryInfo[] directories = saveDataDir[0].GetDirectories();
                foreach (DirectoryInfo dirInfo in directories)
                {

                    FileInfo[] info = dirInfo.GetFiles("trainingRoom.json");

                    foreach (FileInfo f in info)
                    {
                        bool alreadyHasImported = false;
                        foreach (TrainingRoomBasicData trainingRoomData in trainingRooms)
                        {
                            if (trainingRoomData.trainingRoomNamespace == dirInfo.Name)
                            {
                                alreadyHasImported = true;
                            }
                        }
                        if (!alreadyHasImported)
                        {
                            GameManager.instance.menuManager.debugPanel.Log("Importing: " + dirInfo.Name);
                            TrainingRoomBasicData trainingRoomBasicData = new TrainingRoomBasicData(dirInfo.Name);
                            trainingRoomBasicData.fileLoc = dirInfo.FullName;
                            trainingRoomBasicData.name = dirInfo.Name;
                            trainingRooms.Add(trainingRoomBasicData);
                        }
                    }
                }
            }
            parentDir = parentDir.Parent;
        }

    }

   
}