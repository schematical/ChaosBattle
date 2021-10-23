using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GenerationObject 
{
    private string _id;
    public FitnessSortingBlock fitnessSortingBlock = new FitnessSortingBlock();

    public GenerationObject(string id)
    {
        _id = id;
    }
    public string id
    {
        get { return _id; }
    }
}
