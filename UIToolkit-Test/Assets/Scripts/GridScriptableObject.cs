using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Grid", menuName = "ScriptableObjects/Grid")]
public class GridScriptableObject : ScriptableObject
{
    [SerializeField] private List<GridElement> _gridElements = new List<GridElement>();

    public List<GridElement> GridElements
    {
        get => _gridElements;
        set => _gridElements = value;
    }
}

/// <summary>
/// Serialized class for displaying the data more clearly.  
/// </summary>
[System.Serializable]
public class GridElement
{
    public int GridPosX;
    public int GridPosY;

    public string type;
}
