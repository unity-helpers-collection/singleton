#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

public static class SingletonManager
{
    private static readonly Dictionary<Type, object> Instances = new();
    private static GameObject? _singletonGo = null;

    /**
     * Get the instance of the type T.
     * If the instance is not found, it will create a new instance.
     * If the type is a Component, it will create a new GameObject with the Component attached.
     * If the type is not a Component, it will create a new instance using the parameterless constructor.
     *
     * @return The instance of the type T.
     * @throws Exception if the type T is not a Component and does not have a parameterless constructor.
     */
    public static T GetInstance<T>() where T : class
    {
        var type = typeof(T);

        {
            // instance is found and returned
            var instanceOrNull = GetInstanceOrNull<T>(type);
            if (instanceOrNull != null)
                return instanceOrNull;
        }

        // else
        // instance is not found
        if (typeof(T).IsSubclassOf(typeof(Component)))
        {
            // instance type is a Component
            if (_singletonGo == null)
            {
                // There is no Singleton GameObject cached
                _singletonGo = GameObject.Find("Singleton");
                if (_singletonGo != null)
                {
                    // There is a Singleton Object in the scene. Find all existing components on it and register them
                    var components = _singletonGo.GetComponents<Component>();
                    components ??= Array.Empty<Component>();
                    foreach (var component in components)
                    {
                        if (component is Transform)
                            // Skip the Transform component.
                            // It is always on a GameObject, but should not be a singleton 
                            continue;

                        Instances.Add(component.GetType(), component);
                    }

                    // If there is an instance on the already existing Singleton GameObject, return it
                    var instanceOrNull = GetInstanceOrNull<T>(type);
                    if (instanceOrNull != null)
                        return instanceOrNull;
                }
                else
                {
                    // There is no Singleton Object in the scene. Create one
                    _singletonGo = new GameObject("Singleton");
                }
            } 
            // Now there is a Singleton GameObject cached, but the instance is not found on it
            // Creating a new instance
            var newInstance = _singletonGo.AddComponent(typeof(T));
            Instances.Add(type, newInstance);
            return (newInstance as T)!;
        }
        else
        {
            // instance type is not a Component
            var newInstance = Activator.CreateInstance<T>();
            if(newInstance == null)
                throw new Exception($"The type ${typeof(T)} has to be a Component or have a parameterless constructor");
            
            Instances.Add(type, newInstance);
            return newInstance;
        }
    }

    private static T? GetInstanceOrNull<T>(Type type) where T : class
    {
        if (!Instances.TryGetValue(type, out var instanceObject))
            return null;

        if (instanceObject is not T instanceT)
            throw new Exception("The Object should always have the type of the Key");

        return instanceT;
    }
}
