using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;    

[System.Serializable]
public class BrainMakerMutationRateData: BaseData  {

  
    public int adjustNeuronDepRate = 20;
   
    public int setNewNeuronDepWeightRate = 100;
    public int adjustBiasNeuronRate = 50;
   
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




    public BrainMakerMutationRateData():base()
    {
        RefreshOdds();
    }
    public override string _class_name
    {
        get
        {
            return "BrainMakerMutationRateData";
        }
    }
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
        throw new System.Exception("Something is wrong with your math: " + chance + " > " + adjustBiasNeuronChance_ceil);
    }

  

}