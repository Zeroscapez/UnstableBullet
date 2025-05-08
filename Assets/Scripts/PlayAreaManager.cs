using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaManager : MonoBehaviour
{
    public static PlayAreaManager Instance { get; private set; }

    // Define fixed boundaries for the play area
    public float MinX = -14f;
    public float MaxX = 1.4f;
    public float MinY = -10f;
    public float MaxY = 10f;

    private void Awake()
    {
        // Ensure only one instance of PlayAreaManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}