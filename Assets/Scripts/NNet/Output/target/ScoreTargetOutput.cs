
using System;
using UnityEngine;

public class ScoreTargetOutput : OutputNeuron
{
        public Type ActionType;
        public ScoreTargetOutput(string id) : base(id)
        {
     
        }
        public override void Execute(float score)
        {
            npc.CurrentActionCandidate.TypeScores.Add(ActionType, score);
        }
        public override NNetData.NeuronData ToData()
        {
            NNetData.NeuronData neuronData = base.ToData();
            neuronData.Set("actionType", ActionType.ToString());
            return neuronData;
        }
        public override void ParseData(NNetData.NeuronData neuronData)
        {
            base.ParseData(neuronData);
            
            string actionType = neuronData.Get("actionType");
            switch (actionType)
            {
                case("NavigateToAction"):
                    ActionType = typeof(NavigateToAction);
                break;
                case("UsePrimaryItemAction"):
                    ActionType = typeof(UsePrimaryItemAction);
                    break;
                default: 
                    throw new Exception("Not sure how to ParseData of `actionType`: " + actionType);
            }
            //speed = float.Parse(_speed);
        }
        public override void PopulateRandom()
        {
            base.PopulateRandom();
            //speed = Random.Range(1, 6) * .3f;
        }
    

}
