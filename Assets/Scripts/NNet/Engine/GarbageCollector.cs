using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GarbageCollector {
    
    public class GCStats{
        public int currCount = 0;
        public int maxCount = 0;
    }
    public IDictionary<string, GCStats> classes = new Dictionary<string, GCStats>();
    public void RegisterNewObject(string classType){
        if(!classes.ContainsKey(classType)){
            classes.Add(classType, new GCStats());
        }
        classes[classType].currCount += 1;
        if(classes[classType].currCount > classes[classType].maxCount){
            classes[classType].maxCount = classes[classType].currCount;
        }
    }

    public void DeregisterObject(string classType)
    {
        if (!classes.ContainsKey(classType))
        {
            throw new System.Exception("GarbageCollector is trying to Deregister but does not contain instance:" + classType);
        }
        classes[classType].currCount -= 1;
        if (classes[classType].currCount < 0)
        {
            throw new System.Exception("GarbageCollector instance < 0:" + classType + " - " + classes[classType]);
        }
    }
}
