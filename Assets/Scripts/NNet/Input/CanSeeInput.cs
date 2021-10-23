using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSeeInput : InputNeuron
{
    public const float ZERO_VALUE = -1f;
    //public TileFilterData tileFilterData;
    //public EntityFilterData entityFilterData;
    public string attributeId;
    public Eye eye;
    public string eyeIndex;
    public CanSeeInput(string id) : base(id)
    {
        
    }
    public override void Evaluate()
    {
        _lastValue = ZERO_VALUE;
        if (NpcnNet.entity == null || NpcnNet.entity.rigidbody2D == null)
        {
            return;
        }
        if(!eye){
            if (!NpcnNet.botEyeManager.eyes.ContainsKey(eyeIndex))
            {
                throw new System.Exception("Invalid Eye Index: " + eyeIndex);
            }
            eye = NpcnNet.botEyeManager.eyes[eyeIndex];
        }
        //Figure out what the bot can see
        List<CTBaseObservableGameObject> hits = eye.PolyHits();
        //Eye.RaycastHitEvent debugHitcastEvent = null;
        if(debug){
            GameManager.instance.diagnosticManager.Reset();

            /*Color color = CTColor.Red1;
            color.a = .25f;
            bot.botEyeManager.MarkDiagnostic(bot, color); */
            Color color = CTColor.Red1;
            color.a = .75f;
            eye.MarkDiagnostic(NpcnNet, color);
            eye.DrawDebug(NpcnNet, CTColor.Purple1);
        }
        float closestHitDist = -999;
        CTBaseObservableGameObject closestHit = null;
        foreach (CTBaseObservableGameObject hit in hits)
        {
          
            //if(_lastValue.Equals(ZERO_VALUE)){
                
                float observeValue = hit.Observe(attributeId, NpcnNet.entity);
                if(!observeValue.Equals(CTOA.__ZERO_VALUE)){
                 
                 /*
                    if (GameManager.instance.trainingRoomData.brainMakerConfigData.useBinaryNeuronInput)
                    {
                        _lastValue = 1f;
                    }
                    else
                    { */
                        float distance = (NpcnNet.entity.transform.position - hit.transform.position).magnitude;//(bot.entity.transform.position - hit.tile.transform.position).magnitude;
                       
                        if(
                             
                            closestHitDist.Equals(-999) ||
                            distance < closestHitDist
                        ){

                            float distanceValue = (eye.eyeData.visionDistance - (distance - eye.eyeData.startDistance)) / (eye.eyeData.visionDistance);
                            if(distanceValue < 0){
                                distanceValue = .1f;//This happens when you see the edge of something pretty far away
                            }
                            float newValue = observeValue * distanceValue;


                            closestHit = hit;
                            closestHitDist = distance;
                            _lastValue = newValue;
                        }
                       
                    //}

                }
            //}
            if(debug){
                //Keep observing anyway

                if (!observeValue.Equals(CTOA.__ZERO_VALUE))
                {
                    //Draw some stuff
                    Color color = CTColor.BrightBlue1;
                    color.a = _lastValue;
                    hit.MarkDiagnostic(color);
                }
            }

        }
        if(debug && closestHit != null){
            Color color = CTColor.BrightBlue1;
            closestHit.MarkDiagnostic(color);
        }
     
    }
   
    /*

    public override void Evaluate()
    {
        _lastValue = -1f;
        if (bot.entity.rigidbody2D == null)
        {
            return;
        }
        if(!eye){
            foreach (Eye _eye in bot.bioInputs)
            {
                if (_eye.eyeData.id == eyeIndex)
                {
                    eye = _eye;
                }
            }
            if (!eye)
            {
                throw new System.Exception("Invalid Eye Index: " + eyeIndex);
            }
        }

        if (tileFilterData != null)
        {
            TestTile();
        }
        else if (entityFilterData != null)
        {
            TestEntity();
        }
        else
        {
            throw new System.Exception("No target set for CanSeeInput. What is going on here?!?!");
        }

          
    }
   
    public void TestEntity(){
        
        foreach (Eye.RaycastHitEvent hit in eye.RaycastHits(bot))
        {
            if(hit.entity){
                TestEntityHit(hit);
            }
        }
    }
    public void TestEntityHit(Eye.RaycastHitEvent hit){
       
       
        if (!entityFilterData.TestEntity(hit.entity))
        {
            return;
        }
       
        if (GameManager.instance.trainingRoomData.brainMakerConfigData.useBinaryNeuronInput)
        {
            _lastValue = 1f;
        }
        else
        {

            float distance = hit.raycastHit2D.distance;//(bot.entity.transform.position - hit.entity.transform.position).magnitude;
            _lastValue = (eye.eyeData.visionDistance - distance) / (eye.eyeData.visionDistance);
        }
    }
    public void TestTile(){
        //Figure out what the bot can see
        List<Eye.RaycastHitEvent> hits = eye.RaycastHits(bot);
       
        foreach (Eye.RaycastHitEvent hit in hits)
        {
            
            if(hit.tile && _lastValue.Equals(-1)){
             
                if(tileFilterData.TestTile(hit.tile, bot)){
                    //debug += "FOUND! \n";
                    if (GameManager.instance.trainingRoomData.brainMakerConfigData.useBinaryNeuronInput)
                    {
                        _lastValue = 1f;
                    }
                    else
                    {
                        float distance = hit.raycastHit2D.distance;//(bot.entity.transform.position - hit.tile.transform.position).magnitude;
                        _lastValue = (eye.eyeData.visionDistance - distance) / (eye.eyeData.visionDistance);
                    }

                }
            }

        }
     

    }
   */
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        //neuronData.Set("entityFilterData", JsonUtility.ToJson(entityFilterData));
        //neuronData.Set("tileFilterData", JsonUtility.ToJson(tileFilterData));
       
        neuronData.Set("eyeIndex", eyeIndex);
        neuronData.Set("attributeId", attributeId);
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        /*
        string _entityFilterData = neuronData.Get("entityFilterData");
        entityFilterData = JsonUtility.FromJson<EntityFilterData>(_entityFilterData);

        string _tileFilterData = neuronData.Get("tileFilterData");
        tileFilterData = JsonUtility.FromJson<TileFilterData>(_tileFilterData);
        */
        eyeIndex = neuronData.Get("eyeIndex");
        attributeId = neuronData.Get("attributeId");

    }
    public override void PopulateRandom()
    {
        base.PopulateRandom();
       
        int index = Random.Range(0, GameManager.instance.prefabManager.tiles.Length - 1);
        /*
        TileObject tileObject = GameManager.instance.prefabManager.tiles[index].GetComponent<TileObject>();
        tileFilterData = new TileFilterData
        {
            tileTypes = new List<string>{
                tileObject.name
            }
        };
        */

        throw new System.Exception("Not implemented fully. TODO: Add entities");
    }
    public override string ToString()
    {
        string data = "CanSee";
       
        data += " - " + attributeId;
        data +=  " - Last:" + _lastValue;
        return data;
    }
    ~CanSeeInput()
	//public override void OnDestroy()
	{
        //base.OnDestroy();
        eye = null;
       
	}

}