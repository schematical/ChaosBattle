using UnityEngine;
using System.Collections;

public abstract class GameModeBase
{

    public abstract string type
    {
        get;

    }
    public abstract void Setup();

    public abstract void Tick();

    public abstract void Suspend();
    public abstract void Shutdown();
    public TrainingRoomData trainingRoomData{
        get{
            return GameManager.instance.trainingRoomData;
        }
    }
    public virtual void OnInitNPC(NPCControllerBase npcnNetController){}
    public virtual void OnDestroyBot(NPCControllerBase npcnNetController){}
    public virtual void CheckInputs(){}
}
