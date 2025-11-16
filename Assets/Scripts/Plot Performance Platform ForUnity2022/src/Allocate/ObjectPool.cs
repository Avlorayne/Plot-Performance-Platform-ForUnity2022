using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plot_Performance_Platform_ForUnity2022.Include.Construct;

namespace Plot_Performance_Platform_ForUnity2022.src.Allocate
{
    /// <summary>
    /// 看了一下，发现对象池在这个项目和Allocate操作功能重合。
    /// Allocate属于静态预分配，在开始前就将所需资源实例化，并在过程中一一对应分配。
    /// 而ObjectPool属于动态分配，现场搜寻可用资源或生成资源后分配。
    /// PPP模块（指令按序执行）相对静态，故用前者。
    /// </summary>
    public class ObjectPool
    {
        private static ObjectPool instance;

        public static ObjectPool Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ObjectPool();
                }
                return instance;
            }
        }
        /// <summary>
        /// objectPool
        /// </summary>
        /// <Key>InstrType Name</Key>
        /// <Value>Executors</Value>
        private Dictionary<string, Queue<GameObject>> objectPool = new ();
        //我们想将所有预制体分类存储，所以用字典存储比较好
        private GameObject pool;    //声明一个父物体使窗口不要太杂乱

        /// <summary>
        /// 从对象池获取一个物体
        /// </summary>
        /// <param name="prefab">要获取的对象预制体</param>
        /// <param name="position">对象生成的位置，默认为default(Vector3)</param>
        /// <returns>返回从对象池获取或新创建的游戏对象</returns>
        public GameObject GetObject(GameObject prefab, Vector3 position = default)
        {
            GameObject _object;

            // 检查对象池中是否存在该类型的对象且池中有可用对象
            if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
            {
                // 池中没有可用对象，创建新对象
                _object = GameObject.Instantiate(
                    prefab,
                    position,
                    Quaternion.identity);

                // 将新创建的对象放入对象池管理
                PushObject(_object);

                // 如果总对象池不存在，则创建
                if (pool == null)
                    pool = new GameObject("ObjectPool");

                // 查找或创建该类型对象的子对象池
                GameObject childPool = GameObject.Find(prefab.name + "Pool");

                if (!childPool)
                {
                    // 创建新的子对象池并设置为总对象池的子物体
                    childPool = new GameObject(prefab.name + "Pool");
                    childPool.transform.SetParent(pool.transform);
                }

                // 将对象设置为子对象池的子物体，便于场景管理
                _object.transform.SetParent(childPool.transform);
            }

            // 从对象池中取出一个对象
            _object = objectPool[prefab.name].Dequeue();

            // 如果指定了位置，则设置对象位置
            if (position != default)
                _object.transform.position = position;

            // 激活对象
            _object.SetActive(true);

            return _object;
        }

        /// <summary>
        /// 将对象放回对象池
        /// </summary>
        /// <param name="prefab">要放回的对象</param>
        public void PushObject(GameObject prefab)
        {
            // 移除对象名称中的"(Clone)"后缀，获取原始预制体名称
            string _name = prefab.name.Replace("(Clone)", string.Empty);

            // 如果对象池中不存在该类型的队列，则创建新的队列
            if (!objectPool.ContainsKey(_name))
                objectPool.Add(_name, new Queue<GameObject>());

            // 将对象加入对应类型的对象池队列
            objectPool[_name].Enqueue(prefab);

            // 禁用对象，使其不可见但保留在场景中
            prefab.SetActive(false);
        }
    }
}
