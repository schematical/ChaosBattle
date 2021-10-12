using System;
using UnityEngine;
public class ActionPhase {
    public string Name {get; private set;}

    public ActionPhase(string name) {
        Name = name;
    }

    public static ActionPhase Initializing = new ActionPhase("Initializing");
    public static ActionPhase Finished = new ActionPhase("Finished");
}
public abstract class BaseAction
{
    protected NPCEntity actingNPCEntity;
    protected ActionPhase _phase = ActionPhase.Initializing;
    private float activeTime = 0;
    private float timeout = 5;
    public BaseAction(NPCEntity npcEntity)
    {
        actingNPCEntity = npcEntity;
    }

    public virtual void tick()
    {
        activeTime += Time.deltaTime;
        if (activeTime > timeout)
        {
            Debug.Log("Timedout: " + activeTime + " > " + timeout);
            TransitionPhase(ActionPhase.Finished);
        }
    }

    public virtual Boolean isFinished()
    {
        return _phase.Equals(ActionPhase.Finished);
    }
    protected virtual void TransitionPhase(ActionPhase actionPhase)
    {
        _phase = actionPhase;
    
    }
}
