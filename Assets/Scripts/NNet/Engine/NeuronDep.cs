using UnityEngine;
using System.Collections;

public class NeuronDep: CTBaseObject
{

  
    public BaseNeuron depNeuron;
    public decimal weight = 1;
    public bool enabled = true;
    private int _innovationNumber;
    public int origenGen;
    public string origenNNetId;
    public string origenSpecies;
    public float lastValue = 0;
  
    public override string _class_name
    {
        get
        {
            return "NeuronDep";
        }
    }
    public void Init(BaseNeuron _depNeuron, decimal _weight, int innovationNumber)
    {
        depNeuron = _depNeuron;
        weight = _weight;
        if (innovationNumber != -1)
        {
            _innovationNumber = innovationNumber;
        }else{
            _innovationNumber = BrainMaker.IncInnovationNumber();
        }
        if (depNeuron.NpcnNet == null)
        {
            origenGen = 0;
            origenNNetId = null;
        }

    }
    public int innovationNumber
    {
        get { return _innovationNumber; }
    }
    public void ParseData(NNetData.NeuronDepData neuronDepData){
        _innovationNumber = neuronDepData.innovationNumber;
        enabled = neuronDepData.enabled;
        origenGen = neuronDepData.origenGen;
        origenNNetId = neuronDepData.origenNNetId;
        origenSpecies = neuronDepData.origenSpecies;
       
    }
    public NNetData.NeuronDepData ToData(){
        NNetData.NeuronDepData neuronDepData = new NNetData.NeuronDepData(depNeuron.id, weight, _innovationNumber);
        neuronDepData.enabled = enabled;
        neuronDepData.origenGen = origenGen;
        neuronDepData.origenNNetId = origenNNetId;
        neuronDepData.origenSpecies = origenSpecies;
        return neuronDepData;
    }
	//public override void OnDestroy()
    ~NeuronDep()
	{
       // base.OnDestroy();
        depNeuron = null;
	}
}
