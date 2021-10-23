using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RememberOutput : OutputNeuron {
    public int memoryLocation;
    protected bool value;
    public RememberOutput(string id) : base(id)
    {
    }
	public override void Execute(float score)
	{
        NpcnNet.memory.SetFloat(memoryLocation, score);
    
	}
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        neuronData.Set("memoryLocation", memoryLocation.ToString());
        neuronData.Set("value", value.ToString());
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        string _memoryLocation = neuronData.Get("memoryLocation");
        memoryLocation = int.Parse(_memoryLocation);
        string _value = neuronData.Get("value");
        value = bool.Parse(_value);
    }
    public override void PopulateRandom()
    {
        base.PopulateRandom();
        memoryLocation = Random.Range(0, 7);
        if (Random.Range(0, 2) > .5)
        {
            value = true;
        }
        else
        {
            value = false;
        }
    }

}
