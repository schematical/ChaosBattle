using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OutputNeuron : BaseNeuron {
    protected OutputNeuron(string id):base(id)
    {
    }
    public abstract void Execute(float score);
    public override string _base_type
    {
        get { return "output"; }
    }
	
}
