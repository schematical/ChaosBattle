using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class BaseData 
{
    [System.Serializable]
    public class BaseDataProperty
    {
        public string key;
        public string value;
        public string type;
    }

    public List<BaseDataProperty> properties = new List<BaseDataProperty>();

    public BaseData()
    {
        if (GameManager.instance != null && GameManager.instance.garbageCollector != null)
        {
            GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
        }
    }

    ~BaseData()
    {
      
        GameManager.instance.garbageCollector.DeregisterObject(_class_name);
    }
    public abstract string _class_name{
        get;
    }

    public string Get(string key)
    {
        foreach (BaseDataProperty property in properties)
        {
            if (property.key == key)
            {
                return property.value;
            }
        }
        return null;
    }

    public void Set(string key, string value)
    {
        BaseDataProperty property = null;
        foreach (BaseDataProperty _property in properties)
        {
            if (_property.key == key)
            {
                property = _property;
            }
        }
        if (property == null)
        {
            property = new BaseDataProperty();
            property.key = key;
            properties.Add(property);
        }
        property.value = value;

    }
}
