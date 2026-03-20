using System.Collections.Generic;
using Knot.Include.Construct;
using UnityEngine;

namespace Knot.scr.Allocate
{
    /// <summary>
    /// 指令执行器对象池，管理特定类型的 InstrExecute 实例
    /// </summary>
    public class ExecutePool
    {
        private readonly Queue<InstrExecute> _available = new();
        private readonly HashSet<InstrExecute> _allObjects = new();
        private readonly GameObject _prefab;
        private readonly string _instrName;

        /// <summary>
        /// 可用执行器数量
        /// </summary>
        public int AvailableCount => _available.Count;

        /// <summary>
        /// 总执行器数量
        /// </summary>
        public int TotalCount => _allObjects.Count;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prefab">执行器预制体</param>
        /// <param name="instrName">执行器名称，默认为"Execute"</param>
        public ExecutePool(GameObject prefab, string instrName = "Execute")
        {
            _prefab = prefab;
            _instrName = instrName;
        }

        /// <summary>
        /// 从对象池获取一个执行器实例
        /// </summary>
        /// <returns>可用的执行器实例，如果池为空则创建新实例</returns>
        public InstrExecute Get()
        {
            InstrExecute executor = _available.Count > 0 ? _available.Dequeue() : CreateNewExecutor();
            executor.gameObject.SetActive(true);
            return executor;
        }

        /// <summary>
        /// 将执行器释放回对象池
        /// </summary>
        /// <param name="executor">要释放的执行器</param>
        public void Release(InstrExecute executor)
        {
            if (executor == null || !_allObjects.Contains(executor))
            {
                Debug.LogWarning($"[ExecutePool.Release] Attempted to release invalid executor: {executor}");
                return;
            }

            executor.gameObject.SetActive(false);
            _available.Enqueue(executor);
        }

        /// <summary>
        /// 预加载指定数量的执行器实例到对象池
        /// </summary>
        /// <param name="count">预加载数量</param>
        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                InstrExecute executor = CreateNewExecutor();
                executor.gameObject.SetActive(false);
                _available.Enqueue(executor);
            }
        }

        /// <summary>
        /// 清理对象池，销毁所有执行器实例
        /// </summary>
        public void Clear()
        {
            foreach (var executor in _allObjects)
            {
                if (executor != null && executor.gameObject != null)
                {
                    GameObject.DestroyImmediate(executor.gameObject);
                }
            }

            _available.Clear();
            _allObjects.Clear();
        }

        public void SetActive(bool active)
        {
            foreach (var executor in _allObjects)
            {
                executor.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// 创建新的执行器实例
        /// </summary>
        /// <returns>新创建的执行器实例</returns>
        private InstrExecute CreateNewExecutor()
        {
            GameObject obj = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity);
            InstrExecute executor = obj.GetComponent<InstrExecute>();
            if (executor == null)
            {
                Debug.LogError($"[ExecutePool.CreateNewExecutor] Prefab {_prefab.name} does not have InstrExecute component!");
                GameObject.DestroyImmediate(obj);
                return null;
            }

            _allObjects.Add(executor);

            obj.name = _instrName + $"({TotalCount - 1})";
            return executor;
        }
    }
}
