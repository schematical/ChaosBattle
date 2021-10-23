using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnterTileInput : InputNeuron
{
    public string tileClass;
    public OnEnterTileInput(string id) : base(id)
    {
        
    }

    public override void Evaluate()
    {
        _lastValue = 0;
        Vector2 vector = NpcnNet.entity.transform.position;
        TileObject tile = GameManager.instance.boardManager.GetTile(
                (int) vector.x,
                (int) vector.y
        );
        if (tile != null)
        {
            string foundClassType = tile.GetRealType();
            if (foundClassType == tileClass)
            {
                _lastValue = 1;
            }
        }
       
    }
    public override NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        neuronData.Set("tileClass", tileClass);
        return neuronData;
    }
    public override void ParseData(NNetData.NeuronData neuronData)
    {
        base.ParseData(neuronData);
        tileClass = neuronData.Get("tileClass");
    }
    public override void PopulateRandom()
    {
        base.PopulateRandom();
        int index = Random.Range(0, GameManager.instance.prefabManager.tiles.Length - 1);
        TileObject tileObject = GameManager.instance.prefabManager.tiles[index].GetComponent<TileObject>();
        tileClass = tileObject.name;
    }
}