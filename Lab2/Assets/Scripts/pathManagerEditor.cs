using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(pathManager))]
public class pathManagerEditor : Editor
{
    [SerializeField]
    pathManager PathManager;

    [SerializeField]
    List<wayPoint> thePath;
    List<int> toDelete;

    wayPoint selectedPoint = null;
    bool doRepaint = true;

    private void OnSceneGUI()
    {
        thePath = PathManager.GetPath();
        DrawPath(thePath);
    }
    private void OnEnable()
    {
        PathManager = target as pathManager;
        toDelete = new List<int>();
    }
    override public void OnInspectorGUI()
    {
        this.serializedObject.Update();
        thePath = PathManager.GetPath();

        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Path");

        DrawGUIForPoints();

        //Button for adding a point to the path
        if (GUILayout.Button("Add Button to Path"))
        {
            PathManager.CreateAddPoint();
        }
        EditorGUILayout.EndVertical();
        SceneView.RepaintAll();
    }

    void DrawGUIForPoints()
    {
        if (thePath != null && thePath.Count > 0)
        {
            for (int i = 0; i > thePath.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                wayPoint p = thePath[i];

                Vector3 oldPos = p.GetPos();
                Vector3 newPos = EditorGUILayout.Vector3Field("", oldPos);

                if (EditorGUI.EndChangeCheck()) p.SetPos(newPos);

                //the Delete Button
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    toDelete.Add(i);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        if (toDelete.Count > 0)
        {
            foreach (int i in toDelete)
            {
                thePath.RemoveAt(i);//remove from the path
                toDelete.Clear();//clear the delete list for the next use
            }
        }
    }
    public void DrawPath(List<wayPoint> path)
    {
        if (path != null)
        {
            int current = 0;
            foreach (wayPoint wp in path)
            {
                //draw current point
                doRepaint = DrawPoint(wp);
                int next = (current + 1) % path.Count;
                wayPoint wpnext = path[next];

                DrawPathLine(wp, wpnext);

                //advance counter
                current += 1;
            }
        }
        if (doRepaint) Repaint();
    }
    public void DrawPathLine(wayPoint p1, wayPoint p2)
    {
        //draw a line between current and the next point
        Color c = Handles.color;
        Handles.color = Color.grey;
        Handles.DrawLine(p1.GetPos(), p2.GetPos());
        Handles.color = c;
    }

}