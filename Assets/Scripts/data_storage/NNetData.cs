using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class NNetData: BaseData {


    [System.Serializable]
    public class NNetMutateHistory
    {
        public int generation;
        public string type;
        public string notes;
        public string neuron1;
        public string neuron2;
        public new string ToString(){
            string data = "";
            data += generation + "-" + type + "(" + neuron1;
            if(neuron2 != null){
                data += "/" + neuron2;
            }
            data += ")" + notes;
            return data;

        }
    }
    [System.Serializable]
    public class NeuronData: BaseData//ICTSerializedData
    {
        public string id;             //Minimum value for our Count class.
        public string type;             //Maximum value for our Count class.
        public string _base_type;
        public string layer;
        public string origenNNetId;
        public string origenSpecies;
        public int origenGen;
        public int innovationNumber;
        public List<NeuronDepData> dependencies = new List<NeuronDepData>();
      

        //Assignment constructor.
        public NeuronData(string _id, string _type):base()
        {
            id = _id;
            type = _type;

        }

        public override string _class_name
        {
            get
            {
                return "NNetData_NeuronData";
            }
        }
        /*
        public string Get(string key){
            foreach( NeuronDataProperty  property in properties){
                if(property.key == key){
                    return property.value;
                }
            }
            return null;
        }
        public void Set(string key, string value)
        {
            NeuronDataProperty property = null;
            foreach (NeuronDataProperty _property in properties)
            {
                if (_property.key == key)
                {
                    property = _property;
                }
            }
            if(property == null){
                property = new NeuronDataProperty();
                property.key = key;
                properties.Add(property);
            }
            property.value = value;

        }
        */
    }

    [System.Serializable]
    public class NeuronDataProperty
    {
        public string key;
        public string value;
    }
    [System.Serializable]
    public class NeuronDepData: ICTSerializedData
    {
        public string neuronId;             //Minimum value for our Count class.
       
        public int innovationNumber;
        public bool enabled;
        public string origenNNetId;
        public int origenGen;
        public string origenSpecies;
        public string _weight;
        public decimal weight{
            get{
                try{
                    return decimal.Parse(_weight);
                }catch(System.Exception e){
                    //Debug.LogError(e.Message);
                    return 0;
                }

            }
            set{
                _weight = value.ToString();
            }
        }


        //Assignment constructor.
        public NeuronDepData(string _neuronId, decimal _weight, int _innovationNumber)
        {
            neuronId = _neuronId;
            weight = _weight;
            innovationNumber = _innovationNumber;
        }
    }


    public string fileLoc;
    public string id;
    public int generation;
    public string parent;
    public List<NeuronData> neurons = new List<NeuronData>();
    public List<NNetMutateHistory> history = new List<NNetMutateHistory>();

    public override string _class_name
    {
        get
        {
            return "NNetData";
        }
    }
    public static NNetData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NNetData>(jsonString);
    }
    public static NNetData LoadFromLocal(string gameDataFileName)
    {
        string filePath = gameDataFileName;

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot load game data: " + filePath);
            return null;
        }
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(filePath);
        NNetData nNetData = CreateFromJSON(dataAsJson);
        nNetData.fileLoc = filePath;
        return nNetData;

    }

    public void Save()
    {

        string dataAsJson = JsonUtility.ToJson(this);
        Debug.Log("Trying to save: " + this.fileLoc);
        if(this.fileLoc == null){
            fileLoc = Application.persistentDataPath + "/save_data/nnet_saves/" + id + ".json";
        }
        string filePath = this.fileLoc;
        File.WriteAllText(filePath, dataAsJson);

    }

}
