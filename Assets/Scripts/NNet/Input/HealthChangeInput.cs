using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthChangeInput : InputNeuron
{
 

    public HealthChangeInput(string id) : base(id)
    {
        
    }

    public override void Evaluate()
    {
        _lastValue = 0f;

        foreach(WorldEvent worldEvent in NpcnNet.tickEvents){
            if(worldEvent.eventType == WorldEvent.WorldEventTypes.HEALTH_CHANGE){
                WorldHealthChangeEvent worldHealthChangeEvent = (WorldHealthChangeEvent)worldEvent;
   
                _lastValue += worldHealthChangeEvent.healthChangeEvent.change/100;
                //Debug.Log("Health Change: " + _lastValue.ToString());
                
            }
               

        }

       
    }
    /*
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        neuronData.Set("pitch", pitch.ToString());
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        pitch = int.Parse(neuronData.Get("pitch"));
    }*/
    /* public override void PopulateRandom()
    {
        base.PopulateRandom();
        //int index = Random.Range(0, 7);
    
    }*/

}