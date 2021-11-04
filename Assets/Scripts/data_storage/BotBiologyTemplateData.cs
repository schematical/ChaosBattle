using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class BotBiologyTemplateData: BaseData
{
    [System.Serializable]
    public class BotBiologyPropertyTemplateData{
        public string id;
        public bool canMutateBeyondNaturalBounds;
        public int min;
        public int max;
        public float mutateRate;
    }

    public override string _class_name
    {
        get
        {
            return "BotBiologyTemplateData";
        }
    }

 
    public BotBiologyData Spawn(){
        //Create a new bot biology
        BotBiologyData botBiologyData = new BotBiologyData();

        //Iterate through properties


        return botBiologyData;
    }

    public void Mutate(BotBiologyData botBiologyData){
        
    }

  
}
