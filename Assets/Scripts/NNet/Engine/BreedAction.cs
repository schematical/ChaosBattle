using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BreedAction: CTBaseObject {
    public class CompareResult{
        public float score;
        public string debug;
        public string _class_name
        {
            get
            {
                return "BreedAction_CompareResult";
            }
        }
        public CompareResult()
        {
            GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
        }
     

        ~CompareResult()
        {
            //System.Threading.Interlocked.Decrement(ref counter);
            GameManager.instance.garbageCollector.DeregisterObject(_class_name);
        }
    }
    public class GenomeHolder: CTBaseObject
    {
        public int innovationNumber;

        public BaseNeuron neuron;
        public NeuronDep neuronDep;
        public Dictionary<int, BaseNeuron> baseNeurons = new Dictionary<int, BaseNeuron>();
        public Dictionary<int, NeuronDep> neuronDeps = new Dictionary<int, NeuronDep>();
        public List<int> parentIndexs = new List<int>();
        public bool included = false;
        public float weightDiff = 0;
        public override string _class_name
        {
            get
            {
                return "BreedAction_GenomeHolder";
            }
        }
        //public override void OnDestroy()
        ~GenomeHolder()
        {
            //base.OnDestroy();
            baseNeurons.Clear();
            baseNeurons = null;
            neuronDeps.Clear();
            neuronDeps = null;
            neuron = null;
            neuronDep = null;
            parentIndexs.Clear();

        }

    }
    public class ParentHolder:CTBaseObject{
        
        public NPCNNetController NpcnNetController;
        public int parentIndex;

        public override string _class_name
        {
            get
            {
                return "BreedAction_ParentHolder";
            }
        }
        //public override void OnDestroy(){
        ~ParentHolder()
        {
            //base.OnDestroy();
            NpcnNetController = null;

        }
    }
    public IDictionary<int, ParentHolder> parentBots = new Dictionary<int, ParentHolder>();

    public IDictionary<int, GenomeHolder> innovationGenomes = new Dictionary<int, GenomeHolder>();

    public ParentHolder highParentHolder;
    public float similarityScore;

    public BreedAction()
    {
       
       

    }
    public override string _class_name
    {
        get
        {
            return "BreedAction";
        }
    }
    public void AddParent(NPCNNetController npcnNetController){
        ParentHolder parentHolder = new ParentHolder();

        parentHolder.NpcnNetController = npcnNetController;
        parentHolder.parentIndex = parentBots.Count;

        parentBots.Add(parentHolder.parentIndex, parentHolder);
    }
    public void Populate(){
        //Figure out top parent
        float highParentScore = -1;
        highParentHolder = null;
        //Find high innovation number
        foreach(int parentIndex in parentBots.Keys){

            ParentHolder _parentHolder = parentBots[parentIndex];

            float meanScore = _parentHolder.NpcnNetController.botControllerScoreData.GetMeanScore();
            if (
                highParentScore.Equals(-1) ||
                highParentScore < meanScore
            )
            {
                highParentHolder = _parentHolder;
                highParentScore = meanScore;
            }
       }

        foreach (int parentIndex in parentBots.Keys)
        {
            PopulateAvailableInnovationGenomes(parentBots[parentIndex]);  
        }
       

    }
    void PopulateAvailableInnovationGenomes(ParentHolder parentHolder)
    {
        if(
            parentHolder == null ||
            parentHolder.NpcnNetController == null ||
            parentHolder.NpcnNetController.nNet == null ||
            parentHolder.NpcnNetController.nNet.neurons == null
        ){
            throw new System.Exception("ParentHolder Null somewhere");
        }
        foreach (BaseNeuron baseNeuron in parentHolder.NpcnNetController.nNet.neurons.Values)
        {
            GenomeHolder genomeHolder = null;
            if(!innovationGenomes.ContainsKey(baseNeuron.innovationNumber)){
                genomeHolder = new GenomeHolder();
                genomeHolder.neuron = baseNeuron;
                genomeHolder.included = true;
                genomeHolder.innovationNumber = baseNeuron.innovationNumber;
                innovationGenomes.Add(baseNeuron.innovationNumber, genomeHolder);
            }
            genomeHolder = innovationGenomes[baseNeuron.innovationNumber];
            genomeHolder.baseNeurons.Add(parentHolder.parentIndex, baseNeuron);
            genomeHolder.parentIndexs.Add(parentHolder.parentIndex);
            if (parentHolder.parentIndex == highParentHolder.parentIndex)
            {
                genomeHolder.neuron = baseNeuron;
            }
            foreach(NeuronDep neuronDep in baseNeuron.dependencies){
                genomeHolder = null;
                if (!innovationGenomes.ContainsKey(neuronDep.innovationNumber))
                {
                    genomeHolder = new GenomeHolder();
                    genomeHolder.neuron = baseNeuron;
                    genomeHolder.neuronDep = neuronDep;
                    genomeHolder.included = true;
                    genomeHolder.innovationNumber = neuronDep.innovationNumber;
                    innovationGenomes.Add(neuronDep.innovationNumber, genomeHolder);
                }else{
                    genomeHolder = innovationGenomes[neuronDep.innovationNumber];
                }
                genomeHolder.parentIndexs.Add(parentHolder.parentIndex);
                if(parentHolder.parentIndex == highParentHolder.parentIndex){
                    //genomeHolder.neuron = baseNeuron;
                    genomeHolder.neuronDep = neuronDep;
                }
                genomeHolder.neuronDeps.Add(parentHolder.parentIndex, neuronDep);
            }

        }
        foreach (int key1 in innovationGenomes.Keys)
        {
            GenomeHolder genomeHolder = innovationGenomes[key1];
            if(genomeHolder.neuronDep != null){
                decimal lastWeight = -9999;
                foreach (NeuronDep neuronDep in genomeHolder.neuronDeps.Values)
                {

                    if (lastWeight.Equals(-9999))
                    {
                        lastWeight = neuronDep.weight;
                    }
                    else if (!lastWeight.Equals(neuronDep.weight))
                    {
                        genomeHolder.weightDiff += Mathf.Abs((float)(lastWeight - neuronDep.weight));
                        lastWeight = neuronDep.weight;
                    }

                }
                           
            }
            if (genomeHolder.parentIndexs.Count == parentBots.Count)
            {
                //genomeHolder.included = true;

            }
            else 
            {
                
                if (genomeHolder.neuronDep != null)
                {
                    foreach (int key2 in innovationGenomes.Keys)
                    {
                        GenomeHolder genomeHolder2 = innovationGenomes[key2];
                        if(
                            key1 != key2 &&
                            genomeHolder2.neuronDep != null &&
                            genomeHolder.neuron.id == genomeHolder2.neuron.id &&
                            genomeHolder.neuronDep.depNeuron.id == genomeHolder2.neuronDep.depNeuron.id
                        ){
                            genomeHolder.weightDiff  += Mathf.Abs((float)genomeHolder.neuronDep.weight - (float)genomeHolder2.neuronDep.weight);
                          
                            if(highParentHolder.parentIndex == parentHolder.parentIndex){
                                genomeHolder.included = true;
                                genomeHolder2.included = false;
                            }else{
                                genomeHolder.included = false;
                                genomeHolder2.included = true;
                            }

                           
                        }
                    }
                }
               
            }
        }

    }
    public float GetPercentageMatching(){
        int matchesCount = 0;
        foreach(GenomeHolder genomeHolder in innovationGenomes.Values){
            if(genomeHolder.parentIndexs.Count == parentBots.Count){
                matchesCount += 1;
            }
        }
        return matchesCount/innovationGenomes.Count;

    }
    public NNet GenerateNNet(){
        NNetData nNetData = new NNetData();
        int disjointGenes = 0;
        float weightDiff = 0;
        int weightDiffCount = 0;
      
        //First do the neurons
        List<string> neuronIds = new List<string>();
        foreach(GenomeHolder genomeHolder in innovationGenomes.Values){
            if(genomeHolder.included){
                if (genomeHolder.neuronDep == null)
                {
                    //Find Genome
                    string neuronId = genomeHolder.neuron.id;
                    while(neuronIds.Contains(neuronId)){
                        neuronId += "_" + genomeHolder.neuron.innovationNumber;
                    }
                    neuronIds.Add(neuronId);
                    NNetData.NeuronData neuronData = genomeHolder.neuron.ToData();
                    neuronData.id = neuronId;
                    neuronData.dependencies.Clear();
                    if(neuronData.origenNNetId == null || neuronData.origenSpecies == null){
                        throw new System.Exception("Missing Origin info");
                    }
                    nNetData.neurons.Add(neuronData);
                    if (GameManager.instance.gameConfigData.storeNNetHistory)
                    {
                        NNetData.NNetMutateHistory historyData = new NNetData.NNetMutateHistory();
                        historyData.neuron1 = neuronData.id;
                        historyData.type = "BreedAddNeuron";
                        historyData.generation = nNetData.generation;
                        nNetData.history.Add(historyData);
                    }
                }
            }else{
                disjointGenes += 1;
            }
        }
        //Second do the neuronDeps
        foreach (GenomeHolder genomeHolder in innovationGenomes.Values)
        {
            if (genomeHolder.neuronDep != null)
            {
                if (!genomeHolder.included)
                {
                    disjointGenes += 1;

                    //Debug.Log("!!!!Not adding successfull");

                }
                else
                {
                    if (!genomeHolder.weightDiff.Equals(0))
                    {
                        weightDiff += genomeHolder.weightDiff;
                        weightDiffCount += 1;
                    }
                    //Find Genome
                    foreach (NNetData.NeuronData neuronData in nNetData.neurons)
                    {
                        if (neuronData.id == genomeHolder.neuron.id)
                        {


                            neuronData.dependencies.Add(genomeHolder.neuronDep.ToData());

                           

                            /*
                             if (GameManager.instance.gameConfigData.storeNNetHistory)
                            {
                                NNetData.NNetMutateHistory historyData = new NNetData.NNetMutateHistory();
                                historyData.neuron1 = neuronData.id;
                                historyData.neuron2 = genomeHolder.neuronDep.depNeuron.id;
                                historyData.type = "BreedAddNeuronDep";
                                historyData.generation = nNetData.generation;
                                nNetData.history.Add(historyData);
                            }
                            */
                        }
                    }
                    if (genomeHolder.neuronDep.origenNNetId == null || genomeHolder.neuronDep.origenSpecies == null)
                    {
                        throw new System.Exception("Missing Origin info");
                    }



                }
            }
        }

        float innovationGenomes_Count = innovationGenomes.Count;
        float disjointScore = ((float)disjointGenes) / innovationGenomes_Count;
   
        float weightScore = 0;
        if (weightDiffCount > 0)
        {
            weightScore = weightDiff / weightDiffCount;
        }

        similarityScore = disjointScore + weightScore;
       
        int maxGeneration = 0;
        //Debug.Log(parentBots[0].botController.speciesObject.id + " !== " + parentBots[1].botController.speciesObject.id);
        foreach(ParentHolder parentHolder in parentBots.Values){
            if(parentHolder.NpcnNetController.nNet.generation > maxGeneration){
                maxGeneration = parentHolder.NpcnNetController.nNet.generation;
            } 
        }
        BrainMaker brainMaker = GameManager.instance.level.brainMaker;
        NNet nNet = brainMaker.ParseNNetData(nNetData);
        nNet.generation = maxGeneration + 1;
        return nNet;
    }
	//public override void OnDestroy()
    ~BreedAction()
	{
        //base.OnDestroy();
        /*
        for (int i = 0; i < parentBots.Count; i++)
        {
            UnityEngine.Object.Destroy(parentBots.ElementAt(i).Value);
        }*/
        parentBots.Clear();
        parentBots = null;
        /*
        for (int i = 0; i < innovationGenomes.Count; i++)
        {
            UnityEngine.Object.Destroy(innovationGenomes.ElementAt(i).Value);
        }*/
        innovationGenomes.Clear();
        innovationGenomes = null;
        highParentHolder = null;



	}
    public CompareResult Compare(){
        CompareResult compareResult = new CompareResult();

        int disjointGenes = 0;
        float weightDiff = 0;
        int weightDiffCount = 0;
        foreach(GenomeHolder genomeHolder in innovationGenomes.Values){
            compareResult.debug += "\n\n";
            if (genomeHolder.neuronDep == null)
            {
                compareResult.debug += genomeHolder.innovationNumber + "  -> " + genomeHolder.neuron.id + "\n";
            }else{
                compareResult.debug += genomeHolder.neuronDep.innovationNumber + "  -> " + genomeHolder.neuron.id + "-" + genomeHolder.neuronDep.depNeuron.id  + "\n";
            }

            if (genomeHolder.parentIndexs.Count != parentBots.Count)
            {
                compareResult.debug += "!One Parent! \n";
                //Its a diff
                disjointGenes += 1;
            }
            else if (genomeHolder.neuronDep == null)
            {
                if(genomeHolder.neuron.GetType().Name == "BiasInput"){
                    decimal lastWeight = -999;
                    Dictionary<decimal, int> median = new Dictionary<decimal, int>();
                    if (genomeHolder.baseNeurons.Count == 0)
                    {
                        throw new System.Exception("Big fail. `genomeHolder.neuronDeps.Count == 0`");
                    }
                    foreach (BaseNeuron baseNeuron in genomeHolder.baseNeurons.Values)
                    {
                        decimal weight = ((BiasInput)baseNeuron).weight;
                        //Debug.Log("Testing BiasInput - " + weight);
                        if(lastWeight.Equals(-999)){
                            lastWeight = weight;
                        }else if(!lastWeight.Equals(weight)){
                            //Debug.Log("LOOK HERE: " + lastWeight + " != " + weight);
                        }
                        if (!median.ContainsKey(weight))
                        {
                            median.Add(weight, 0);
                        }
                        median[weight] += 1;
                    }
                    if (median.Count > 1)
                    {
                        //It is a diff

                        disjointGenes += median.Count - 1;
                       
                    }
                }

            }else {
                Dictionary<decimal, int> median = new Dictionary<decimal, int>();
                if(genomeHolder.neuronDeps.Count == 0){
                    throw new System.Exception("Big fail. `genomeHolder.neuronDeps.Count == 0`");
                }
                bool disabledFlag = false;
                foreach(int parentIndex in genomeHolder.neuronDeps.Keys){
                    NeuronDep neuronDep = genomeHolder.neuronDeps[parentIndex];
                 
                    decimal weight = neuronDep.weight;
                    compareResult.debug += "Parent: " + parentIndex.ToString() + " Weight: " + weight + " - Enabled: " + neuronDep.enabled + "\n";
                    foreach (NeuronDep neuronDep2 in genomeHolder.neuronDeps.Values)
                    {
                        if (
                            neuronDep.enabled != neuronDep2.enabled
                        ){
                            disabledFlag = true;
                        }
                    }



                    if(!median.ContainsKey(weight)){
                        median.Add(weight, 0);
                    }
                    median[weight] += 1;
                }
                if(median.Count > 1){
                    //It is a diff
                    float genomeWeightDiff = 0;
                    foreach(float medianWeight in median.Keys){
                        foreach (float medianWeight2 in median.Keys)
                        {
                            if(!medianWeight.Equals(medianWeight2)){
                                weightDiffCount += 1;
                                genomeWeightDiff += Mathf.Abs(medianWeight - medianWeight2);
                            }
                        }
                    }
                    weightDiff += genomeWeightDiff;
                }

                if (disabledFlag)
                {
                    //I think this is a disjoint
                    compareResult.debug += "Enabled/Disabled Toggled - Marking Disjoint +1\n";
                    disjointGenes += 1;
                }
            }
           


        }
     
        float innovationGenomes_Count = innovationGenomes.Count;
        float disjointScore = ((float)disjointGenes) / innovationGenomes_Count;
        compareResult.debug += "Disjoint Genes: " + disjointGenes + "\n";
        compareResult.debug += "innovationGenomes.Count: " + innovationGenomes.Count + "\n";
        compareResult.debug += "disjointScore: " + disjointScore + "\n";
        float weightScore = 0;
        if (weightDiffCount > 0)
        {
            weightScore = weightDiff / weightDiffCount;
        }
        compareResult.debug += "weightDiff: " + weightDiff + "\n";
        compareResult.debug += "weightDiffCount: " + weightDiffCount + "\n";
        compareResult.debug += "weightScore: " + weightScore + "\n";
        compareResult.score = disjointScore + weightScore;
        compareResult.debug += "score: " + compareResult.score + "\n";
        return compareResult;
    }

}
