using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocityInput : InputNeuron
{
   
    public AngularVelocityInput(string id) : base(id)
    {
        
    }

    public override void Evaluate()
    {
        if (NpcnNet.entity.rigidbody2D == null)
        {
            return;
        }
        _lastValue = (NpcnNet.entity.rigidbody2D.angularVelocity / 360 * 2) - 1;

       
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