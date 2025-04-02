using UnityEngine;
using System.Collections.Generic;

namespace GameLogic
{
    public class NpcMovement : MonoBehaviour
    {
        public float MoveSpeed = 2f;

        private List<Vector3> _currentPath; // 当前路径
        private int _currentPathIndex; // 当前路径索引
        private bool _isMoving = true; // 控制移动状态

        private void Start()
        {
            if (LinePathManager.Instance == null || !LinePathManager.Instance.IsNavigable)
                return;

            GeneratePathToRandomTarget(); // 初始生成一个随机目标路径
        }

        private void Update()
        {
            // 控制暂停和恢复移动
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isMoving = !_isMoving;
            }

            if (!_isMoving || _currentPath == null || _currentPathIndex >= _currentPath.Count) return;

            // 移动到当前路径点
            transform.position = Vector3.MoveTowards(transform.position, _currentPath[_currentPathIndex], MoveSpeed * Time.deltaTime);

            // 到达当前路径点
            if (Vector3.Distance(transform.position, _currentPath[_currentPathIndex]) < 0.1f)
            {
                _currentPathIndex++;

                // 如果到达路径终点，生成新路径
                if (_currentPathIndex >= _currentPath.Count)
                {
                    GeneratePathToRandomTarget();
                }
            }
        }

        /// <summary>
        /// 生成到随机目标点的路径
        /// </summary>
        private void GeneratePathToRandomTarget()
        {
            _currentPath = LinePathManager.Instance.GetPathToRandomTarget(transform.position);
            _currentPathIndex = 0;

            if (_currentPath == null || _currentPath.Count == 0)
            {
                Debug.LogWarning("未生成有效的随机目标路径！");
                GeneratePathToRandomTarget();
            }
        }

        /// <summary>
        /// 生成到指定目标点的路径
        /// </summary>
        /// <param name="targetPosition">目标位置</param>
        public void GeneratePathToSpecificTarget(Vector3 targetPosition)
        {
            _currentPath = LinePathManager.Instance.GetPathToSpecificTarget(transform.position, targetPosition);
            _currentPathIndex = 0;

            if (_currentPath == null || _currentPath.Count == 0)
            {
                Debug.LogWarning("未生成有效的指定目标路径！");
            }
        }
    }
}