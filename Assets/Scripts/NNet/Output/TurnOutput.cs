using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOutput : OutputNeuron {
    //public float rot;
    public TurnOutput(string id) : base(id)
    {
    }
	public override void Execute(float score)
	{
        if (NpcnNet.entity.rigidbody2D == null)
        {
            return;
        }
        float rotModifier = 0;
        if (GameManager.instance.trainingRoomData.brainMakerConfigData.useBinaryNeuronOutput)
        {
            if(score > .5f){
                rotModifier = score;
            }else{
                rotModifier = score * -1;
            }
        }else{
            rotModifier = ((score * 2f) - 1f);
        }
       
        if(rotModifier.Equals(0)){
            return;
        }
        float newAngularVelocityDeg = NpcnNet.botBiology.turnSpeed * rotModifier;
        NpcnNet.entity.rigidbody2D.angularVelocity = newAngularVelocityDeg;

	}
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        //neuronData.Set("rot", rot.ToString());
        return neuronData;
    }
	public override void ParseData(NNetData.NeuronData neuronData)
	{
        base.ParseData(neuronData);
        //string _rot = neuronData.Get("rot");
        //rot = float.Parse(_rot);
	}
	/* public override void PopulateRandom()
	{
        base.PopulateRandom();
        rot = 0;
        while (rot.Equals(0))
        {
            rot = Random.Range(-2, 2) * Mathf.PI / 8;
        }
	} */
}
