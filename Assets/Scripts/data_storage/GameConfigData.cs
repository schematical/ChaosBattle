using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class GameConfigData: ICTSerializedData{

    //Fittness
  
    //public decimal secondsBetweenTicks = .2m;
    public string _secondsBetweenTicks = ".2";
    public int secondsSinceLastPing = 2;

    public bool storeNNetHistory = false;

    //public decimal spawnThroddle = .25m;
    public string _spawnThroddle = ".1";
    public decimal spawnThroddle{
        get{
            return decimal.Parse(_spawnThroddle);
        }
        set{
            _spawnThroddle = value.ToString();
        }
    }

    public decimal secondsBetweenTicks{
        get{
            return decimal.Parse(_secondsBetweenTicks);
        }
        set{
            _secondsBetweenTicks = value.ToString();
        }
    }
    
}
