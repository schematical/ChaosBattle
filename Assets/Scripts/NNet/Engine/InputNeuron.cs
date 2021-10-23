using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputNeuron : BaseNeuron {
    protected InputNeuron(string id):base(id) 
    {
    }
        
    public override void Evaluate()
    {
        throw new System.NotImplementedException();
    }
    public override string _base_type
    {
        get { return "input"; }
    }
}
