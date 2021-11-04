using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    

[System.Serializable]
public class DebugData : ICTSerializedData {
    public class DebugInfoType{
        public const string PARENT_IDS = "PARENT_IDS";
    }

    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [System.Serializable]
    public class DebugInfo
    {
        public string entityId;
        public string entityType;
        public string infoType;
       

        public DebugInfo(string _entityId, string _entityType, string _infoType)
        {
            entityId = _entityId;
            entityType = _entityType;
            infoType = _infoType;
        }
    }


    public string fileLoc;
    public List<DebugInfo> debugInfos = new List<DebugInfo>();
    public DebugData(){
        
    }
    public string saveStateDir{
        get{
            return fileLoc + "/save_states";
        }
    }

    public static DebugData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<DebugData>(jsonString);
    }



    public static DebugData LoadFromLocal(string gameDataFileName)
    {
        return LoadFromLocal(gameDataFileName, true);
    }
    public static DebugData LoadFromLocal(string gameDataFileName, bool useLocalName)
    {

        string filePath = gameDataFileName + "/debug.json";
       
        if(!useLocalName){
            filePath = gameDataFileName;
        }
        GameManager.instance.menuManager.debugPanel.Log("Loading: " + filePath);
        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot load game data: " + filePath);
            return null;
        }
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(filePath);
        DebugData trainingRoomData =  CreateFromJSON(dataAsJson);
        //trainingRoomData.fileLoc = filePath;
        return trainingRoomData;

    }

    public void Save()
    {
      
        string dataAsJson = JsonUtility.ToJson(this);

        System.IO.Directory.CreateDirectory(this.fileLoc);
        GameManager.instance.menuManager.debugPanel.Log("Saving: " + this.fileLoc + "/debug.json");
        File.WriteAllText(this.fileLoc + "/debug.json", dataAsJson);

    }


}