using UnityEngine;
using System.Collections;

public abstract class CTBaseMonoBehavior : MonoBehaviour
{
    public virtual void Restart(){}
    public abstract string _class_name
    {
        get;

    } 
    public virtual void Awake()
    {
        //disposed = false;
        // handle = new SafeFileHandle(System.IntPtr.Zero, true);
        //System.Threading.Interlocked.Increment(ref counter);
        if (GameManager.instance)
        {
            GameManager.instance.garbageCollector.RegisterNewObject(_class_name);
        }
    }

    //~CTBaseObject()
    public virtual void OnDestroy()
    {
        //System.Threading.Interlocked.Decrement(ref counter);
        GameManager.instance.garbageCollector.DeregisterObject(_class_name);
    }
	
}
