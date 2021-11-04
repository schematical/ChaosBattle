using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    

[System.Serializable]
public class BotControllerData: BaseData  {




    public string fileLoc;

    public string id;
    public NNetData nNetData;
    public BotBiologyData botBiology;
    public string speciesId;
    public BotControllerScoreData botControllerScoreData = new BotControllerScoreData();

 
    public override string _class_name
    {
        get
        {
            return "BotControllerData";
        }
    }
    public static string SaveDir(){
       return Application.persistentDataPath + "/save_data/bot_controllers/";
    }
  
    public BotControllerData(){
        
    }

    public static BotControllerData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<BotControllerData>(jsonString);
    }
    public static BotControllerData LoadFromLocal(string gameDataFileName){
        string filePath = gameDataFileName;//SaveDir() + gameDataFileName + ".json";

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot load game data: " + filePath);
            return null;
        }
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(filePath);
        BotControllerData botControllerData =  CreateFromJSON(dataAsJson);
        botControllerData.fileLoc = filePath;
        return botControllerData;

    }
    public static List<BotControllerData> LoadAll(){
        System.IO.Directory.CreateDirectory(SaveDir());
        DirectoryInfo dir = new DirectoryInfo(SaveDir());
        List<BotControllerData> botControllerDatas = new List<BotControllerData>();

        FileInfo[] info = dir.GetFiles("*.json");//GetDirectories();
        List<string> saveStates = new List<string>();
        foreach (FileInfo f in info)
        {
            botControllerDatas.Add(LoadFromLocal(f.FullName));
        }

        return botControllerDatas;
    }
    public void Save()
    {
        System.IO.Directory.CreateDirectory(SaveDir());
        string dataAsJson = JsonUtility.ToJson(this);
        Debug.Log("Trying to save: " + this.fileLoc);
        if (this.fileLoc == null)
        {
            fileLoc = SaveDir() + id + ".json";
        }
        string filePath = this.fileLoc;
        File.WriteAllText(filePath, dataAsJson);

    }

}