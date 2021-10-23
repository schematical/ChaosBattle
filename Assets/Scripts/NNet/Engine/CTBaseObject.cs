using UnityEngine;
using System.Collections;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

public abstract class CTBaseObject//, System.IDisposable
{
    
    public abstract string _class_name
    {
        get;

    } 

    public CTBaseObject()
    {
       
        GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
    }

    ~CTBaseObject(){
   
      
        GameManager.instance.garbageCollector.DeregisterObject(_class_name);
    }

}
