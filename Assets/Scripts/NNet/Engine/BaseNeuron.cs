using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 

public abstract class BaseNeuron : CTBaseObject, ICTSerializable
{

    public static class BaseType
    {
        public const string output = "output";
        public const string input = "input";
        public const string hidden = "hidden";
    }
    public override string _class_name
    {
        get
        {
            return "BaseNeuron";
        }
    }

    private string _id;
    private int _innovationNumber;
    protected float _lastValue = 0;
    protected List<NeuronDep> _dependencies;
    protected NNet nNet;
    public bool debug = false;
    public int origenGen;
    public string origenNNetId;
    public string origenSpecies;
    public bool hasBeenEvaluatedThisTick = false;

    // Use this for initialization
    public BaseNeuron(string id)
    {
        _dependencies = new List<NeuronDep>();   //A list of possible locations to place tiles.
        _innovationNumber = BrainMaker.IncInnovationNumber();
        _id = id;
        _lastValue = 0;
        debug = false;
        hasBeenEvaluatedThisTick = false;
    }
    public string id
    {
        get { return _id; }

    }
    public int innovationNumber
    {
        get { return _innovationNumber; }
    }
    public NPCNNetController npc
    {
        get { return nNet.NPCController; }
        //set { bar = value; }
    }
    public float lastValue
    {
        get { return _lastValue; }
        //set { bar = value; }
    }
    public List<NeuronDep> dependencies
    {
        get { return _dependencies; }
        //set { bar = value; }
    }
    public NeuronDep AddDep(BaseNeuron baseNeuron, decimal weight)
    {
        NeuronDep neuronDep = new NeuronDep();
        neuronDep.Init(baseNeuron, weight, -1);
        _dependencies.Add(neuronDep);
        return neuronDep;

    }
    public void AttachNNet(NNet _nNet)
    {
        nNet = _nNet;

    }
    public virtual void Evaluate()
    {
        Evaluate(true);
    }
    public virtual void Evaluate(bool evalDependancies)
    {
        if (hasBeenEvaluatedThisTick)
        {
            return;
        }
        hasBeenEvaluatedThisTick = true;
        float totalScores = 0;
        if (_dependencies.Count == 0)
        {
            return;
        }

        foreach (NeuronDep neuronDep in _dependencies)
        {
            if (neuronDep.enabled && !neuronDep.weight.Equals(0))
            {
                if (evalDependancies)
                {
                    neuronDep.depNeuron.Evaluate();//TODO: This could become an infinite loop if we are not carful
                }
                float result = neuronDep.depNeuron.lastValue;
                neuronDep.lastValue = (float)neuronDep.weight * result;
                totalScores += neuronDep.lastValue;
            }

        }

        float newValue = totalScores;

        /*if (GameManager.instance.trainingRoomData.brainMakerConfigData.lstmLogicEnabled)
        {
            newValue = _lastValue * newValue;
        }*/
        _lastValue = Sigmoid(newValue);


    }
    public virtual NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = new NNetData.NeuronData(id, GetType().ToString());
        neuronData._base_type = _base_type;
        neuronData.innovationNumber = innovationNumber;
        neuronData.origenGen = origenGen;
        neuronData.origenNNetId = origenNNetId;
        neuronData.origenSpecies = origenSpecies;
        foreach (NeuronDep neuronDep in _dependencies)
        {
            neuronData.dependencies.Add(neuronDep.ToData());
        }


        System.Reflection.PropertyInfo[] properties = this.GetType().GetProperties();
        /*
         foreach(System.Reflection.PropertyInfo property in properties){
            property.GetValue(this);
        }
        */
        return neuronData;
    }
    public virtual void ParseData(NNetData.NeuronData neuronData)
    {
        this._id = neuronData.id;
        this._innovationNumber = neuronData.innovationNumber;
        this.origenGen = neuronData.origenGen;
        origenNNetId = neuronData.origenNNetId;
        origenSpecies = neuronData.origenSpecies;
        foreach (NNetData.NeuronDepData neuronDepData in neuronData.dependencies)
        {
            NeuronDep neuronDep = new NeuronDep();
            neuronDep.Init(
                    nNet.neurons[neuronDepData.neuronId],
                    neuronDepData.weight,
                    neuronDepData.innovationNumber
            );
            neuronDep.ParseData(neuronDepData);
            _dependencies.Add(
                neuronDep
            );
        }
    }
    public virtual void PopulateRandom()
    {
        //Do nothing 
    }
    public abstract string _base_type
    {
        get;

    }
    public float Sigmoid(float x)
    {

        return 1 / (1 + Mathf.Exp(-x));
    }
    public override string ToString()
    {
        string rString = "";
        rString += /* _base_type + " " + */ GetType().ToString();

        return rString;
    }
    ~BaseNeuron(){
        //base.OnDestroy();
        /*
        for (int i = 0; i < dependencies.Count; i++)
        {
            UnityEngine.Object.Destroy(dependencies[i]);
        }*/
        nNet = null;
        _dependencies.Clear();
        _dependencies = null;
	}
	/*
    public override void Dispose()
    {
        base.Dispose();

        for (int i = 0; i < dependencies.Count; i++)
        {
            dependencies[i].Dispose();
        }
    }
    */

}
