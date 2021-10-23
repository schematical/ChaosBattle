using System;
using UnityEngine;
public class ActionPhase {
    public string Name {get; private set;}

    public ActionPhase(string name) {
        Name = name;
    }

    public static ActionPhase Initializing = new ActionPhase("Initializing");
    public static ActionPhase Finished = new ActionPhase("Finished");
    public static ActionPhase Timedout = new ActionPhase("Timedout");
}
public abstract class BaseAction
{
    protected ChaosNPCEntity ActingChaosNpcEntity;
    protected ActionPhase _phase = ActionPhase.Initializing;
    private float activeTime = 0;
    private float timeout = 5;
    public BaseAction(ChaosNPCEntity chaosNpcEntity)
    {
        ActingChaosNpcEntity = chaosNpcEntity;
    }

    public virtual void tick()
    {
        activeTime += Time.deltaTime;
        if (activeTime > timeout)
        {
            TransitionPhase(ActionPhase.Timedout);
        }
    }

    public virtual Boolean isFinished()
    {
        return (
            _phase.Equals(ActionPhase.Finished) ||
            _phase.Equals(ActionPhase.Timedout)
        );
    }
    protected virtual void TransitionPhase(ActionPhase actionPhase)
    {
        _phase = actionPhase;
    
    }

    public virtual string GetDebugString()
    {
        string className = GetType().Name;
        return className + "(Phase: " + _phase.Name + ")";
    }
}
