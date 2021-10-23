using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiasInput : InputNeuron
{
    public decimal weight;
   
    public BiasInput(string id) : base(id)
    {
       
    }

    public override void Evaluate()
    {

        _lastValue = (float)weight;//TODO: Convert this to a decimal

       
    }
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        neuronData.Set("weight", weight.ToString());
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        weight = decimal.Parse(neuronData.Get("weight"));
       
    }
    public override void PopulateRandom()
    {
        base.PopulateRandom();
        weight = Random.Range(-10, 10);
    }

}