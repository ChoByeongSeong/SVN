using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(YellowBean.Grid))]
public class GridEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        YellowBean.Grid grid = (YellowBean.Grid)target;

        if (GUILayout.Button("Create Grid"))
        {
            grid.CreateGrid();
        }
    }
}

#endif