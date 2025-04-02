using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Random = UnityEngine.Random;

namespace GameLogic
{
    /// <summary>
    /// 线条路径管理器
    /// </summary>
    [ExecuteAlways]
    public class LinePathManager : MonoBehaviour
    {
        [Serializable]
        public class PathGroup
        {
            [Tooltip("路径点列表")]
            public List<Vector3> PathPoints = new List<Vector3>();

            [Tooltip("路径是否闭合")]
            public bool IsClosed = false;
        }

        [Tooltip("路径分组列表")]
        public List<PathGroup> PathGroups = new List<PathGroup>();

        [Tooltip("当前选中的路径分组索引")]
        public int CurrentGroupIndex = 0;

        [Tooltip("均匀分布点之间的距离")]
        [SerializeField]
        private float _pointDensity = 0.5f;

        [Tooltip("邻接表，key为路径点，value为相邻路径点列表")]
        private readonly Dictionary<Vector3, List<Vector3>> _adjacencyList = new Dictionary<Vector3, List<Vector3>>();

        public static LinePathManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GenerateGlobalAdjacencyList();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// 生成全局邻接表
        /// </summary>
        public void GenerateGlobalAdjacencyList()
        {
            _adjacencyList.Clear();

            foreach (var group in PathGroups)
            {
                for (int i = 0; i < group.PathPoints.Count - 1; i++)
                {
                    GenerateUniformPoints(group.PathPoints[i], group.PathPoints[i + 1]);
                }

                if (group.IsClosed && group.PathPoints.Count > 1)
                {
                    GenerateUniformPoints(group.PathPoints[group.PathPoints.Count - 1], group.PathPoints[0]);
                }
            }

            ConnectClosestPointsBetweenGroups();
        }

