using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class BotControllerScoreData: BaseData {
    public int respawnCount = 0;
    public List<float> scores = new List<float>();

    public float GetMeanScore(){
        float totalScores = 0;
        foreach(float score in scores){
            totalScores += score;
        }
        return totalScores / scores.Count;
    }

    public override string _class_name
    {
        get
        {
            return "BotControllerScoreData";
        }
    }

}
