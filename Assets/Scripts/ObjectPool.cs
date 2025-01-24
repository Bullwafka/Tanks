using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPoolable
{
    bool Pooled { get; }
}

public class ObjectPool<T> : IDisposable where T : MonoBehaviour, IPoolable
{
    private T m_prefab;
    private List<T> m_pool;
    public ObjectPool(T prefab, int initCount = 0)
    {
        m_prefab = prefab;
        m_pool = new List<T>(initCount);

        for (int i = 0; i < initCount; i++)
        {
            CreateNewPooledInstance();
        }
    }

    public T GetFromPool()
    {
        T pooled = m_pool.FirstOrDefault(p => p.Pooled);
        if (pooled == null)
        {
            pooled = CreateNewInstance();
        }

        return pooled;
    }

    public void AddToPool(T pooled)
    {
        if (m_pool.Contains(pooled))
        {
            Debug.LogError($"Pool already contains {pooled}");
            return;
        }

        m_pool.Add(pooled);
    }

    private void CreateNewPooledInstance()
    {
        T pooled = CreateNewInstance();
        AddToPool(pooled);
    }

    private T CreateNewInstance()
    {
        T pooled = GameObject.Instantiate(m_prefab);
        pooled.gameObject.SetActive(false);
        AddToPool(pooled);
        return pooled;
    }

    public void Dispose()
    {
        foreach (T pooled in m_pool.ToArray())
        {
            GameObject.Destroy(pooled.gameObject);
        }
    }
}
