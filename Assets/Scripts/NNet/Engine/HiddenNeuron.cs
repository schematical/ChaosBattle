using UnityEngine;
using System.Collections;

public class HiddenNeuron : BaseNeuron
{
    public int layer;
    public HiddenNeuron(string id, int _layer):base(id)
    {
        
        layer = _layer;
    }
    public override string _base_type
    {
        get { return "hidden"; }
    }
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        neuronData.Set("layer", layer.ToString());
        return neuronData;
    }
}
