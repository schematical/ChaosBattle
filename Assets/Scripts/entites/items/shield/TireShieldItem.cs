using System.Collections;
using System.Collections.Generic;
using services;
using UnityEngine;

public class TireShieldItem : ChaosShieldItem
{
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override string _class_name
    {
        get { return "TireShieldItem"; }
    }
}
