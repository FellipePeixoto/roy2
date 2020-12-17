﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakWallController : MonoBehaviour
{
    Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    public void BreakWall()
    {
        col.enabled = false;
    }
}
