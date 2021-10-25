
using System;

public class EntityStatInput: InputNeuron
{
    public const float ZERO_VALUE = -1f;
    public ChaosEntityStatType StatType;
    public EntityStatInput(string id) : base(id)
    {
        
    }
    public override void Evaluate()
    {
        _lastValue = ZERO_VALUE;
        try
        {
            float statVal = this.npc.CurrentActionCandidate.Entity.GetStatVal(StatType);
            _lastValue = statVal;
        }
        catch (Exception e)
        {
            
        }

    }
   
   
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        //neuronData.Set("entityFilterData", JsonUtility.ToJson(entityFilterData));
        //neuronData.Set("tileFilterData", JsonUtility.ToJson(tileFilterData));
       
        neuronData.Set("statType", StatType.ToString());
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        /*
        string _entityFilterData = neuronData.Get("entityFilterData");
        entityFilterData = JsonUtility.FromJson<EntityFilterData>(_entityFilterData);

        string _tileFilterData = neuronData.Get("tileFilterData");
        tileFilterData = JsonUtility.FromJson<TileFilterData>(_tileFilterData);
        */
        ChaosEntityStatType.TryParse(neuronData.Get("statType"), out StatType);
    }
    /*public override void PopulateRandom()
    {
        base.PopulateRandom();
       
        int index = Random.Range(0, GameManager.instance.prefabManager.tiles.Length - 1);
        /*
        TileObject tileObject = GameManager.instance.prefabManager.tiles[index].GetComponent<TileObject>();
        tileFilterData = new TileFilterData
        {
            tileTypes = new List<string>{
                tileObject.name
            }
        };
        #1#

        throw new System.Exception("Not implemented fully. TODO: Add entities");
    }*/
    public override string ToString()
    {
        string data = "EntityStatInput - ";
       
        data += " - " + StatType;
        data +=  " - Last:" + _lastValue;
        return data;
    }
    ~EntityStatInput()
	//public override void OnDestroy()
	{
        //base.OnDestroy();

    }


}
