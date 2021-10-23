using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FitnessSortingBlock: CTBaseObject
{
    public class ScoreBlock: CTBaseObject{
        public float score;
        public NPCNNetController NpcnNetController;
        public void Init(float _score, NPCNNetController npcnNetController){
            score = _score;
            NpcnNetController = npcnNetController;
        }

        public override string _class_name
        {
            get
            {
                return "FitnessSortingBlock_ScoreBlock";
            }
        }
    }


    public List<ScoreBlock> storedStats;
        
    public int memberCapacity;

    public override string _class_name
    {
        get
        {
            return "FitnessSortingBlock";
        }
    }
    public float minScore{
        get{
            if(storedStats.Count == 0){
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
    public List<NPCNNetController> GetBotControllers(){
        List<NPCNNetController> botControllers = new List<NPCNNetController>();
        foreach(ScoreBlock scoreBlock in storedStats){
            botControllers.Add(scoreBlock.NpcnNetController);
        }
        return botControllers;
    }
    public NPCNNetController GetBotControllerByScore(float score){
        foreach (ScoreBlock scoreBlock in storedStats)
        {
            if (scoreBlock.score.Equals(score))
            {
                return scoreBlock.NpcnNetController;
            }
        }
        return null;
    }
 

    public FitnessSortingBlock():base(){
        storedStats = new List<ScoreBlock>();
    }
	//public override void OnDestroy()
    ~FitnessSortingBlock()
	{
        //base.OnDestroy();
        /*
        for (int i = 0; i < storedStats.Count; i++)
        {
            UnityEngine.Object.Destroy(storedStats[i]);
        }
        */
        storedStats.Clear();

	}
	public NPCNNetController GetRandom(){
        if(storedStats.Count == 0){
            return null;
        }
        int index = Random.Range(0, storedStats.Count);
        return storedStats[index].NpcnNetController;
    }
    public bool TestBotStat(NPCNNetController npcnNet, float score){
        ScoreBlock scoreBlock = new ScoreBlock();
        scoreBlock.Init(score, npcnNet);
        storedStats.Add(
            scoreBlock
        );
        storedStats.Sort((x, y) => y.score.CompareTo(x.score));
        if(storedStats.Count > memberCapacity){
            for (int i = memberCapacity; i < storedStats.Count; i++)
            {
                storedStats[i] = null;//UnityEngine.Object.Destroy(storedStats[i]);
            }
            storedStats.RemoveRange(memberCapacity, storedStats.Count - memberCapacity);
        }

           
        return IsBotInTopStored(npcnNet);
    }
    public bool IsBotInTopStored(NPCNNetController npcnNetController)
    {
        foreach (ScoreBlock scoreBlock in storedStats)
        {
            if (scoreBlock == null || scoreBlock.NpcnNetController == null){
                throw new System.Exception("Null ScoreBlock.botController");
            }
            if(scoreBlock.NpcnNetController.id == npcnNetController.id){
                return true;
            }
        }

        return false;
    }
    public FitnessSortingBlockData ToData(){
        FitnessSortingBlockData fitnessSortingBlockData = new FitnessSortingBlockData();
       
        fitnessSortingBlockData.memberCapacity =  memberCapacity;
        fitnessSortingBlockData.minScore = minScore;
        fitnessSortingBlockData.maxScore = maxScore;
        foreach(ScoreBlock scoreBlock in storedStats){
            
            fitnessSortingBlockData.scores.Add(new FitnessSortingBlockData.ScoreData(scoreBlock.score, scoreBlock.NpcnNetController.id));
        }
        //storedStats = new Dictionary<float, BotController>();
        return fitnessSortingBlockData;
    }
    public void ParseData(FitnessSortingBlockData fitnessSortingBlockData, SpeciesObject speciesObject){
        memberCapacity = fitnessSortingBlockData.memberCapacity;
     
        foreach (NPCNNetController botController in speciesObject.organisims)
        {
            foreach (FitnessSortingBlockData.ScoreData scoreData in fitnessSortingBlockData.scores)
            {

                if (botController.id == scoreData.botId)
                {
                    ScoreBlock scoreBlock = new ScoreBlock();
                    scoreBlock.Init(
                        scoreData.score,
                        botController
                    );
                    storedStats.Add(
                        scoreBlock
                    );

                }

            }
        }
    }
}
