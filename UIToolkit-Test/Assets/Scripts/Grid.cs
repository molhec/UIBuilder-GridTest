using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Vector2Int _gridSize = new Vector2Int(0,0);

    public Vector2Int GridSize
    {
        get => _gridSize;
        set => _gridSize = value;
    }
}
