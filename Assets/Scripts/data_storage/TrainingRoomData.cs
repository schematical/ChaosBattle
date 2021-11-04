using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    

[System.Serializable]
public class TrainingRoomData : ICTSerializedData {


    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [System.Serializable]
    public class Tile
    {
        public int x;             //Minimum value for our Count class.
        public int y;             //Maximum value for our Count class.
        public string type;
        public string tileGroupId;


        //Assignment constructor.
        public Tile(int _x, int _y, string _type)
        {
            x = _x;
            y = _y;
            type = _type;
        }
    }

    [System.Serializable]
    public class EntityData: BaseData
    {
        public float x;             //Minimum value for our Count class.
        public float y;             //Maximum value for our Count class.
        public float rotation;
        public string type;


        //Assignment constructor.
        public EntityData(float _x, float _y, string _type):base()
        {
            x = _x;
            y = _y;
            type = _type;
        }
        public override string _class_name
        {
            get
            {
                return "EntityData";
            }
        }
    }
    public class FitnessRule{
        public int score = 1;
        public string stat;

    }

    public string fileLoc;
    public string name;
    public List<Tile> tiles = new List<Tile>();
    public List<EntityData> entities = new List<EntityData>();

    public BrainMakerConfigData brainMakerConfigData;
    public FitnessManagerConfigData fitnessManagerConfigData;
    public DisplaySettingsData displaySettingsData;
    //TODO: Fix this
    public BotBiologyData botBiologyData;
    public TrainingRoomData(){
        brainMakerConfigData = new BrainMakerConfigData();
        fitnessManagerConfigData = new FitnessManagerConfigData();
        displaySettingsData = new DisplaySettingsData();
        botBiologyData = new BotBiologyData
            {
                eyes = new List<BotBiologyData.EyeData>{
                    new BotBiologyData.EyeData{
                        id = "0",
                        visionDistance = 4,
                        angle = -45
                    },
                    new BotBiologyData.EyeData{
                        id = "1",
                        visionDistance = 7,
                        angle = -15
                    },
                    new BotBiologyData.EyeData{
                        id = "2",
                        visionDistance = 7,
                        angle = 15
                    },
                    new BotBiologyData.EyeData{
                        id = "3",
                        visionDistance = 4,
                        angle = 45
                    }
                }
            };
    }
    public string saveStateDir{
        get{
            return fileLoc + "/save_states";
        }
    }

    public static TrainingRoomData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TrainingRoomData>(jsonString);
    }



    public static TrainingRoomData LoadFromLocal(string gameDataFileName)
    {
        return LoadFromLocal(gameDataFileName, true);
    }
    public static TrainingRoomData LoadFromLocal(string gameDataFileName, bool useLocalName)
    {

        string filePath = gameDataFileName + "/trainingRoom.json";
       
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
        TrainingRoomData trainingRoomData =  CreateFromJSON(dataAsJson);



        //trainingRoomData.fileLoc = filePath;
        return trainingRoomData;

    }

    public void Save()
    {
      
        string dataAsJson = JsonUtility.ToJson(this);

        System.IO.Directory.CreateDirectory(this.fileLoc);
        GameManager.instance.menuManager.debugPanel.Log("Saving: " + this.fileLoc + "/trainingRoom.json");
        File.WriteAllText(this.fileLoc + "/trainingRoom.json", dataAsJson);

    }
    public List<string> GetSaveStates(){
        string dirPath = saveStateDir;
        System.IO.Directory.CreateDirectory(dirPath);
        DirectoryInfo dir = new DirectoryInfo(dirPath);

        DirectoryInfo[] info = dir.GetDirectories();// GetFiles("*.json");
        List<string> saveStates = new List<string>();
        foreach (DirectoryInfo f in info)
        {
            saveStates.Add(f.Name);//f.FullName);
        }
        return saveStates;
    }

}