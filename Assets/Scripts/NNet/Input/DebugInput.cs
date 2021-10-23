using UnityEngine;
using System.Collections;

public class DebugInput : InputNeuron
{
    protected float score = 1;
    public DebugInput(string id) : base(id)
    {
    }

	public override void Evaluate()
	{
        _lastValue = score;
	}
    public new NNetData.NeuronData ToData()
    {
        NNetData.NeuronData neuronData = base.ToData();
        neuronData.Set("score", score.ToString());
        return neuronData;
    }
}
