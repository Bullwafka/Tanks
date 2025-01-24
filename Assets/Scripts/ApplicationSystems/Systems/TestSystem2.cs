using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem2 : MonoBehaviour, IApplicationSystem
{
    public void Initialize()
    {
        Debug.Log($"{this.GetType()} Initialized");
    }

    public void Shutdown()
    {
        throw new System.NotImplementedException();
    }
}