        /// <summary>
        /// 生成两个点之间的均匀分布点，并添加到邻接表中
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void GenerateUniformPoints(Vector3 start, Vector3 end)
        {
            float distance = Vector3.Distance(start, end);

            // 如果两点之间的距离小于等于密度阈值，则不生成额外的点
            if (distance <= _pointDensity)
            {
                AddToAdjacencyList(start, end); // 直接连接这两个点
                return;
            }

            // 计算应该生成的分段数
            int segmentCount = Mathf.CeilToInt(distance / _pointDensity);
            Vector3 previousPoint = start;

            for (int i = 1; i <= segmentCount; i++)
            {
                float t = i / (float)segmentCount;
                Vector3 currentPoint = Vector3.Lerp(start, end, t);

                // 如果当前点距离前一个点小于点密度阈值，则跳过
                if (Vector3.Distance(previousPoint, currentPoint) <= _pointDensity)
                    continue;

                AddToAdjacencyList(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            // 最后添加终点
            AddToAdjacencyList(previousPoint, end);
        }

        /// <summary>
        /// 将两个点添加到邻接表中
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void AddToAdjacencyList(Vector3 start, Vector3 end)
        {
            start = RoundVector(start); // 四舍五入处理
            end = RoundVector(end); // 四舍五入处理

            if (!_adjacencyList.ContainsKey(start))
                _adjacencyList[start] = new List<Vector3>();
            if (!_adjacencyList.ContainsKey(end))
                _adjacencyList[end] = new List<Vector3>();

            if (!_adjacencyList[start].Contains(end))
                _adjacencyList[start].Add(end);
            if (!_adjacencyList[end].Contains(start))
                _adjacencyList[end].Add(start);
        }

        /// <summary>
        /// 处理浮点数精度问题
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        private Vector3 RoundVector(Vector3 vector, int decimals = 2)
        {
            float multiplier = Mathf.Pow(10, decimals);
            return new Vector3(
                Mathf.Round(vector.x * multiplier) / multiplier,
                Mathf.Round(vector.y * multiplier) / multiplier,
                Mathf.Round(vector.z * multiplier) / multiplier
            );
        }

        /// <summary>
        /// 连接两个路径分组中的最近的点
        /// </summary>
        private void ConnectClosestPointsBetweenGroups()
        {
            for (int i = 0; i < PathGroups.Count; i++)
            {
                for (int j = i + 1; j < PathGroups.Count; j++)
                {
                    var groupA = PathGroups[i];
                    var groupB = PathGroups[j];

                    // 遍历路径组A和路径组B之间的每一对点，计算最短距离并连接
                    float minDistance = float.MaxValue;
                    Vector3 closestA = Vector3.zero;
                    Vector3 closestB = Vector3.zero;

                    foreach (var pointA in groupA.PathPoints)
                    {
                        foreach (var pointB in groupB.PathPoints)
                        {
                            float distance = Vector3.Distance(pointA, pointB);
                            if (distance < minDistance && distance <= _pointDensity)
                            {
                                minDistance = distance;
                                closestA = pointA;
                                closestB = pointB;
                            }
                        }
                    }

                    if (minDistance < float.MaxValue)
                    {
                        // 在相邻的两个路径点之间添加连接
                        AddToAdjacencyList(closestA, closestB);
                    }
                }
            }
        }

        /// <summary>
        /// 是否可以导航，即是否存在路径点
        /// </summary>
        public bool IsNavigable => _adjacencyList.Count > 0;

        /// <summary>
        /// 查找指定位置距离邻接表中最近的路径点
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 FindClosestPoint(Vector3 position)
        {
            Vector3 closestPoint = Vector3.zero;
            float minDistance = float.MaxValue;

            foreach (var point in _adjacencyList.Keys)
            {
                float distance = Vector3.Distance(position, point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                }
            }

            return closestPoint;
        }

        /// <summary>
        /// 获取指定范围内的随机目标点路径
        /// </summary>
        /// <param name="currentPosition">当前位置</param>
        /// <param name="minRadius">搜索半径</param>
        /// <param name="maxRadius">搜索半径</param>
        /// <returns>通往随机目标点的路径</returns>
        public List<Vector3> GetPathToRandomTarget(Vector3 currentPosition, float minRadius = 0f, float maxRadius = 100f)
        {
            // 搜索范围内的路径点
            List<Vector3> potentialTargets = new List<Vector3>();

            // 获取范围内的路径点
            foreach (var point in _adjacencyList.Keys)
            {
                float distance = Vector3.Distance(currentPosition, point);
                if (distance >= minRadius && distance <= maxRadius)
                {
                    potentialTargets.Add(point);
                }
            }

            if (potentialTargets.Count == 0)
            {
                Debug.LogWarning($"在半径 {maxRadius} 内没有找到目标点！");
                return null;
            }

            // 计算路径
            Vector3 closestStartPoint = FindClosestPoint(currentPosition);

            if (potentialTargets.Contains(closestStartPoint))
            {
                potentialTargets.Remove(closestStartPoint);
            }

            // 随机选择目标点
            Vector3 randomTarget = potentialTargets[Random.Range(0, potentialTargets.Count)];

            return FindPath(closestStartPoint, randomTarget);
        }

        /// <summary>
        /// 获取到指定目标点的路径
        /// </summary>
        /// <param name="currentPosition">当前位置</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>通往目标点的路径</returns>
        public List<Vector3> GetPathToSpecificTarget(Vector3 currentPosition, Vector3 targetPosition)
        {
            // 确定路径的起点和终点
            Vector3 startPoint = FindClosestPoint(currentPosition);
            Vector3 endPoint = FindClosestPoint(targetPosition);

            // 计算路径
            return FindPath(startPoint, endPoint);
        }

        /// <summary>
        /// 使用广度优先搜索算法计算路径
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>从起点到终点的路径</returns>
        public List<Vector3> FindPath(Vector3 start, Vector3 end)
        {
            start = RoundVector(start); // 使用四舍五入后的坐标
            end = RoundVector(end); // 使用四舍五入后的坐标

            // 如果起点或终点不在邻接表中，直接返回空路径
            if (!_adjacencyList.ContainsKey(start))
            {
                Debug.LogWarning($"起点 {start} 不存在于邻接表中！");
                return null;
            }

            if (!_adjacencyList.ContainsKey(end))
            {
                Debug.LogWarning($"终点 {end} 不存在于邻接表中！");
                return null;
            }

            // BFS需要的辅助数据结构
            Queue<Vector3> frontier = new Queue<Vector3>(); // 队列用于存储待检查的路径点
            Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>(); // 记录路径
            frontier.Enqueue(start); // 将起点加入队列
            cameFrom[start] = start; // 起点的前驱是它自己

            // 开始BFS搜索
            while (frontier.Count > 0)
            {
                Vector3 current = frontier.Dequeue(); // 从队列中取出一个路径点

                // 如果找到了终点，返回路径
                if (current == end)
                {
                    List<Vector3> path = new List<Vector3>(); // 用于存储路径
                    while (current != start) // 从终点回溯到起点
                    {
                        path.Add(current);
                        current = cameFrom[current];
                    }

                    path.Add(start); // 将起点加入路径
                    path.Reverse(); // 反转路径，使其从起点到终点
                    return path;
                }

                // 遍历当前路径点的相邻点，加入队列进行下一步搜索
                foreach (Vector3 neighbor in _adjacencyList[current])
                {
                    if (!cameFrom.ContainsKey(neighbor)) // 如果邻居没有被访问过
                    {
                        frontier.Enqueue(neighbor); // 加入队列
                        cameFrom[neighbor] = current; // 记录该邻居的前驱点
                    }
                }
            }

            Debug.LogWarning($"未找到从 {start} 到 {end} 的路径！记录的路径点数量={cameFrom.Count}");
            return null;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            DrawAdjacencyList();
            DrawPathGroups();
        }

        /// <summary>
        /// 绘制邻接表
        /// </summary>
        private void DrawAdjacencyList()
        {
            foreach (var point in _adjacencyList)
            {
                Vector3 key = point.Key;
                List<Vector3> neighbors = point.Value;

                // 绘制连接线
                foreach (var neighbor in neighbors)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(key, neighbor);
                }

                // 绘制当前点并显示索引
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(key, 0.1f);
            }
        }

        /// <summary>
        /// 绘制路径分组
        /// </summary>
        private void DrawPathGroups()
        {
            for (int i = 0; i < PathGroups.Count; i++)
            {
                var group = PathGroups[i];

                if (group.PathPoints.Count == 0) continue;

                // 绘制路径点
                for (int j = 0; j < group.PathPoints.Count; j++)
                {
                    if (j == 0)
                    {
                        // 起始点，绿色
                        Gizmos.color = Color.red;
                    }
                    else if (j == group.PathPoints.Count - 1 && !group.IsClosed)
                    {
                        // 结束点，绿色（非闭合路径）
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        // 中间点，红色
                        Gizmos.color = Color.blue;
                    }

                    Gizmos.DrawSphere(group.PathPoints[j], 0.1f);
                }

                // 绘制路径连接线，当前选中的路径分组为红色，其余路径分组为绿色
                Gizmos.color = CurrentGroupIndex == i ? Color.red : Color.green;

                if (group.PathPoints.Count > 0)
                {
                    Handles.Label(group.PathPoints[0], $"Group Index: {i}");
                }

                for (int j = 0; j < group.PathPoints.Count - 1; j++)
                {
                    Gizmos.DrawLine(group.PathPoints[j], group.PathPoints[j + 1]);
                }

                if (group.IsClosed && group.PathPoints.Count > 1)
                {
                    Gizmos.DrawLine(group.PathPoints[group.PathPoints.Count - 1], group.PathPoints[0]);
                }
            }
        }
#endif
    }
}