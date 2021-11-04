using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GenerationScoreResultData{
    public int generation;
    public float min;
    public float max = -99999;
    public float median;
    public float mean;
    public string _species;
    public int _speciesAge;
    public float preCull_max;
    public float preCull_min;
    public float preCull_median;
    public float preCull_mean;
}
