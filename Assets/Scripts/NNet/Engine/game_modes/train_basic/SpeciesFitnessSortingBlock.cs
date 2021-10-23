using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeciesFitnessSortingBlock: CTBaseObject
{
    public class ScoreBlock: CTBaseObject{
        public float score;
        public SpeciesObject speciesObject;
        public void Init(float _score, SpeciesObject _speciesObject){
            score = _score;
            speciesObject = _speciesObject;
        }
        public override string _class_name
        {
            get
            {
                return "SpeciesFitnessSortingBlock_ScoreBlock";
            }
        }
    }

    public override string _class_name
    {
        get
        {
            return "SpeciesFitnessSortingBlock";
        }
    }
    public List<ScoreBlock> storedStats = new List<ScoreBlock>();
    public int memberCapacity = -1;
  
    public float minScore
    {
        get
        {
            if (storedStats.Count == 0)
            {
                return 0;
            }
            return storedStats[storedStats.Count - 1].score;
        }
    }
    public float maxScore
    {
        get
        {
            if (storedStats.Count == 0)
            {
                return 0;
            }
            return storedStats[0].score;
        }
    }

    public SpeciesObject GetRandom(){
        if(storedStats.Count == 0){
            return null;
        }
        int index = Random.Range(0, storedStats.Count);
        return storedStats[index].speciesObject;
    }
    public bool TestSpeciesStat(SpeciesObject speciesObject, float score){
        ScoreBlock scoreBlock = new ScoreBlock();
        scoreBlock.Init(score, speciesObject);
        storedStats.Add(
            scoreBlock
        );
        storedStats.Sort((x, y) => y.score.CompareTo(x.score));
        if (storedStats.Count > memberCapacity)
        {
            /*
            for (int i = memberCapacity; i < storedStats.Count; i++)
            {
                UnityEngine.Object.Destroy(storedStats[i]);
            }
            */
            storedStats.RemoveRange(memberCapacity, storedStats.Count - memberCapacity);
        }


        return true;
    }
    public bool RemoveById(string id){
        List<ScoreBlock> toRemove = new List<ScoreBlock>();
        foreach (ScoreBlock scoreBlock in storedStats)
        {
            if (scoreBlock.speciesObject.id == id)
            {
                toRemove.Add(scoreBlock);

            }
        }
        foreach(ScoreBlock scoreBlock in toRemove){
            storedStats.Remove(scoreBlock);
        }
        return true;
    }

    public float GetSpeciesObjectScore(SpeciesObject speciesObject){
        foreach (ScoreBlock scoreBlock in storedStats)
        {
            if(scoreBlock.speciesObject.id == speciesObject.id){
                return scoreBlock.score;
            }
        }
        return -1;
    }
    //public override void OnDestroy()
    ~SpeciesFitnessSortingBlock()
    {
        //base.OnDestroy();
        /*
        for (int i = 0; i < storedStats.Count; i++)
        {
            UnityEngine.Object.Destroy(storedStats[i]);
        }*/
        storedStats.Clear();
        storedStats = null;

    }
}
