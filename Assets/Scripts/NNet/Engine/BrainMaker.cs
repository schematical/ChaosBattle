using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.




public abstract class BrainMaker : CTBaseObject  {
    protected BrainMakerConfigData brainMakerConfigData;
    protected TrainBasicGameMode gameMode;

    public class Action{
        public NPCNNetController ParentNpcnNetController;
        public NNet resultNNet;
        public string evolveAction;
        public string mutateAction;
        public BrainMakerMutationRateData mutationRateData;
		public float parentNNetSimilarityScore;
        public bool flagAsFailedToEvolve = false;

		public string _class_name
        {
            get
            {
                return "BrainMaker_Config";
            }
        }
        public Action()
        {
            GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
        }
     
        ~Action()
        {
            //System.Threading.Interlocked.Decrement(ref counter);
            GameManager.instance.garbageCollector.DeregisterObject(_class_name);
        }

    }
    public class BrainMakerMutationAction{
        public const string ADJUST_NEURON_DEP_WEIGHT = "ADJUST_NEURON_DEP_WEIGHT";
        public const string SET_NEW_NEURON_DEP_WEIGHT = "SET_NEW_NEURON_DEP_WEIGHT";
        public const string ADD_NEW_NEURON_DEP = "ADD_NEW_NEURON_DEP";
        public const string ADD_NEW_NEURON = "ADD_NEW_NEURON";
        public const string ADJUST_BIAS_NEURON = "ADJUST_BIAS_NEURON";
        public const string TOGGLE_NEURON_DEP_ENABLED = "TOGGLE_NEURON_DEP_ENABLED";
    }

    public class BrainMakerEvolveAction
    {
        public const string BREED = "BREED";
        public const string MUTATE = "MUTATE";

    }


    private static int globalInnovationNumber = 0;
    public static int IncInnovationNumber()
    {
        int _innovationNumber = globalInnovationNumber;
        globalInnovationNumber += 1;
        return _innovationNumber;
    }
    protected static NNetData globalBaseNNetData;
  

    protected List<string> outputNeuronTypes = new List<string>();
    protected List<string> inputNeuronTypes = new List<string>();
    //protected BotController parentBotController1 = null;
    public virtual void Init(BrainMakerConfigData _brainMakerConfigData, TrainBasicGameMode _trainBasicGameMode){
        brainMakerConfigData = _brainMakerConfigData;
        gameMode = _trainBasicGameMode;
        if (globalBaseNNetData == null)
        {
            NNet baseNNet = new NNet();
            SetupNewSpeciesBasicInputOutputs(baseNNet);
            globalBaseNNetData = baseNNet.GetSerializer();
        }

    }
    /*public void SetParentBotController1(BotController botController){
        parentBotController1 = botController;
    }*/
    public override string _class_name
    {
        get
        {
            return "BrainMaker";
        }
    }

   
    public void PopulateHiddenNeurons(NNet nNet)
    {
        for (int i = 0; i < brainMakerConfigData.hiddenLayerNeuronCounts.Count; i++)
        {
            IDictionary<string, HiddenNeuron> layer = new Dictionary<string, HiddenNeuron>();
            if (!nNet.hiddenNeuronLayers.ContainsKey(i))
            {
                nNet.hiddenNeuronLayers.Add(i, layer);
            }
            for (int ii = nNet.hiddenNeuronLayers[i].Count; ii < brainMakerConfigData.hiddenLayerNeuronCounts[i]; ii++)
            {
                HiddenNeuron hiddenNeuron = new HiddenNeuron("hidden_" + nNet.generation + "_" + i + "_" + ii, i);
                nNet.neurons.Add(hiddenNeuron.id, hiddenNeuron);
                layer.Add(hiddenNeuron.id, hiddenNeuron);
                hiddenNeuron.AttachNNet(nNet);

            }
        }
    }
  
