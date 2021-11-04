using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class BrainMakerConfigData: BaseData  {

    /*

    public int adjustNeuronDepWeightChance = 80;
    public int setNewNeuronDepWeightChance = 10;
    public int addNewNeuronDepChance = 5;
    public int addNewNeuronChance = 5;
    public int adjustBiasNeuronChance = 10;
    public int toggleNeuronDepEnabledChance = 20;

    private int setNewNeuronDepWeightChance_ceil;
    private int addNewNeuronDepChance_ceil;
    private int addNewNeuronChance_ceil;
    private int adjustBiasNeuronChance_ceil;
    private int toggleNeuronDepEnabledChance_ceil;


    public int EVOLVE_ACTION_breedChance = 10;
    public int EVOLVE_ACTION_mutateChance = 90;
    private int EVOLVE_ACTION_mutateChance_ceil;

    public float BREED_ACTION_interSpecies = 1;
    */




    public BrainMakerMutationRateData mutationRateData = new BrainMakerMutationRateData();

    public string fileLoc;
    public bool inputBasicMovment = false;
    public bool outputBasicMovment = true;
    public bool ioBasicMemory = false;
    public bool ioBasicNoise = false;
    public bool inputHighPitchNoise = false;

    public bool inputBasicVision = true;
    public bool inputBasicSelfAwearness = false;

    public bool botTileMemoryActive = false;
    public float accellerateMutationAsSpeciesGetsStaleRate = -1;


    public decimal backPropigationRate = 0;
    public bool neuralPlasticity = false;
    public bool lstmLogicEnabled = false;

    public int NEATStartNeuronDepCount = 3;
    public int NEATMutationsPerGeneration = 1;
    public float NEATMaxGenomeDiffBeforeNewSpecies = .5f;//TODO: Change this


  


    public bool useBinaryNeuronOutput = false;
    public bool useBinaryNeuronInput = false;

	internal BrainMakerMutationRateData GetNewSpeciesMutationRateData()
	{
        string jsonData = JsonUtility.ToJson(mutationRateData);
        BrainMakerMutationRateData brainMakerMutationRateData = JsonUtility.FromJson<BrainMakerMutationRateData>(jsonData);
        return brainMakerMutationRateData;
	}




	//Other
	public bool fireOnlyTopNeuron = false;


    public List<int> hiddenLayerNeuronCounts = new List<int>{
        16,
        16
    };

    //public int outputNeuronCount = 5;
    //public int inputNeuronCount = 5;


    public BrainMakerConfigData():base()
    {
        
    }
    public override string _class_name
    {
        get
        {
            return "BrainMakerConfigData";
        }
    }
    /*
    public void RefreshOdds(){
        setNewNeuronDepWeightChance_ceil = adjustNeuronDepWeightChance + setNewNeuronDepWeightChance;
        addNewNeuronDepChance_ceil = setNewNeuronDepWeightChance_ceil + addNewNeuronDepChance;
        addNewNeuronChance_ceil = addNewNeuronDepChance_ceil + addNewNeuronChance;
        adjustBiasNeuronChance_ceil = addNewNeuronChance_ceil + adjustBiasNeuronChance;
        toggleNeuronDepEnabledChance_ceil = adjustBiasNeuronChance_ceil + toggleNeuronDepEnabledChance;

        EVOLVE_ACTION_mutateChance_ceil = EVOLVE_ACTION_breedChance + EVOLVE_ACTION_mutateChance;
    }
    public string GetRandomEvolveAction(){
        int chance = Random.Range(
            1,
            EVOLVE_ACTION_breedChance + EVOLVE_ACTION_mutateChance
        );
        if (chance > 0 && chance <= EVOLVE_ACTION_breedChance)
        {
            return BrainMaker.BrainMakerEvolveAction.BREED;
        }
        if (chance > EVOLVE_ACTION_breedChance && chance <= EVOLVE_ACTION_mutateChance_ceil)
        {
            return BrainMaker.BrainMakerEvolveAction.MUTATE;
        }
       
        throw new System.Exception("Something is wrong with your math");
    }
    public string GetRandomMutationAction(){
        int chance = Random.Range(
            1,
            toggleNeuronDepEnabledChance_ceil
        );
        if(chance > 0 && chance <= adjustNeuronDepWeightChance){
            return BrainMaker.BrainMakerMutationAction.ADJUST_NEURON_DEP_WEIGHT;
        }
        if (chance > adjustNeuronDepWeightChance && chance <= setNewNeuronDepWeightChance_ceil)
        {
            return BrainMaker.BrainMakerMutationAction.SET_NEW_NEURON_DEP_WEIGHT;
        }
        if (chance > setNewNeuronDepWeightChance_ceil && chance <= addNewNeuronDepChance_ceil)
        {
            return BrainMaker.BrainMakerMutationAction.ADD_NEW_NEURON_DEP;
        }
        if (chance > addNewNeuronDepChance_ceil && chance <= addNewNeuronChance_ceil)
        {
            return BrainMaker.BrainMakerMutationAction.ADD_NEW_NEURON;
        }



        if (chance > addNewNeuronChance_ceil && chance <= adjustBiasNeuronChance_ceil)
        {
            return BrainMaker.BrainMakerMutationAction.ADJUST_BIAS_NEURON;
        }
        if (chance > adjustBiasNeuronChance_ceil && chance <= toggleNeuronDepEnabledChance_ceil)
        {
            return BrainMaker.BrainMakerMutationAction.TOGGLE_NEURON_DEP_ENABLED;
        }
        throw new System.Exception("Something is wrong with your math");
    }
    */
    public static BrainMakerConfigData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<BrainMakerConfigData>(jsonString);
    }
    public static BrainMakerConfigData LoadFromLocal(string gameDataFileName){
        string filePath = gameDataFileName;

        if (!File.Exists(filePath))
        {
            Debug.LogError("Cannot load game data: " + filePath);
            return null;
        }
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(filePath);
        BrainMakerConfigData brianMakerConfigData =  CreateFromJSON(dataAsJson);
        brianMakerConfigData.fileLoc = filePath;
        return brianMakerConfigData;

    }

    public void Save()
    {

        string dataAsJson = JsonUtility.ToJson(this);

        string filePath = this.fileLoc;
        File.WriteAllText(filePath, dataAsJson);

    }

}