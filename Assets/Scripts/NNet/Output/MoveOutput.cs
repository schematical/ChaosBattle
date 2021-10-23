using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOutput : OutputNeuron {

    public MoveOutput(string id) : base(id)
    {
    }
    public override void Execute(float score)
    {

        if (NpcnNet.entity.rigidbody2D == null)
        {
            return;
        }
        float xV = 0;
        if (GameManager.instance.trainingRoomData.brainMakerConfigData.useBinaryNeuronOutput)
        {
            if (score > .5f)
            {
                xV = NpcnNet.botBiology.speed;
            }else{
                xV = 0;//speed * -1;
            }
        }else{
            if (score > .5f)
            {
                xV = (NpcnNet.botBiology.speed) * ((score * 2) - 1);
            }
        }

        float yV = 0;
    
        Vector2 v = new Vector2(xV, yV);
       
        Vector2 rotatedVector = v.Rotate(NpcnNet.entity.rigidbody2D.rotation);

        //Vector2 estimatedVector = bot.entity.rigidbody2D.velocity + rotatedVector;
        //if(estimatedVector.magnitude < bot.botBiology.speed){
            NpcnNet.entity.rigidbody2D.velocity = rotatedVector; 
        //}

      
    
	}
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        //neuronData.Set("speed", speed.ToString());
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        //string _speed = neuronData.Get("speed");
        //speed = float.Parse(_speed);
    }
	public override void PopulateRandom()
	{
        base.PopulateRandom();
        //speed = Random.Range(1, 6) * .3f;
	}
}
