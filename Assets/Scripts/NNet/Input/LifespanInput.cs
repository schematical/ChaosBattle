using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LifespanInput : InputNeuron
{
   
    public LifespanInput(string id) : base(id)
    {
        
    }

    public override void Evaluate()
    {
        if (NpcnNet.entity.rigidbody2D == null)
        {
            return;
        }
        _lastValue = (float)(NpcnNet.realGameAge / NpcnNet.maxLifeExpectancy);//((bot.realGameAge/ bot.maxLifeExpectancy) * 2) -1;

       
    }
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();

        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
       
    }
    public override void PopulateRandom()
    {
        base.PopulateRandom();
    }

}