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
        outputNeuronTypes.Add("MoveOutput");
        outputNeuronTypes.Add("TurnOutput");
        outputNeuronTypes.Add("StopOutput");
        outputNeuronTypes.Add("RememberOutput");

        //inputNeuronTypes.Add("DebugInput");
        inputNeuronTypes.Add("CanSeeEntityInput");
        inputNeuronTypes.Add("CanSeeTileInput");
        inputNeuronTypes.Add("CanRememberInput");
        inputNeuronTypes.Add("OnEnterTileInput");
        inputNeuronTypes.Add("IsFacingAngleInput");
    }

    public override void SetupNewSpeciesBasicInputOutputs(NNet nNet){


        // BotBiologyData botBiologyData = GameManager.instance.trainingRoomData.botBiologyData;
        //TODO:Figure out how to beter populate better bot biology



  
        //1 Move
        OutputNeuron outputNeuron = new MoveOutput("output_" + nNet.generation + "_" + nNet.neurons.Count);
        nNet.neurons.Add(outputNeuron.id, outputNeuron);
        nNet.outputNeurons.Add(outputNeuron.id, outputNeuron);
        outputNeuron.AttachNNet(nNet);


        //2 turns(left/right)
        TurnOutput turnOutput = new TurnOutput("output_" + nNet.generation + "_" + nNet.neurons.Count);
        //turnOutput.rot = 90;
        nNet.neurons.Add(turnOutput.id, turnOutput);
        nNet.outputNeurons.Add(turnOutput.id, turnOutput);
        turnOutput.AttachNNet(nNet);

  
 
        CanSeeInput canSeeInput = new CanSeeInput("input_" + nNet.generation + "_" + nNet.neurons.Count);
        canSeeInput.attributeId = CTOA._IS_REAL_TYPE_ + "X";


        nNet.neurons.Add(canSeeInput.id, canSeeInput);
        nNet.inputNeurons.Add(canSeeInput.id, canSeeInput);
        canSeeInput.AttachNNet(nNet);

    }


    public override OutputNeuron InitializeOutputNeuron(string id, string outputNeuronType){

        switch(outputNeuronType){
            case("MoveOutput"):
                MoveOutput output = new  MoveOutput(id);

                return output;
            case ("JumpOutput"):
                JumpOutput jumpOutput = new JumpOutput(id);

                return jumpOutput;
            
            case("TurnOutput"):
                TurnOutput turnOutput = new TurnOutput(id);

                return turnOutput;
            case ("StopOutput"):
                StopOutput stopOutput = new StopOutput(id);

                return stopOutput;

            case ("RememberOutput"):
                RememberOutput rememberOutput = new RememberOutput(id);
                return rememberOutput;
            case ("EmitNoiseOutput"):
                EmitNoiseOutput emitNoiseOutput = new EmitNoiseOutput(id);
                return emitNoiseOutput;
            default:
                throw new System.Exception("Invalid OutputNeuron: " + outputNeuronType);
        }
    }
    public override InputNeuron InitializeInputNeuron(string id, string inputNeuronType)
	{

        switch (inputNeuronType)
        {
            case ("DebugInput"):
                DebugInput debugInput = new DebugInput(id);
              
                return debugInput;

            case ("CanRememberInput"):
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

            case ("BiasInput"):
                BiasInput biasInput = new BiasInput(id);
                return biasInput;
            case ("CanHearNoiseInput"):
                CanHearNoiseInput canHearNoiseInput = new CanHearNoiseInput(id);
                return canHearNoiseInput;
            case ("LifespanInput"):
                LifespanInput lifespanInput = new LifespanInput(id);
                return lifespanInput;
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
