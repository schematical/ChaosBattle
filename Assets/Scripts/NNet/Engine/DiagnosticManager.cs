using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DiagnosticManager {
    public int debugLogPositionTickInterval = 2;
    /*public class DebugTile{
        public TileObject tile;
        public Color origColor;
    }*/
    protected BaseNeuron currDebugNeuron;
    protected ObjectPoolCollection lineObjectPool = new ObjectPoolCollection();
    protected IList<CTBaseObservableGameObject> highlighted = new List<CTBaseObservableGameObject>();
    public void SetDebugNeuron(BaseNeuron baseNeuron){
        if(currDebugNeuron != null){
            currDebugNeuron.debug = false;   
        }
        currDebugNeuron = baseNeuron;
        currDebugNeuron.debug = true;
    }
    public void Reset(){
        foreach(CTBaseObservableGameObject debugObject in highlighted){
            debugObject.ClearDiagnostic();
        }
        highlighted.Clear();
        ClearDebugLines();
    }
    public void RemoveHighlighedObservibal(CTBaseObservableGameObject cTBaseObservableGameObject)
    {
        if (!highlighted.Contains(cTBaseObservableGameObject))
        {
            return;
        }
        highlighted.Remove(cTBaseObservableGameObject);
    }
    public void AddHighlighedObservibal(CTBaseObservableGameObject cTBaseObservableGameObject){
        if(highlighted.Contains(cTBaseObservableGameObject)){
            return;
        }
        highlighted.Add(cTBaseObservableGameObject);
    }
    /*
    public void MarkDebugObject(CTBaseObservableGameObject ctGameObject, Color color){
        HighlightObject highlightObject = new HighlightObject();
        highlightObject.attachedGameObject = ctGameObject;
        SpriteRenderer spriteRenderer = highlightObject.attachedGameObject.gameObject.GetComponent<SpriteRenderer>();
        debugTile.origColor = spriteRenderer.color;
        spriteRenderer.color = color;
        debugTiles.Add(debugTile);
    }

    public void MarkDebugTile(TileObject tileObject, Color color)
    {
        DebugTile debugTile = new DebugTile();
        debugTile.tile = tileObject;
        SpriteRenderer spriteRenderer = debugTile.tile.gameObject.GetComponent<SpriteRenderer>();
        debugTile.origColor = spriteRenderer.color;
        spriteRenderer.color = color;
        debugTiles.Add(debugTile);
    }
    */

    public class NNetDebugData{
        public int totalGenomeCount = 0;
        public IDictionary<string, int> speciesBreakdown = new Dictionary<string, int>();
        public IDictionary<string, int> botBreakdown = new Dictionary<string, int>();
    }


    public NNetDebugData DebugNNet(NNet nNet)
    {

        NNetDebugData nNetDebugData = new NNetDebugData();

        foreach (BaseNeuron neuron in nNet.neurons.Values)
        {
            nNetDebugData.totalGenomeCount += 1;
            if (neuron.origenSpecies == null)
            {
                throw new System.Exception("null origenSpecies found");
            }
            if (!nNetDebugData.speciesBreakdown.ContainsKey(neuron.origenSpecies))
            {
                nNetDebugData.speciesBreakdown.Add(neuron.origenSpecies, 0);
            }
            nNetDebugData.speciesBreakdown[neuron.origenSpecies] += 1;
            if (!nNetDebugData.botBreakdown.ContainsKey(neuron.origenNNetId))
            {
                nNetDebugData.botBreakdown.Add(neuron.origenNNetId, 0);
            }
            nNetDebugData.botBreakdown[neuron.origenNNetId] += 1;


            foreach (NeuronDep neuronDep in neuron.dependencies)
            {
                nNetDebugData.totalGenomeCount += 1;
                if(neuronDep.origenSpecies == null){
                    throw new System.Exception("null origenSpecies found");
                }
                if (!nNetDebugData.speciesBreakdown.ContainsKey(neuronDep.origenSpecies))
                {
                    nNetDebugData.speciesBreakdown.Add(neuronDep.origenSpecies, 0);
                }
                nNetDebugData.speciesBreakdown[neuronDep.origenSpecies] += 1;

                if (!nNetDebugData.botBreakdown.ContainsKey(neuronDep.origenNNetId))
                {
                    nNetDebugData.botBreakdown.Add(neuronDep.origenNNetId, 0);
                }
                nNetDebugData.botBreakdown[neuronDep.origenNNetId] += 1;


            }
        }
        return nNetDebugData;


    }
    public List<GameObject> debugLines = new List<GameObject>();
    public void ClearDebugLines()
    {
        foreach (GameObject line in debugLines)
        {
            if (line != null)
            {
                line.SetActive(false);
                //Object.Destroy(line);
            }
        }
        debugLines.Clear();
    }
    public void DrawDebug(Vector2 v1, Vector2 v2, Color color)
    {
        
        GameObject instance = lineObjectPool.GetInActive();
        LineRenderer lineRenderer = null;
        if (instance == null)
        {
            instance = new GameObject("DebugLine");
            //neuronDepGameObject.transform.SetParent(neuronDisplayBoard.transform, false);
            lineRenderer = instance.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            //set the width
     
            lineRenderer.sortingOrder = 1;
            lineRenderer.sortingLayerName = "UI";
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));//lineMaterial;//new Material(Shader.Find("Sprites/Diffuse"));// = new Material(Shader.Find("Sprites/Default"));

            lineObjectPool.objects.Add(instance);

        }else{
            instance.SetActive(true);
            lineRenderer = instance.GetComponent<LineRenderer>();
        }
       
      
        lineRenderer.startWidth = Camera.main.orthographicSize * .001f + .05f;
        lineRenderer.endWidth = lineRenderer.startWidth;


        lineRenderer.SetPosition(0, v1);



        lineRenderer.SetPosition(1, v2);



        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        debugLines.Add(instance);
    }
}
