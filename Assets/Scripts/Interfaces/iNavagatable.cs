﻿
using UnityEngine;

public interface iNavagatable
{
    GameObject gameObject { get;  }
    Transform transform { get; }
    float speed { get; }
    T GetComponent<T>();
}
