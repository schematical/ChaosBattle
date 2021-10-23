using UnityEngine;
using System.Collections;

public interface IGameModeWithFitnessConfig{
     
    FitnessManagerConfigData fitnessManagerConfigData
    {
        get;
        set;
    }
}
