using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Service locator class for finding general game singleton services
/// </summary>
public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;
    Dictionary<Type, MonoBehaviour> serviceCache;

    private void Awake()
    {
        Instance = this;
        serviceCache = new Dictionary<Type, MonoBehaviour>();
    }

    public T GetService<T>() where T : MonoBehaviour
    {
        // If Dict is not set up, avoid null reference exception
        if (serviceCache == null) {
            return null;
        }

        // Search the cache
        if (serviceCache.ContainsKey(typeof(T)))
        {
            return (T)serviceCache[typeof(T)];
        }

        // Fallback
        T component = GetComponentInChildren<T>();
        if (!component) { return null; }
        serviceCache.Add(typeof(T), component);
        return component;
    }
}