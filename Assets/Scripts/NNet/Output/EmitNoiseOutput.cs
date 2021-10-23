using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitNoiseOutput : OutputNeuron {
    public int pitch = 0;
    public int coolDownCount = 0;
    public EmitNoiseOutput(string id) : base(id)
    {
    }
    public override void Execute(float score)
    {
        if(score < 0){
            return;
        }
        if (NpcnNet.entity.rigidbody2D == null)
        {
            return;
        }
        if(coolDownCount > 0){
            coolDownCount -= 1;
            return;
        }
        coolDownCount = 20;

      
        GameObject instance = GameManager.instance.boardManager.AddMisc("NoiseObject", NpcnNet.entity.transform.position, Quaternion.identity);
        NoiseObject noiseObject = instance.GetComponent<NoiseObject>();
        instance.transform.SetParent(GameManager.instance.boardManager.boardHolder, false);
        int _pitch = (int) Mathf.Floor(score * 8);
        noiseObject.Init(_pitch, 1);
        instance = GameManager.instance.boardManager.AddMisc("SpeechBubble", NpcnNet.entity.transform.position, Quaternion.identity);
        SpeechBubble speechBubble = instance.GetComponent<SpeechBubble>();
        instance.transform.SetParent(GameManager.instance.boardManager.boardHolder, false);
        speechBubble.Setup(NpcnNet.entity.gameObject, noiseObject.pitch);
    
	}
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        neuronData.Set("pitch", pitch.ToString());
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        string _pitch = neuronData.Get("pitch");
        pitch = int.Parse(_pitch);
    }
	public override void PopulateRandom()
	{
        base.PopulateRandom();
        pitch = Random.Range(0, 7);
	}
}
