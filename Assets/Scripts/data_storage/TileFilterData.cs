using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TileFilterData{
    
    public bool forceClimb = false;
    public List<string> tileTypes = new List<string>();
    public bool canRemember_hasEntered = false;
    public string tileGroupId;

    /*public bool TestTile(TileObject tileObject, BotController botController)
    {

        if (tileTypes.Count > 0)
        {
            if (!tileTypes.Contains(tileObject.GetRealType()))
            {
                return false;
            }
        }
        if(canRemember_hasEntered){
            BotMemory.BotTileMemory botTileMemory = botController.memory.GetTileMemory((int)tileObject.transform.position.x, (int)tileObject.transform.position.y);
            if(
                botTileMemory != null &&
                botTileMemory.hasEntered
            ){
                //Debug.Log("I remember " + botTileMemory.x + ", " + botTileMemory.y);
                //We are good
            }else{
                return false;
            }
        }
        return true;
    }*/

    public override string ToString()
    {
        string data = string.Join(" ", tileTypes.ToArray());
        return data;
    }
}
