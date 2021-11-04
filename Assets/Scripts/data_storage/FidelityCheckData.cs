using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    

[System.Serializable]
public class FidelityCheckData : ICTSerializedData {


    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [System.Serializable]
    public class NeuronEvaluationResult
    {
        public string neuronId;
        public float result;


        //Assignment constructor.
        public NeuronEvaluationResult(string _neuronId, float _result)
        {
            neuronId = _neuronId;
            result = _result;
           
        }
    }
    [System.Serializable]
    public class NNetTickEvaluationResult
    {
        public int tickAge;
       
        public List<NeuronEvaluationResult> neuronResults = new List<NeuronEvaluationResult>();
        public NeuronEvaluationResult AddNewNeuronEvaluationResult(string _neuronId, float _result){
            NeuronEvaluationResult neuronEvaluationResult = new NeuronEvaluationResult(_neuronId, _result);
            return neuronEvaluationResult;
        }
        public FidelityCompareNNetTickResult CompairTo(NNetTickEvaluationResult _botRunEvaluationResult)
        {
            FidelityCompareNNetTickResult fidelityCompareNNetResult = new FidelityCompareNNetTickResult();
            fidelityCompareNNetResult.tickAge1 = this.tickAge;
            fidelityCompareNNetResult.tickAge2 = _botRunEvaluationResult.tickAge;
            for (int i = 0; i < neuronResults.Count; i++)
            {
                NeuronEvaluationResult neuronEvaluationResult1 = neuronResults[i];
                NeuronEvaluationResult neuronEvaluationResult2 = _botRunEvaluationResult.neuronResults[i];
                if (neuronEvaluationResult1.neuronId != neuronEvaluationResult2.neuronId)
                {
                    throw new System.Exception("Tick Age Missmatch... TODO: Sort these I guess?");
                }
                fidelityCompareNNetResult.totalNeuronsTested += 1;
                if(!neuronEvaluationResult1.result.Equals(neuronEvaluationResult2.result)){
                    fidelityCompareNNetResult.diffNeurons.Add(
                        new FidelityCompareNeuronResult(
                            neuronEvaluationResult1,
                            neuronEvaluationResult2
                        )
                    );

                }

            }
            return fidelityCompareNNetResult;
        }
      
    }

    [System.Serializable]
    public class BotRunEvaluationResult
    {
        public int spawnCount;

        public List<NNetTickEvaluationResult> nNetTickResults = new List<NNetTickEvaluationResult>();
        public NNetTickEvaluationResult AddNewNNetEvaluationResult(int tickAge)
        {
            NNetTickEvaluationResult nNetTickEvaluationResult = new NNetTickEvaluationResult();
            nNetTickEvaluationResult.tickAge = tickAge;
            nNetTickResults.Add(nNetTickEvaluationResult);
            return nNetTickEvaluationResult;
        }

        public FidelityCompareBotRunResult CompairTo(BotRunEvaluationResult _botRunEvaluationResult)
        {
            FidelityCompareBotRunResult fidelityCompareBotRunResult = new FidelityCompareBotRunResult();
            fidelityCompareBotRunResult.botRun1 = this.spawnCount;
            fidelityCompareBotRunResult.botRun2 = _botRunEvaluationResult.spawnCount;
            if(nNetTickResults.Count != _botRunEvaluationResult.nNetTickResults.Count){
                throw new System.Exception("Compaing SpawnCount " + this.spawnCount + " to " +_botRunEvaluationResult.spawnCount + " - Error Count Mismatch: " + nNetTickResults.Count + " != " + _botRunEvaluationResult.nNetTickResults.Count);
            }
            nNetTickResults.Sort((x, y) =>  x.tickAge.CompareTo(y.tickAge));
            _botRunEvaluationResult.nNetTickResults.Sort((x, y) => x.tickAge.CompareTo(y.tickAge));
            for (int i = 0; i < nNetTickResults.Count; i++)
            {
                NNetTickEvaluationResult nNetEvaluationResult1 = nNetTickResults[i];
                NNetTickEvaluationResult nNetEvaluationResult2 = _botRunEvaluationResult.nNetTickResults[i];
                if (nNetEvaluationResult1.tickAge != nNetEvaluationResult2.tickAge)
                {
                    
                    throw new System.Exception("Tick Age Missmatch... TODO: Sort these I guess?" + nNetEvaluationResult1.tickAge + "!=" + nNetEvaluationResult2.tickAge + " \n " + nNetTickResults.Count + " - " + _botRunEvaluationResult.nNetTickResults.Count);
                }
                FidelityCompareNNetTickResult fidelityCompareNNetTickResult = nNetEvaluationResult1.CompairTo(nNetEvaluationResult2);
                if(fidelityCompareNNetTickResult.diffNeurons.Count > 0){
                    fidelityCompareBotRunResult.diffTicks.Add(fidelityCompareNNetTickResult);
                }
            }
            return fidelityCompareBotRunResult;
        }
    }