    public static IDictionary<string, BaseNeuron> ConvertToGenericNeuronLayer(IDictionary<string, OutputNeuron> neurons){
        Dictionary<string, BaseNeuron> finishedNeurons = new Dictionary<string, BaseNeuron>();
        foreach(string key in neurons.Keys){
            finishedNeurons.Add(key, (BaseNeuron)neurons[key]);
        }
        return finishedNeurons;
    }
    public static IDictionary<string, BaseNeuron> ConvertToGenericNeuronLayer(IDictionary<string, InputNeuron> neurons)
    {
        Dictionary<string, BaseNeuron> finishedNeurons = new Dictionary<string, BaseNeuron>();
        foreach (string key in neurons.Keys)
        {
            finishedNeurons.Add(key, (BaseNeuron)neurons[key]);
        }
        return finishedNeurons;
    }
    public static IDictionary<string, BaseNeuron> ConvertToGenericNeuronLayer(IDictionary<string, HiddenNeuron> neurons)
    {
        Dictionary<string, BaseNeuron> finishedNeurons = new Dictionary<string, BaseNeuron>();
        foreach (string key in neurons.Keys)
        {
            finishedNeurons.Add(key, (BaseNeuron)neurons[key]);
        }
        return finishedNeurons;
    }
   
    public NNet ParseNNetData(NNetData nNetData)
    {
        return ParseNNetData(nNetData, null);
    }
    public NNet ParseNNetData(NNetData nNetData, NNet nNet)
    {
        if (nNet == null)
        {
            nNet = new NNet();//new NNet();
        }
        //First setup neurons
        nNet.generation = nNetData.generation;
        nNet.history.Clear();
        foreach(NNetData.NNetMutateHistory historyData in nNetData.history){
            nNet.history.Add(historyData);
        }


        foreach (NNetData.NeuronData neuronData in nNetData.neurons)
        {
            BaseNeuron neuron = ParseNNetDataNeuron(neuronData);  
            neuron.AttachNNet(nNet);
            if(nNet.neurons.ContainsKey(neuron.id)){
                throw new System.Exception("Duplicate Nuron: " + neuron.id);
            }
            nNet.neurons.Add(neuron.id, neuron);
        }
        //Second Add Dependencies
        foreach (NNetData.NeuronData neuronData in nNetData.neurons)
        {
            nNet.neurons[neuronData.id].ParseData(neuronData);
        }
        nNet.InitNeurons();
        return nNet;

    }
    public BaseNeuron ParseNNetDataNeuron(NNetData.NeuronData nNetData){
        BaseNeuron neuron = null;
        switch (nNetData._base_type)
        {
            case ("input"):
                neuron = InitializeInputNeuron(nNetData.id, nNetData.type);
                break;
            case ("output"):
                neuron = InitializeOutputNeuron(nNetData.id, nNetData.type);
                break;
            case ("hidden"):
                neuron = new HiddenNeuron(nNetData.id, int.Parse(nNetData.Get("layer")));

                break;
        }
        return neuron;
    }

    public OutputNeuron RndOutput(string id)
    {
        int index = Random.Range(0, outputNeuronTypes.Count);

        string outputNeuronType = outputNeuronTypes[index];
        OutputNeuron outputNeuron = InitializeOutputNeuron(id, outputNeuronType);
        outputNeuron.PopulateRandom();
        return outputNeuron;


    }
    public InputNeuron RndInput(string id)
    {
        int inputIndex = Random.Range(0, inputNeuronTypes.Count);
        string inputNeuronType = inputNeuronTypes[inputIndex];

        InputNeuron inputNeuron = InitializeInputNeuron(id, inputNeuronType);
        inputNeuron.PopulateRandom();
        return inputNeuron;
    }
    /*
    public virtual void PopulateStratigicNNet(NNet nNet)
    {
        if (nNet.generation == 0)
        {
            SetupNewSpeciesBasicInputOutputs(nNet);

            //inputNeuronCount = nNet.inputNeurons.Count;
            PopulateHiddenNeurons(nNet);

        }
        else
        {
            DecayNeuronDependants(nNet);
        }
        //maxDepentants = brainMakerConfigData.maxDepentants + nNet.generation;
        PopulateRandomDependants(nNet);


    }*/

