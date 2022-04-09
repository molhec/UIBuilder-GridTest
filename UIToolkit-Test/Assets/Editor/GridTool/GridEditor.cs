using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    private Grid _grid;

    private VisualElement _rootElement;
    private VisualTreeAsset _visualTree;
    private VisualTreeAsset _gridElement;
    private StyleSheet _uss;

    private Vector2IntField _gridSize;
    private VisualElement _gridContainer;
    private Button _generateButton;
    private Button _createScriptableButton;
    private TextField _scriptableName;

    private int _gridWidth = 0;
    private int _gridHeight = 0;

    private List<GridButton> _gridButtons = new ListStack<GridButton>();
    
    private struct GridButton
    {
        public int GridPosX;
        public int GridPosY;

        public Button Button;
    }
    
    private void OnEnable()
    {
        _grid = target as Grid;
        _rootElement = new VisualElement();
        _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/GridTool/GridToolXDoc.uxml");
        _gridElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/GridTool/GridElement.uxml");
        _uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/GridTool/GridToolStyle.uss");
        
        _rootElement.styleSheets.Add(_uss);
        _visualTree.CloneTree(_rootElement);

        _gridSize = _rootElement.Q<Vector2IntField>("gridSize");
        _gridContainer = _rootElement.Q<VisualElement>("grid");
        _generateButton = _rootElement.Q<Button>("generateButton");
        _createScriptableButton = _rootElement.Q<Button>("createScriptableButton");
        _scriptableName = _rootElement.Q<TextField>("scriptableName");
        
        _gridSize.BindProperty(serializedObject.FindProperty("_gridSize"));
    }

    public override VisualElement CreateInspectorGUI()
    {
        _gridSize.RegisterValueChangedCallback(OnSaveGridValues);
        _generateButton.clicked += GenerateGrid;
        _createScriptableButton.clicked += CreateNewScriptableObject;
        
        return _rootElement;
    }
    
    /// <summary>
    /// Update the values of the _gridSize every time the value is changed.
    /// </summary>
    /// <param name="e"></param>
    private void OnSaveGridValues(ChangeEvent<Vector2Int> e)
    {
        _gridWidth = e.newValue.x;
        _gridHeight = e.newValue.y;
    }

    /// <summary>
    /// Create the grid by adding copies of '_gridElement' as children of 'gridContainer'.
    /// </summary>
    private void GenerateGrid()
    {
        _gridContainer.style.width = (_gridWidth * 100) / 2;
        _gridContainer.style.height = _gridHeight * 100 / 2;
        
        _gridContainer.Clear();
        _gridButtons.Clear();

        for (int w = 0; w < _gridHeight; w++)
        {
            for (int h = 0; h < _gridWidth; h++)
            {   
                VisualElement clone = _gridElement.CloneTree();
                Button button = clone.Q<Button>("buttonType");
                button.RegisterCallback<ClickEvent>(OnButtonClicked);
                _gridContainer.Add(clone);

                GridButton newElement = new GridButton
                {
                    GridPosX = w,
                    GridPosY = h,
                    Button = button
                };
                _gridButtons.Add(newElement);
            }
        }
        
        _createScriptableButton.visible = _gridWidth != 0 && _gridHeight != 0;
        _scriptableName.visible = _gridWidth != 0 && _gridHeight != 0;
    }

    /// <summary>
    /// Changes the Text and Color of the clicked button.
    /// </summary>
    /// <param name="e"></param>
    private void OnButtonClicked(ClickEvent e)
    {
        foreach (var b in _gridButtons.Where(b => e.currentTarget == b.Button))
        {
            b.Button.text = b.Button.text == "Gem" ? "Empty" : "Gem";
            b.Button.style.backgroundColor = b.Button.text == "Gem" ? new Color(0f, 117f/255f, 180f/255f, 1f) : Color.black;
        }
    }

    /// <summary>
    /// Creates a new Scriptable Object with the info of the buttons, after that it selects it in the Project Window.
    /// </summary>
    private void CreateNewScriptableObject()
    {
        if (_scriptableName.value == "")
        {
            Debug.LogError("The field for the name of the Scriptable Object is empty.");
            return;
        }
        
        GridScriptableObject newAsset = CreateInstance<GridScriptableObject>();

        foreach (var b in _gridButtons)
        {
            GridElement newElement = new GridElement
            {
                type = b.Button.text,
                GridPosX = b.GridPosX,
                GridPosY = b.GridPosY
            };
            
            newAsset.GridElements.Add(newElement);
        }
        
        if (AssetDatabase.LoadAssetAtPath<GridScriptableObject>("Assets/ScriptableObjects/" + _scriptableName.value + ".asset") is null)
        {
            AssetDatabase.CreateAsset(newAsset, "Assets/ScriptableObjects/" + _scriptableName.value + ".asset");
            AssetDatabase.SaveAssets();
        
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAsset;
        }
        else
        {
            Debug.LogError("A Scriptable Object with that name already exists, please try a different one.");
        }
        
    }
}