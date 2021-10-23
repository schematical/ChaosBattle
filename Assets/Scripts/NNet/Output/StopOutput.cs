using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopOutput : OutputNeuron {
  
    public StopOutput(string id) : base(id)
    {
    }
	public override void Execute(float score)
	{
        
        if (NpcnNet.entity.rigidbody2D == null)
        {
            return;
        }

        NpcnNet.entity.rigidbody2D.velocity = new Vector2(0f,0f);
    
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