    public virtual Action Populate(Action brainMakerAction){
        //TODO: Add switch case for generation methods
        return PopulateNEATNNet(brainMakerAction);
    }
    public virtual Action PopulateNEATNNet(Action brainMakerAction){
        //Connect X dependants
        //brainMakerAction.resultNNet = null;
        if(brainMakerAction.ParentNpcnNetController == null){
          
       
            brainMakerAction.resultNNet = new NNet();//new NNet();
            brainMakerAction.resultNNet = ParseNNetData(globalBaseNNetData, brainMakerAction.resultNNet);
            brainMakerAction.resultNNet.history = new List<NNetData.NNetMutateHistory>();
            PopulateNEATDependants(brainMakerAction);

        }else{
            //TODO: Mutate some stuff
            System.Exception exception = new System.Exception("BlankException");
            float saftyCatch = 0;
            while (
                exception != null && 
                saftyCatch < 20
            )
            {
                exception = null;
                saftyCatch += 1;
                try
                {

                    brainMakerAction.evolveAction = brainMakerAction.mutationRateData.GetRandomEvolveAction();//BrainMakerEvolveAction.MUTATE;

                    NNetData.NNetMutateHistory testHistoryData = new NNetData.NNetMutateHistory();
                   
                    testHistoryData.type = "EVLOVING_" + brainMakerAction.evolveAction;
                    if(brainMakerAction.resultNNet == null){
                        Debug.LogError("Missing `brainMakerAction.resultNNet`");
                    }
                    brainMakerAction.resultNNet.LogHistory(testHistoryData);
                    switch (brainMakerAction.evolveAction)
                    {

                        case (BrainMakerEvolveAction.BREED):
                            NNet newNNet = BreedNEATNet(brainMakerAction);
                            if (newNNet == null)
                            {

                                MutateNEATNNet(brainMakerAction);
                            }
                            else
                            {
                                //ScriptableObject.Destroy(brainMakerAction.resultNNet);
                                brainMakerAction.resultNNet = newNNet;
                            }
                            break;
                        case (BrainMakerEvolveAction.MUTATE):
                            int mutationCount = brainMakerConfigData.NEATMutationsPerGeneration;
                            if(!brainMakerConfigData.accellerateMutationAsSpeciesGetsStaleRate.Equals(-1)){
                                mutationCount += (int)Mathf.Round(
                                    brainMakerConfigData.NEATMutationsPerGeneration * 
                                    brainMakerAction.ParentNpcnNetController.speciesObject.generationsSinceLastImprovment *
                                    brainMakerConfigData.accellerateMutationAsSpeciesGetsStaleRate
                                );
                               
                            }
                                
                            for (int i = 0; i < mutationCount; i++)
                            {
                                MutateNEATNNet(brainMakerAction);
                            }
                            break;
                        default:
                            throw new System.Exception("Invalid Evolve Action:" + brainMakerAction.evolveAction);
                    }
                }catch(System.Exception e){
                    exception = e;
                    throw e;
                    //Debug.LogError("Mutation Error: " + exception.ToString() + " - Try: " + saftyCatch);
                }

            }
        }
        return brainMakerAction;
    }
    public void PopulateNEATDependants(Action brainMakerAction){
        int saftyCatch = 0;
        int neuronConnections = 0;
        while (
            saftyCatch < 20 && 
            neuronConnections < brainMakerConfigData.NEATStartNeuronDepCount
        )
        {
          
            BaseNeuron outputNeuron = brainMakerAction.resultNNet.RandomNeuron(new List<string> { "output" });
            BaseNeuron inputNeuron = brainMakerAction.resultNNet.RandomNeuron(new List<string> { "input" });
            foreach(NeuronDep _neuornDep in outputNeuron.dependencies){
                if(_neuornDep.depNeuron.id == inputNeuron.id){
                    outputNeuron = null;
                    //inputNeuron = null;
                }  
            }

            if (
                outputNeuron != null &&
                inputNeuron != null
            )
            {
                decimal weight = (decimal)(Mathf.Round(Random.Range(-1 * brainMakerAction.mutationRateData.setNewNeuronDepWeightRate, brainMakerAction.mutationRateData.setNewNeuronDepWeightRate)) * .01f);
                outputNeuron.AddDep(inputNeuron, weight);
                neuronConnections += 1;

                NNetData.NNetMutateHistory historyData = new NNetData.NNetMutateHistory();
                historyData.neuron1 = outputNeuron.id;
                historyData.neuron2 = inputNeuron.id;
                historyData.type = "OriginalPopulate";
                brainMakerAction.resultNNet.LogHistory(historyData);
            }

            saftyCatch += 1;
        }
        if(saftyCatch >= 20){
            throw new System.Exception("Safty Catch met during `PopulateNEATDependants`");
        }

        //Also randomize the Bias

        MutateAdjustBiasNeuron(brainMakerAction, 100);
    }
    public NNet BreedNEATNet(Action brainMakerAction){
        //Get 2 random species
        if(brainMakerAction.ParentNpcnNetController == null){
            throw new System.Exception("`parentBotController1` is null");   
        }
       
        SpeciesObject speciesObject1 = brainMakerAction.ParentNpcnNetController.speciesObject;
        SpeciesObject speciesObject2 = null;
        BreedAction breedAction = new BreedAction();
        int saftyCount = 0;
        bool breedInterspecies = false;
        if(Random.Range(0, 100)  <= brainMakerAction.mutationRateData.BREED_ACTION_interSpecies){
            breedInterspecies = true;
        }
        while (
            speciesObject2 == null && 
            saftyCount < 10
        )
        {
            saftyCount += 1;
            //speciesObject1 = GameManager.instance.speciesManager.GetRandom();
            if(!breedInterspecies){
                speciesObject2 = speciesObject1;
            }else{
                speciesObject2 = gameMode.speciesManager.GetRandom();
            }

            if (
                (breedInterspecies && speciesObject1.id == speciesObject2.id) ||
                (!breedInterspecies && speciesObject1.id != speciesObject2.id) 
            ){
                //speciesObject1 = null;
                speciesObject2 = null;

            }
            else
            {
                //BotController botController1 = speciesObject1.GetRandomHighScoringBotController();
                NPCNNetController npcnNetController2 = speciesObject2.GetRandomHighScoringBotController();
                if (
                    npcnNetController2 == null ||
                    npcnNetController2.id == brainMakerAction.ParentNpcnNetController.id
                    //||

                    //botController2.ageTicks < gameMode.fitnessManagerConfigData.speciesMaxTurnsToOptimizeBeforeSubjectToExtinction
                )
                {
                    speciesObject2 = null;
                }
                else
                {
                    breedAction.AddParent(brainMakerAction.ParentNpcnNetController);
                    breedAction.AddParent(npcnNetController2);
                    /*BreedAction.CompareResult compareResult = breedAction.Compare();
                    if (compareResult.score.Equals(0))
                    {
                        breedAction.
                    }
                    */
                    //Debug.Log("Breeding: " + brainMakerAction.parentBotController.id + " and " + botController2.id + " SIMILARITIES: " + brainMakerAction.parentBotController.CompareWithBot(botController2).score);
                }
            }


        }


        if(saftyCount >= 10){
            // throw new System.Exception("SaftyCount met unable to find a bot to breed with");
            return null;
        }

          

        breedAction.Populate();
        if(breedAction.parentBots.Count != 2){
            throw new System.Exception("Incorrect number of breedAction parents");
        }
        //TODO: DO stuff...
        NNet nNet = breedAction.GenerateNNet();
        if(breedAction.similarityScore.Equals(0)){
            //Mutate it
            MutateNEATNNet(brainMakerAction);
        }

        NNetData.NNetMutateHistory historyData = new NNetData.NNetMutateHistory();
        historyData.neuron1 = speciesObject1.id;
        historyData.neuron2 = speciesObject2.id;
        historyData.type = "BREED";
        nNet.LogHistory(historyData);

        //UnityEngine.Object.Destroy(breedAction);
        breedAction = null;
        return nNet;
        
    }
    public void MutateNEATNNet(Action brainMakerAction){
        brainMakerAction.mutateAction = brainMakerAction.mutationRateData.GetRandomMutationAction();
        //Get a random neuron
        BaseNeuron baseNeuron = null;
        List<string> selectorList = new List<string>{
            BaseNeuron.BaseType.output,
            BaseNeuron.BaseType.hidden
        };
        while (baseNeuron == null || baseNeuron.dependencies.Count == 0)
        {
            baseNeuron = brainMakerAction.resultNNet.RandomNeuron(
               selectorList
            );
        }

        int range = Random.Range(0, baseNeuron.dependencies.Count);
      

        NeuronDep neuronDep = baseNeuron.dependencies[range];
       
        switch(brainMakerAction.mutateAction){
            case(BrainMakerMutationAction.ADD_NEW_NEURON):

                BaseNeuron connectionNeuron = neuronDep.depNeuron;
                //Get a random neuron dep
                int whileLoopSaftyCheck = 0;
                while (
                   (
                        (
                            baseNeuron._base_type == BaseNeuron.BaseType.hidden ||
                            connectionNeuron._base_type == BaseNeuron.BaseType.hidden
                        ) ||
                        baseNeuron.dependencies.Count == 0
                    )
                    &&
                    whileLoopSaftyCheck < 30
                ){
                    whileLoopSaftyCheck += 1;
                    baseNeuron = brainMakerAction.resultNNet.RandomNeuron(
                        new List<string>{
                            BaseNeuron.BaseType.output,
                        }
                    );

                    if (baseNeuron.dependencies.Count > 0)
                    {

                        range = Random.Range(0, baseNeuron.dependencies.Count);
                        if (baseNeuron.dependencies.Count <= range)
                        {
                            throw new System.Exception("Invalid Range: " + range);
                        }
                        try
                        {
                            neuronDep = baseNeuron.dependencies[range];
                        }
                        catch (System.Exception exe)
                        {
                            Debug.Log("Invalid Range 2: " + range.ToString() + " - Message:" + exe.Message);
                        }
                        connectionNeuron = neuronDep.depNeuron;
                    }

                }
                if (whileLoopSaftyCheck >= 20)
                {
                    Debug.LogError("ERROR: Expanded beyond whileLoopSaftCheck");
                }
                if(
                    baseNeuron._base_type == BaseNeuron.BaseType.hidden ||
                    connectionNeuron._base_type == BaseNeuron.BaseType.hidden
                ){
                    throw new System.Exception("Double Middle Check Failed :(");
                }
               
                int neuronLayerIndex = 0;
                if (!brainMakerAction.resultNNet.hiddenNeuronLayers.ContainsKey(neuronLayerIndex))
                {
                    brainMakerAction.resultNNet.hiddenNeuronLayers.Add(neuronLayerIndex, new Dictionary<string, HiddenNeuron>());
                }
                HiddenNeuron hiddenNeuron = new HiddenNeuron("hidden_" + brainMakerAction.resultNNet.generation + "_" + 0 + "_" + brainMakerAction.resultNNet.hiddenNeuronLayers[neuronLayerIndex].Count, neuronLayerIndex);
                brainMakerAction.resultNNet.neurons.Add(hiddenNeuron.id, hiddenNeuron);
                brainMakerAction.resultNNet.hiddenNeuronLayers[neuronLayerIndex].Add(hiddenNeuron.id, hiddenNeuron);
                hiddenNeuron.AttachNNet(brainMakerAction.resultNNet);
                hiddenNeuron.origenGen = brainMakerAction.resultNNet.generation;

                hiddenNeuron.origenNNetId = brainMakerAction.ParentNpcnNetController.id;
                hiddenNeuron.origenSpecies = brainMakerAction.ParentNpcnNetController.speciesObject.id;


                //We will have to destroy the neuronDep to ensure a new innovation number is added
                baseNeuron.dependencies.Remove(neuronDep);

                //Create a neuronDep between 1 and 2
                decimal rndWeightAdjustment = (decimal)Random.Range(-1 * brainMakerAction.mutationRateData.adjustNeuronDepRate, brainMakerAction.mutationRateData.adjustNeuronDepRate);
                baseNeuron.AddDep(hiddenNeuron, neuronDep.weight + (rndWeightAdjustment * .01m));

                //2 and 3
                int rnd2WeightAdjustment = Random.Range(-1 * brainMakerAction.mutationRateData.adjustNeuronDepRate, brainMakerAction.mutationRateData.adjustNeuronDepRate);
                NeuronDep newNeuronDep = hiddenNeuron.AddDep(connectionNeuron, neuronDep.weight + (rndWeightAdjustment * .01m));

                newNeuronDep.origenGen = -1;//nNet.generation;
                newNeuronDep.origenNNetId = null;//nNet.bot.id;


                BaseNeuron firstDepNeuron = null;
                whileLoopSaftyCheck = 0;
                while (firstDepNeuron == null && whileLoopSaftyCheck < 20)
                {
                    firstDepNeuron = brainMakerAction.resultNNet.RandomNeuron(
                        new List<string>{
                            BaseNeuron.BaseType.input
                        }
                    );
                    bool hasDepAlready = false;
                    foreach (NeuronDep _neuronDep in hiddenNeuron.dependencies)
                    {
                        if (_neuronDep.depNeuron.id == firstDepNeuron.id)
                        {
                            hasDepAlready = true;
                        }
                    }
                    if (hasDepAlready)
                    {
                        firstDepNeuron = null;
                    }
                }
                if (whileLoopSaftyCheck >= 20)
                {
                    throw new System.Exception("Failed Safty Check");
                    //If this happens we should prob default to adding a neuron
                }
                decimal weightX = (decimal)Mathf.Round(Random.Range(-1 * brainMakerAction.mutationRateData.setNewNeuronDepWeightRate, brainMakerAction.mutationRateData.setNewNeuronDepWeightRate)) * .01m;
                NeuronDep newNeuronDepX = hiddenNeuron.AddDep(firstDepNeuron, weightX);



                NNetData.NNetMutateHistory historyData = new NNetData.NNetMutateHistory();
                historyData.neuron1 = baseNeuron.id;
                historyData.neuron2 = connectionNeuron.id;
                historyData.type = "Mutate_" + brainMakerAction.mutateAction;
                brainMakerAction.resultNNet.LogHistory(historyData);


            break;
            case (BrainMakerMutationAction.ADD_NEW_NEURON_DEP):
                //Randomly select two neurons
                BaseNeuron neuron2 = null;
                int saftyCheck = 0;
                while(neuron2 == null && saftyCheck < 20){
                    saftyCheck += 1;
                    neuron2 = brainMakerAction.resultNNet.RandomNeuron(
                        new List<string>{
                            BaseNeuron.BaseType.input,
                            BaseNeuron.BaseType.hidden
                        }
                    );
                    bool hasDepAlready = false;
                    foreach(NeuronDep _neuronDep in baseNeuron.dependencies){
                        if(_neuronDep.depNeuron.id == neuron2.id){
                            hasDepAlready = true;
                        }
                    }
                    if(hasDepAlready){
                        neuron2 = null;
                    }
                }
                if(saftyCheck >= 20){
                    throw new System.Exception("Failed Safty Check");
                    //return null;
                    //If this happens we should prob default to adding a neuron
                }
                decimal weight = (decimal)Mathf.Round(Random.Range(-1 * brainMakerAction.mutationRateData.setNewNeuronDepWeightRate, brainMakerAction.mutationRateData.setNewNeuronDepWeightRate)) * .01m;
                NeuronDep newNeuronDep2 = baseNeuron.AddDep(neuron2, weight);
             
                neuronDep.origenGen = -1;//nNet.generation;
                neuronDep.origenNNetId = null;//nNet.bot.id;

                historyData = new NNetData.NNetMutateHistory();
                historyData.neuron1 = baseNeuron.id;
                historyData.neuron2 = neuron2.id;
                historyData.type = "Mutate_" + brainMakerAction.mutateAction;
                brainMakerAction.resultNNet.LogHistory(historyData);

            break;
            case (BrainMakerMutationAction.ADJUST_NEURON_DEP_WEIGHT):
                //Get a random neuron dep
                rndWeightAdjustment = 0;
                while (rndWeightAdjustment.Equals(0))
                {
                    rndWeightAdjustment = Random.Range(-1 * brainMakerAction.mutationRateData.adjustNeuronDepRate, brainMakerAction.mutationRateData.adjustNeuronDepRate);
                }
                neuronDep.weight = neuronDep.weight + (rndWeightAdjustment * .01m);
                /*
                if(neuronDep.weight > 1){
                    neuronDep.weight = 1;
                }
                if (neuronDep.weight < -1)
                {
                    neuronDep.weight = -1;
                }
                */
                //Adjust neuronDep weight by .2+/-

                historyData = new NNetData.NNetMutateHistory();
                historyData.neuron1 = baseNeuron.id;
                historyData.type = "Mutate_" + brainMakerAction.mutateAction;
                brainMakerAction.resultNNet.LogHistory(historyData);
            break;
            case (BrainMakerMutationAction.SET_NEW_NEURON_DEP_WEIGHT):

                //Get a random neuron dep

                //Randomly generate a new weight
                weight = neuronDep.weight;
                while (weight.Equals(neuronDep.weight))
                {
                    weight =(decimal)Mathf.Round(Random.Range(-1 * brainMakerAction.mutationRateData.setNewNeuronDepWeightRate, brainMakerAction.mutationRateData.setNewNeuronDepWeightRate)) * .01m;
                }
                neuronDep.weight = weight;
                neuronDep.origenGen = -1;//nNet.generation;
                neuronDep.origenNNetId = null;//nNet.bot.id;

                historyData = new NNetData.NNetMutateHistory();
                historyData.neuron1 = baseNeuron.id;
                historyData.type = "Mutate_" + brainMakerAction.mutateAction;
                brainMakerAction.resultNNet.LogHistory(historyData);
            break;
            case(BrainMakerMutationAction.ADJUST_BIAS_NEURON):

                BiasInput biasInput = MutateAdjustBiasNeuron(brainMakerAction, brainMakerAction.mutationRateData.adjustBiasNeuronRate);
                historyData = new NNetData.NNetMutateHistory();
                historyData.neuron1 = biasInput.id;
                historyData.type = "Mutate_" + brainMakerAction.mutateAction;
                brainMakerAction.resultNNet.LogHistory(historyData);
            break;
            case(BrainMakerMutationAction.TOGGLE_NEURON_DEP_ENABLED):
                
                neuronDep.enabled = !neuronDep.enabled;
                neuronDep.origenGen = -1;//nNet.generation;
                neuronDep.origenNNetId = null;//nNet.bot.id;

                historyData = new NNetData.NNetMutateHistory();
                historyData.neuron1 = baseNeuron.id;
                historyData.neuron2 = neuronDep.depNeuron.id;
                if (neuronDep.enabled)
                {
                    historyData.notes = "State: ENABLED";
                }else{
                    historyData.notes = "State: DISABLED";
                }
                historyData.type = "Mutate_" + brainMakerAction.mutateAction;
                brainMakerAction.resultNNet.LogHistory(historyData);

            break;
            default:
                throw new System.Exception("Invalid Breed Acting: " + brainMakerAction.mutateAction);
        }
    }
    protected BiasInput MutateAdjustBiasNeuron(Action brainMakerAction, float mutationRate){
       
        List<BiasInput> biasInputs = new List<BiasInput>();
        foreach (BaseNeuron _baseNeuron in brainMakerAction.resultNNet.neurons.Values)
        {
            if (_baseNeuron.GetType().Name == "BiasInput")
            {
                biasInputs.Add((BiasInput)_baseNeuron);
            }
        }
        if (biasInputs.Count == 0)
        {
            Debug.LogError("No Bias inputs found ");
            return null;
        }
        int index = Random.Range(0, biasInputs.Count);
        BiasInput biasInput = biasInputs[index];
        decimal rndWeightAdjustment = 0;
        while (rndWeightAdjustment.Equals(0))
        {
            rndWeightAdjustment = (decimal)Random.Range(-1 * mutationRate, mutationRate);
        }
        biasInput.weight += rndWeightAdjustment * 0.01m;

        return biasInput;

    }
    public abstract OutputNeuron InitializeOutputNeuron(string id, string outputNeuronType);
    public abstract InputNeuron InitializeInputNeuron(string id, string inputNeuronType);

    public abstract void SetupNewSpeciesBasicInputOutputs(NNet nNet);
}
