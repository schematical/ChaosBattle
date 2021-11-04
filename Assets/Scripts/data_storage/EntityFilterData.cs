using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class EntityFilterData: ICTSerializedData{
    public List<string> entityTypes = new List<string>();

    public bool TestEntity(ChaosEntity entityObject)
    {
        if (!entityTypes.Contains(entityObject._class_name))
        {
            return false;
        }
        return true;
    }
    public override string ToString()
    {
        string data = string.Join(" ", entityTypes.ToArray());
        return data;
    }
}
