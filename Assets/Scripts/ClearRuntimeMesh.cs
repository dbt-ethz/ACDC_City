using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mola;
using System;

public class ClearRuntimeMesh : MolaMonoBehaviour
{
    [Range(0, 10)]
    public float seed = 0;
    public override void UpdateGeometry()
    {
        
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        Debug.Log(filters.Length);

        int n = filters.Length;
        for (int i = 0; i < n; i++)
        {
            DestroyImmediate(filters[i]);
            //Debug.Log($"i: {i}");
        }

    }
}
