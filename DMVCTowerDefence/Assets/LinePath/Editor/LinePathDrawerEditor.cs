using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using GameLogic;

[CustomEditor(typeof(LinePathManager))]
public class LinePathDrawerEditor : Editor
{
    private LinePathManager _linePathManager;
    private const float PointSelectThreshold = 0.2f;
    private int _hoverIndex = -1;

    private void OnEnable()
    {
        _linePathManager = (LinePathManager)target;
    }

    private void OnSceneGUI()
    {
        if (_linePathManager.PathGroups.Count == 0) return;

        var currentGroup = _linePathManager.PathGroups[_linePathManager.CurrentGroupIndex];
        Event e = Event.current;

        // 鼠标悬停逻辑，计算距离最近的点
        _hoverIndex = -1;
        Vector3 mousePosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
        mousePosition.z = 0;
        for (int i = 0; i < currentGroup.PathPoints.Count; i++)
        {
            if (Vector3.Distance(mousePosition, currentGroup.PathPoints[i]) < PointSelectThreshold)
            {
                _hoverIndex = i;
                break;
            }
        }

        // 显示高亮提示
        if (_hoverIndex != -1)
        {
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(currentGroup.PathPoints[_hoverIndex], Vector3.forward, 0.15f);
        }

        // 绘制每个点的拖拽Handle
        for (int i = 0; i < currentGroup.PathPoints.Count; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPoint = Handles.PositionHandle(currentGroup.PathPoints[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_linePathManager, "Move Path Point");
                currentGroup.PathPoints[i] = newPoint;
                _linePathManager.GenerateGlobalAdjacencyList();
                EditorUtility.SetDirty(_linePathManager);
            }
        }

        // 左键点击逻辑：插入新点
        if (e.type == EventType.MouseDown && e.button == 0 && _hoverIndex == -1)
        {
            Undo.RecordObject(_linePathManager, "Insert Path Point");
            InsertPointIntoPath(mousePosition, currentGroup);
            _linePathManager.GenerateGlobalAdjacencyList();
            e.Use();
        }

        // 右键移除点逻辑
        if (e.type == EventType.MouseDown && e.button == 1 && _hoverIndex != -1)
        {
            Undo.RecordObject(_linePathManager, "Remove Path Point");
            currentGroup.PathPoints.RemoveAt(_hoverIndex);
            _linePathManager.GenerateGlobalAdjacencyList();
            e.Use();
        }

        // 绘制平滑曲线
        Handles.color = Color.red;
        for (int i = 0; i < currentGroup.PathPoints.Count - 1; i++)
        {
            DrawSmoothCurve(currentGroup.PathPoints[i], currentGroup.PathPoints[i + 1]);
        }

        if (currentGroup.IsClosed && currentGroup.PathPoints.Count > 1)
        {
            DrawSmoothCurve(currentGroup.PathPoints[currentGroup.PathPoints.Count - 1], currentGroup.PathPoints[0]);
        }
    }

    private void InsertPointIntoPath(Vector3 newPoint, LinePathManager.PathGroup group)
    {
        // 如果路径为空，直接添加点
        if (group.PathPoints.Count == 0)
        {
            group.PathPoints.Add(newPoint);
            return;
        }

        // 找到距离新点最近的路径段
        int insertIndex = 0; // 默认插入到开头
        float minSegmentDistance = float.MaxValue;

        for (int i = 0; i < group.PathPoints.Count - 1; i++)
        {
            Vector3 p0 = group.PathPoints[i];
            Vector3 p1 = group.PathPoints[i + 1];
            float distance = DistanceToSegment(newPoint, p0, p1);

            if (distance < minSegmentDistance)
            {
                minSegmentDistance = distance;
                insertIndex = i + 1; // 插入到当前段的后面
            }
        }

        // 判断是否插入到开头或结尾
        float distanceToStart = Vector3.Distance(newPoint, group.PathPoints[0]);
        float distanceToEnd = Vector3.Distance(newPoint, group.PathPoints[group.PathPoints.Count - 1]);

        if (distanceToStart <= minSegmentDistance)
        {
            group.PathPoints.Insert(0, newPoint); // 插入到开头
            return;
        }

        if (distanceToEnd <= minSegmentDistance)
        {
            group.PathPoints.Add(newPoint); // 插入到结尾
            return;
        }

        // 插入到最近的路径段
        group.PathPoints.Insert(insertIndex, newPoint);
    }

