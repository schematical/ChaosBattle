using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class GenerationPerformanceData
{
    public const int medianRoundTo = 1;
    public int generation;
    public float maxNNetProcessingTime;
    public float minNNetProcessingTime;
    public float meanNNetProcessingTime;
    public float medianNNetProcessingTime;

    public float totalNNetTime;
    public IDictionary<float, int> medianNNetHolder = new Dictionary<float, int>();

    public float generationDurationRealWorld;
    public float avgRealWorldToGameTimeRatio;
}