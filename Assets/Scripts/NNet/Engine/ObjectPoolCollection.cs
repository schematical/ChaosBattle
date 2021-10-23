using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ObjectPoolCollection
{
    public GameObject prefab;
    public List<GameObject> objects = new List<GameObject>();
  
    public GameObject GetInActive(){
        foreach(GameObject gameObject in objects){
           
            if (gameObject != null && !gameObject.activeInHierarchy)
            {
                return gameObject;
            }

        }
        return null;
    }
    public int GetInActiveCount(){
        int inactiveCount = 0;
        foreach (GameObject gameObject in objects)
        {
            if (!gameObject.activeInHierarchy)
            {
                inactiveCount += 1;
            }
        }
        return inactiveCount;
    }
}