    private float DistanceToSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
    {
        Vector3 segment = segmentEnd - segmentStart; // 线段向量
        Vector3 pointToStart = point - segmentStart; // 点到线段起点的向量

        float segmentLengthSquared = segment.sqrMagnitude; // 线段长度平方
        if (segmentLengthSquared == 0) return Vector3.Distance(point, segmentStart); // 线段退化为点

        // 计算投影系数t（点在线段上的投影比例）
        float t = Vector3.Dot(pointToStart, segment) / segmentLengthSquared;
        t = Mathf.Clamp01(t); // 限制t在[0, 1]之间

        // 投影点的位置
        Vector3 projection = segmentStart + t * segment;

        // 返回点到投影点的距离
        return Vector3.Distance(point, projection);
    }

    private void DrawSmoothCurve(Vector3 p0, Vector3 p1)
    {
        for (float t = 0; t < 1f; t += 0.1f)
        {
            Vector3 point = Vector3.Lerp(p0, p1, t);
            Vector3 nextPoint = Vector3.Lerp(p0, p1, t + 0.1f);
            Handles.DrawLine(point, nextPoint);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_linePathManager.PathGroups.Count > 1)
        {
            _linePathManager.CurrentGroupIndex = EditorGUILayout.IntSlider("Current Group Index", _linePathManager.CurrentGroupIndex, 0, _linePathManager.PathGroups.Count - 1);
        }

        if (_linePathManager.PathGroups.Count > 0)
        {
            var currentGroup = _linePathManager.PathGroups[_linePathManager.CurrentGroupIndex];
            currentGroup.IsClosed = EditorGUILayout.Toggle("Is Closed Path", currentGroup.IsClosed);
        }

        if (GUILayout.Button("Add New Path Group"))
        {
            Undo.RecordObject(_linePathManager, "Add Path Group");
            _linePathManager.PathGroups.Add(new LinePathManager.PathGroup());
            Debug.Log("New Path Group added.");
        }

        if (GUILayout.Button("Generate Global Adjacency List"))
        {
            _linePathManager.GenerateGlobalAdjacencyList();
            Debug.Log("Global adjacency list generated.");
        }

        if (GUILayout.Button("Save Path Groups"))
        {
            string savePath = EditorUtility.SaveFilePanel("Save Path Groups", "", "PathGroups.json", "json");
            if (!string.IsNullOrEmpty(savePath))
            {
                SavePathGroups(savePath);
            }
        }

        if (GUILayout.Button("Load Path Groups"))
        {
            string loadPath = EditorUtility.OpenFilePanel("Load Path Groups", "", "json");
            if (!string.IsNullOrEmpty(loadPath))
            {
                LoadPathGroups(loadPath);
            }
        }
    }

    /// <summary>
    /// 保存路径数据到 JSON 文件
    /// </summary>
    /// <param name="path"></param>
    private void SavePathGroups(string path)
    {
        LinePathManager linePathManager = (LinePathManager)target;
        PathGroupsData pathGroupsData = new PathGroupsData
        {
            PathGroups = linePathManager.PathGroups
        };

        string json = JsonUtility.ToJson(pathGroupsData, true);
        File.WriteAllText(path, json);
        Debug.Log("Path groups saved to: " + path);
    }

    /// <summary>
    /// 从 JSON 文件加载路径数据
    /// </summary>
    /// <param name="path"></param>
    private void LoadPathGroups(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        PathGroupsData pathGroupsData = JsonUtility.FromJson<PathGroupsData>(json);

        // 更新 CampPathManager 的路径数据
        LinePathManager linePathManager = (LinePathManager)target;
        linePathManager.PathGroups = pathGroupsData.PathGroups;
        linePathManager.GenerateGlobalAdjacencyList(); // 重新生成邻接表
        EditorUtility.SetDirty(linePathManager); // 标记对象为脏数据以确保保存
        Debug.Log("Path groups loaded from: " + path);
    }

    // 用于保存路径数据的类
    [System.Serializable]
    public class PathGroupsData
    {
        public List<LinePathManager.PathGroup> PathGroups = new List<LinePathManager.PathGroup>();
    }
}