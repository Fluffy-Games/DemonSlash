using System;
using System.Collections;
using System.Collections.Generic;
using Tabtale.TTPlugins;
using UnityEngine;

public class ClikInit : MonoBehaviour
{
    private void Awake()
    {
        TTPCore.Setup();
    }
}
