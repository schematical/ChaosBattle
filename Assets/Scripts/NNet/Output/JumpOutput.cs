using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpOutput : OutputNeuron {
    public float currCooloffSec = 0;
    public float maxCooloffSec = 1;
    public JumpOutput(string id) : base(id)
    {
        
    }
    public override void Evaluate(bool evalDependancies)
	{
       
        base.Evaluate(evalDependancies);

	}
	public override void Execute(float score)
	{
        if (NpcnNet.entity.rigidbody2D == null)
        {
            return;
        }
        if (currCooloffSec > 0)
        {

            currCooloffSec -= Time.deltaTime * Time.timeScale;
            return;
        }

        if (GameManager.instance.trainingRoomData.brainMakerConfigData.useBinaryNeuronOutput)
        {
            if(score > .5f){
                NpcnNet.entity.zVelosity = 1;
            }
        }else{
            NpcnNet.entity.zVelosity = score;
        }
        if(score > 0){
            currCooloffSec = 5;
        }
       
       
       

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
	public override void PopulateRandom()
	{
        base.PopulateRandom();
       
	}
}
