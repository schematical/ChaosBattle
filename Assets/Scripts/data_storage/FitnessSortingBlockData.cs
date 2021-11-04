using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class FitnessSortingBlockData: BaseData
{
    [System.Serializable]
    public class ScoreData{
        public float score;
        public string botId;
        public ScoreData(float _score, string _botId){
            score = _score;
            botId = _botId;
        }
        public ScoreData()
        {
     
            GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
        }

        ~ScoreData()
        {
           
            GameManager.instance.garbageCollector.DeregisterObject(_class_name);
        }

        public string _class_name
        {
            get
            {
                return "FitnessSortingBlockData_ScoreData";
            }
        }

    }
   
    public override string _class_name
    {
        get
        {
            return "FitnessSortingBlockData";
        }
    }

    public int memberCapacity = 100;
    public float minScore = 0;
    public float maxScore = 0;
    public List<ScoreData> scores = new List<ScoreData>();
    /*
	public override void OnDestroy()
	{
        base.OnDestroy();
        for (int i = 0; i < scores.Count; i++)
        {
            UnityEngine.Object.Destroy(scores[i]);//.Dispose();
        }
        scores.Clear();
	}
	*/
	/* public override void Dispose()
	{
        base.Dispose();
        for (int i = 0; i < scores.Count; i++ ){
            scores[i].Dispose();
        }
        scores.Clear();
	}
	*/

}
