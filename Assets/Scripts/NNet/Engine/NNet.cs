using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NNet: CTBaseObject 
{
    public int generation = 0;

    protected NPCNNetController npcController;
    public IDictionary<string, BaseNeuron> neurons = new Dictionary<string, BaseNeuron>();
    public IDictionary<string, OutputNeuron> outputNeurons = new Dictionary<string, OutputNeuron>();
    public IDictionary<string, InputNeuron> inputNeurons = new Dictionary<string, InputNeuron>();
    public IDictionary<int, IDictionary<string, HiddenNeuron>> hiddenNeuronLayers = new Dictionary<int, IDictionary<string, HiddenNeuron>>();
    public List<NNetData.NNetMutateHistory> history = new List<NNetData.NNetMutateHistory>();
    public NPCNNetController NPCController
    {
        get { return npcController; }
    }
    public override string _class_name{
        get{
            return "NNet";
        }
    }

    public void LogHistory(NNetData.NNetMutateHistory nNetMutateHistory){
        if(!GameManager.instance.gameConfigData.storeNNetHistory){
            return;
        }
        nNetMutateHistory.generation = generation;
        history.Add(nNetMutateHistory);
    }
    public void InitNeurons(){
        ICollection<string> keys = neurons.Keys;

        foreach (string s in keys)
        {
            BaseNeuron neuron = neurons[s];
            neuron.AttachNNet(this);
            if (typeof(OutputNeuron).IsAssignableFrom(neuron.GetType()))
            {
                outputNeurons.Add(neuron.id, (OutputNeuron)neuron);
            }
            if (typeof(InputNeuron).IsAssignableFrom(neuron.GetType()))
            {
                inputNeurons.Add(neuron.id, (InputNeuron)neuron);
            }
            if (typeof(HiddenNeuron).IsAssignableFrom(neuron.GetType()))
            {
                HiddenNeuron hiddenNeuron = (HiddenNeuron)neuron;
                if(!hiddenNeuronLayers.ContainsKey(hiddenNeuron.layer)){
                //if(hiddenNeuronLayers[hiddenNeuron.layer] == null){
                    hiddenNeuronLayers.Add(hiddenNeuron.layer, new Dictionary<string, HiddenNeuron>());
                }
                hiddenNeuronLayers[hiddenNeuron.layer].Add(neuron.id, hiddenNeuron);
            }
        }
    }
    public void AttachBotController(NPCNNetController npcnNetController){
        npcController = npcnNetController;
        ICollection<string> keys = neurons.Keys;

        foreach (string s in keys)
        {
            BaseNeuron neuron = neurons[s];
            neuron.AttachNNet(this);
           
        }


    }
    public void OrigenCheck(){
        foreach (string s in neurons.Keys)
        {
            BaseNeuron neuron = neurons[s];
            neuron.AttachNNet(this);
            if (neuron.origenNNetId == null)
            {
              
                neuron.origenNNetId = NPCController.id;
                neuron.origenSpecies = NPCController.speciesObject.id;
               
            }
            foreach (NeuronDep neuronDep in neuron.dependencies)
            {
                if (neuronDep.origenNNetId == null)
                {
                    neuronDep.origenNNetId = NPCController.id;
                    neuronDep.origenSpecies = NPCController.speciesObject.id;
                }
            }
        }
    }
    public List<OutputNeuron> EvaluateNeurons(){

        /*
        EvaluateNeuronLayer(BrainMaker.ConvertToGenericNeuronLayer(inputNeurons));
        for (int i = 0; i < hiddenNeuronLayers.Count; i++)
        {
            EvaluateNeuronLayer(BrainMaker.ConvertToGenericNeuronLayer(hiddenNeuronLayers[i]));
        }
        EvaluateNeuronLayer(BrainMaker.ConvertToGenericNeuronLayer(outputNeurons));
*/
        foreach(BaseNeuron baseNeuron in neurons.Values){
            baseNeuron.hasBeenEvaluatedThisTick = false;
        }

        foreach(OutputNeuron outputNeuron in outputNeurons.Values){
            outputNeuron.Evaluate();
        }



        List<OutputNeuron> firingOutputs = new List<OutputNeuron>();
        /*if (GameManager.instance.trainingRoomData.brainMakerConfigData.fireOnlyTopNeuron)
        {
            OutputNeuron highestScoringNeuron = null;
            foreach (string neuronId in outputNeurons.Keys)
            {
                if (highestScoringNeuron == null)
                {
                    highestScoringNeuron = outputNeurons[neuronId];
                }
                else
                {
                    //Debug.Log(neuronId + " - " + outputNeurons[neuronId].lastValue);
                    if (outputNeurons[neuronId].lastValue > highestScoringNeuron.lastValue)
                    {

                        highestScoringNeuron = outputNeurons[neuronId];
                    }
                }
            }
            firingOutputs.Add(highestScoringNeuron);
        }else{*/
            foreach (string neuronId in outputNeurons.Keys)
            {
                if(!outputNeurons[neuronId].lastValue.Equals(0)){
                    firingOutputs.Add(outputNeurons[neuronId]);
                }
            }
        // }
        return firingOutputs;


    }
    public void EvaluateNeuronLayer(IDictionary<string, BaseNeuron> neuronLayer){
        foreach(string neuronId in neuronLayer.Keys){
            neuronLayer[neuronId].Evaluate();
        }
    }
    public NNetData GetSerializer()
	{
        NNetData nNetData = new NNetData();
        if (NPCController != null)
        {
            nNetData.id = NPCController.id;
        }
        nNetData.generation = generation;
        //nNetData.parent = parent;

        foreach (string neuronId in neurons.Keys)
        {
            nNetData.neurons.Add(neurons[neuronId].ToData());
        }
        nNetData.history = history;
        return nNetData;
	}
    public BaseNeuron RandomNeuron()
    {
        return RandomNeuron(null);
    }
    public BaseNeuron RandomNeuron(List<string> types){
        List<string> neuronKeys = new List<string>();
        foreach (string key in neurons.Keys)
        {
            if (types == null)
            {
                neuronKeys.Add(key);
            }else{
                bool matchesTypes = false;
                foreach(string type in types){
                    if (neurons[key]._base_type == type)
                    {
                        matchesTypes = true;
                    }
                }
                if(matchesTypes){
                    neuronKeys.Add(key);
                }

            }
        }
       
        int range = Random.Range(0, neuronKeys.Count);
        BaseNeuron baseNeuron = neurons[neuronKeys[range]];
        return baseNeuron;
    }
	//public override void OnDestroy()
    ~NNet()
	{
        //base.OnDestroy();
        /*
        for (int i = 0; i < neurons.Count; i++)
        {
            UnityEngine.Object.Destroy(neurons.ElementAt(i).Value);
        }
        */
        neurons.Clear();
        neurons = null;
        outputNeurons.Clear();
        outputNeurons = null;
        inputNeurons.Clear();
        inputNeurons = null;
        hiddenNeuronLayers.Clear();
        hiddenNeuronLayers = null;
        npcController = null;
	}
	/* 
	public override void Dispose()
	{
        base.Dispose();

        for (int i = 0; i < neurons.Count; i++){
            neurons.ElementAt(i).Value.Dispose();
        }
	}*/

}
