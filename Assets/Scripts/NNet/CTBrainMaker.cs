using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 

public class CTBrainMaker : BrainMaker
{

    //public static BrainMakerConfigData STATIC_brainMakerConfigData;
    /*
     *  STATIC
     */
   


    /*
     * Instance
     */
  
 
    public override void Init(BrainMakerConfigData _brainMakerConfigData, TrainBasicGameMode _gameMode){
      
        base.Init(_brainMakerConfigData, _gameMode);

        //STATIC_brainMakerConfigData = new BrainMakerConfigData();
        outputNeuronTypes.Add("ScoreTargetOutput");
        /*outputNeuronTypes.Add("TurnOutput");
        outputNeuronTypes.Add("StopOutput");
        outputNeuronTypes.Add("RememberOutput");*/

        //inputNeuronTypes.Add("DebugInput");
        inputNeuronTypes.Add("EntityStatInput");
/*        inputNeuronTypes.Add("CanSeeTileInput");
        inputNeuronTypes.Add("CanRememberInput");
        inputNeuronTypes.Add("OnEnterTileInput");
        inputNeuronTypes.Add("IsFacingAngleInput");*/
    }

    public override void SetupNewSpeciesBasicInputOutputs(NNet nNet){

        
        // BotBiologyData botBiologyData = GameManager.instance.trainingRoomData.botBiologyData;
        //TODO:Figure out how to better populate better bot biology



  
        ScoreTargetOutput scoreTargetOutputNavigateTo = new ScoreTargetOutput("output_" + nNet.generation + "_" + nNet.neurons.Count);
        scoreTargetOutputNavigateTo.ActionType = typeof(NavigateToAction);
        nNet.neurons.Add(scoreTargetOutputNavigateTo.id, scoreTargetOutputNavigateTo);
        nNet.outputNeurons.Add(scoreTargetOutputNavigateTo.id, scoreTargetOutputNavigateTo);
        scoreTargetOutputNavigateTo.AttachNNet(nNet);
        
        ScoreTargetOutput scoreTargetOutputUsePrimaryItem = new ScoreTargetOutput("output_" + nNet.generation + "_" + nNet.neurons.Count);
        scoreTargetOutputUsePrimaryItem.ActionType = typeof(UsePrimaryItemAction);
        nNet.neurons.Add(scoreTargetOutputUsePrimaryItem.id, scoreTargetOutputUsePrimaryItem);
        nNet.outputNeurons.Add(scoreTargetOutputUsePrimaryItem.id, scoreTargetOutputUsePrimaryItem);
        scoreTargetOutputUsePrimaryItem.AttachNNet(nNet);
        
        
        
        
        BiasInput biasInput = new BiasInput("input_" + nNet.generation + "_" + nNet.neurons.Count);
        nNet.neurons.Add(biasInput.id, biasInput);
        nNet.inputNeurons.Add(biasInput.id, biasInput);
        biasInput.AttachNNet(nNet);
        
        
        foreach (ChaosEntityStatType statType in (ChaosEntityStatType[]) Enum.GetValues(typeof(ChaosEntityStatType)))
        {
            EntityStatInput entityStatInput = new EntityStatInput("input_" + nNet.generation + "_" + nNet.neurons.Count);
            entityStatInput.StatType = statType;
            nNet.neurons.Add(entityStatInput.id, entityStatInput);
            nNet.inputNeurons.Add(entityStatInput.id, entityStatInput);
            entityStatInput.AttachNNet(nNet);
        }


    }


    public override OutputNeuron InitializeOutputNeuron(string id, string outputNeuronType){

        switch(outputNeuronType){
            case("ScoreTargetOutput"):
                ScoreTargetOutput output = new  ScoreTargetOutput(id);
                return output;
            default:
                throw new System.Exception("Invalid OutputNeuron: " + outputNeuronType);
        }
    }
    public override InputNeuron InitializeInputNeuron(string id, string inputNeuronType)
	{

        switch (inputNeuronType)
        {
            case ("EntityStatInput"):
                EntityStatInput entityStatInput = new EntityStatInput(id);
                return entityStatInput;
            case ("DebugInput"):
                DebugInput debugInput = new DebugInput(id);
              
                return debugInput;
            case ("BiasInput"):
                BiasInput biasInput = new BiasInput(id);
                return biasInput;
            /*case ("CanRememberInput"):
                CanRememberInput canRememberInput =new CanRememberInput(id);
                return canRememberInput;

            case ("CanSeeInput"):
                CanSeeInput canSeeInput = new CanSeeInput(id);
                return canSeeInput;

          

            case ("OnEnterTileInput"):
                OnEnterTileInput onEnterTileInput = new  OnEnterTileInput(id);
                return onEnterTileInput;
            case("IsFacingAngleInput"):
                IsFacingAngleInput isFacingAngleInput = new IsFacingAngleInput(id);
                return isFacingAngleInput;
            case ("VelocityInput"):
                VelocityInput velocityInput = new VelocityInput(id);
                return velocityInput;
            case ("AngularVelocityInput"):
                AngularVelocityInput angularVelocityInput = new AngularVelocityInput(id);
                return angularVelocityInput;
            case ("HealthInput"):
                HealthInput healthInput = new HealthInput(id);
                return healthInput;
            case ("HealthChangeInput"):
                HealthChangeInput healthChangeInput = new HealthChangeInput(id);
                return healthChangeInput;

            
            case ("CanHearNoiseInput"):
                CanHearNoiseInput canHearNoiseInput = new CanHearNoiseInput(id);
                return canHearNoiseInput;
            case ("LifespanInput"):
                LifespanInput lifespanInput = new LifespanInput(id);
                return lifespanInput;*/
            default:
                throw new System.Exception("Invalid InputNeuron: " + inputNeuronType);
        }
	}
    /*public List<string> GetEntityListFromCurrTrainingRoom()
    {
        List<string> entities = new List<string>();
        TrainingRoomData trainingRoomData = GameManager.instance.trainingRoomData;
        foreach(TrainingRoomData.EntityData entityData in trainingRoomData.entities){
           
                switch(entityData.type){
                    case("FactoryEntity"):
                        string productPrefabName = entityData.Get("productPrefabName");
                        if (!entities.Contains(productPrefabName))
                        {
                            entities.Add(productPrefabName);
                        }
                    break;
                    default:
                        if (!entities.Contains(entityData.type))
                        {
                            entities.Add(entityData.type);
                        }
                    break;
                
            }
        }
        return entities;
    }
    public List<string> GetTileListFromCurrTrainingRoom()
    {
        List<string> tiles = new List<string>();
        TrainingRoomData trainingRoomData = GameManager.instance.trainingRoomData;
        foreach (TrainingRoomData.Tile tileData in trainingRoomData.tiles)
        {

            switch (tileData.type)
            {
                case ("SpawnTile"):
                case ("Floor1"):
                    //Do nothing
                    break;
                default:
                    if (!tiles.Contains(tileData.type))
                    {
                        tiles.Add(tileData.type);
                    }
                    break;

            }
        }
        return tiles;
    }*/
}
