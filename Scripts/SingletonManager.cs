#nullable enable
using System;
using System.Collections.Generic;

public static class SingletonManager
{
    private static readonly Dictionary<Type, object> Instances = new();

    public static T GetInstance<T>() where T : class, new()
    {
        var type = typeof(T);

        if (Instances.TryGetValue(type, out var instance))
        {
            if (instance is T instanceT)
                return instanceT;
            throw new Exception("The Object should always have the type of the Key");
        }

        // else
        var newInstance = new T();
        Instances.Add(type, newInstance);
        return newInstance;
    }
}
