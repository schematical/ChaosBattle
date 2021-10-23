using UnityEngine;
using System.Collections;

public class CanRememberInput : InputNeuron
{
    public int memoryLocation;
    public CanRememberInput(string id) : base(id)
    {
    }

    public override void Evaluate()
    {
        _lastValue = NpcnNet.memory.GetFloat(memoryLocation);
    }
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        neuronData.Set("memoryLocation", memoryLocation.ToString());
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        memoryLocation = int.Parse(neuronData.Get("memoryLocation"));
    }
    public override void PopulateRandom()
    {
        base.PopulateRandom();
        memoryLocation = Random.Range(0, 7);
    }
}