    public class FidelityCompareResult
    {
        public List<FidelityCompareBotRunResult> botRunDiffs = new List<FidelityCompareBotRunResult>();
        public int botRunCount = 0;
        public float GetDiffPercent(){
            float diffPercent = 0;
            foreach(FidelityCompareBotRunResult fidelityCompareBotRunResult in botRunDiffs){
                float botRunDiffPercent = 0;
                foreach (FidelityCompareNNetTickResult fidelityCompareNNetTickResult in fidelityCompareBotRunResult.diffTicks)
                {
                    float tickDiffPercent = 0;
                    //foreach (FidelityCompareNeuronResult fidelityCompareNeuronResult in fidelityCompareNNetTickResult.diffNeurons){
                        tickDiffPercent += fidelityCompareNNetTickResult.diffNeurons.Count / fidelityCompareNNetTickResult.totalNeuronsTested;
                    //}
                    botRunDiffPercent += tickDiffPercent / fidelityCompareBotRunResult.diffTicks.Count;
                }
                diffPercent += botRunDiffPercent / botRunDiffs.Count;

            }
            return diffPercent;
        }
        public override string ToString()
        {
            string data = "Diff Percentage: " + GetDiffPercent().ToString() + "\n";
            foreach(FidelityCompareBotRunResult fidelityCompareBotRunResult in botRunDiffs){
                data += "\t BotRun: " + fidelityCompareBotRunResult.botRun1 + " != " + fidelityCompareBotRunResult.botRun2 + "\n";
                foreach (FidelityCompareNNetTickResult fidelityCompareNNetTickResult in fidelityCompareBotRunResult.diffTicks)
                {

                    data += "\t\t NNetTick: " + fidelityCompareNNetTickResult.tickAge1 + " != " + fidelityCompareNNetTickResult.tickAge1 + "\n";
                    foreach( FidelityCompareNeuronResult fidelityCompareNeuronResult in fidelityCompareNNetTickResult.diffNeurons)
                    {
                        data += "\t\t\t " + fidelityCompareNeuronResult.neuronEvaluationResult1.neuronId + "  " + fidelityCompareNeuronResult.neuronEvaluationResult2.neuronId + "\n";
                        data += "\t\t\t " + fidelityCompareNeuronResult.neuronEvaluationResult2.result.ToString() + "  " + fidelityCompareNeuronResult.neuronEvaluationResult2.result.ToString() + "\n";
                    
                    }
                }  
            }
            return data;

        }
    }
    public class FidelityCompareNeuronResult
    {
        public NeuronEvaluationResult neuronEvaluationResult1;
        public NeuronEvaluationResult neuronEvaluationResult2;
        public FidelityCompareNeuronResult(NeuronEvaluationResult _neuronEvaluationResult1, NeuronEvaluationResult _neuronEvaluationResult2){
            neuronEvaluationResult1 = _neuronEvaluationResult1;
            neuronEvaluationResult2 = _neuronEvaluationResult2;
        }
    }
    public class FidelityCompareNNetTickResult
    {
        public int tickAge1;
        public int tickAge2;
        public List<FidelityCompareNeuronResult> diffNeurons = new List<FidelityCompareNeuronResult>();
        public int totalNeuronsTested = 0;
    }
    public class FidelityCompareBotRunResult
    {
        public int ticksTested = 0;
        public int botRun1;
        public int botRun2;
        public List<FidelityCompareNNetTickResult> diffTicks = new List<FidelityCompareNNetTickResult>();

    }
   

    public string fileLoc;
    public string nNetId;
    public int checkNum = 0;
    public List<BotRunEvaluationResult> botRunResults = new List<BotRunEvaluationResult>();
    public BotRunEvaluationResult currBotRun;
    public FidelityCheckData(){
        
    }
 
    public BotRunEvaluationResult AddBotRunEvaluationResult(int spawnCount)
    {
        BotRunEvaluationResult botRunEvaluationResult = new BotRunEvaluationResult();
        botRunEvaluationResult.spawnCount = spawnCount;
        botRunResults.Add(botRunEvaluationResult);
        currBotRun = botRunEvaluationResult;
        return botRunEvaluationResult;
    }
    public NNetTickEvaluationResult AddNewNNetEvaluationResult(int tickAge)
    {
        if (currBotRun == null)
        {
            throw new System.Exception("You must first call `AddBotRunEvaluationResult` before calling this method");
        }
        return currBotRun.AddNewNNetEvaluationResult(tickAge);
    }

    public string saveStateDir{
        get{
            return fileLoc + "/fidelity";
        }
    }

    public static FidelityCheckData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<FidelityCheckData>(jsonString);
    }
    public static FidelityCheckData LoadFromLocal(string gameDataFileName){
        string filePath = gameDataFileName + "/fidelity.json";

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot load game data: " + filePath);
            return null;
        }
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(filePath);
        FidelityCheckData data =  CreateFromJSON(dataAsJson);
        //trainingRoomData.fileLoc = filePath;
        return data;

    }

    public void Save()
    {

        string dataAsJson = JsonUtility.ToJson(this);

        System.IO.Directory.CreateDirectory(this.fileLoc);
        File.WriteAllText(this.fileLoc + "/fidelity.json", dataAsJson);
    }
    public FidelityCompareResult Test(){
        if(botRunResults.Count < 2){
            throw new System.Exception("Not enough `botRunResults` to compare(<2)");
        }
        FidelityCompareResult fidelityCompareResult = new FidelityCompareResult();
        foreach(BotRunEvaluationResult botRunEvaluationResult1 in botRunResults){
            foreach (BotRunEvaluationResult botRunEvaluationResult2 in botRunResults)
            {
                if(botRunEvaluationResult1.spawnCount != botRunEvaluationResult2.spawnCount){
                    FidelityCompareBotRunResult botRunResult = botRunEvaluationResult1.CompairTo(botRunEvaluationResult2);
                    if (botRunResult.diffTicks.Count > 0)
                    {
                        fidelityCompareResult.botRunDiffs.Add(botRunResult);
                    }
                }
            }
        }
        return fidelityCompareResult;
    }
	


}