using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameInitializer
{
    static GameObject[] managers;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        managers = Resources.LoadAll<GameObject>("Managers");

        foreach(GameObject m in managers)
        {
            Object.Instantiate(m);
        }
    }
}
