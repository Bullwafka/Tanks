using System;
using System.Collections.Generic;
using System.Linq;

public static class AllSystems
{
    private static Dictionary<Type, IApplicationSystem> s_applicationSystems;

    private static List<IApplicationSystem> s_systems;
    public static List<IApplicationSystem> ApplicationSystems 
    { 
        get 
        {
            s_systems ??= s_applicationSystems.Values.ToList();
            return s_systems;
        }
    }

    static AllSystems()
    {
        s_applicationSystems = new();    
    }

    public static void Initialize(IApplicationSystem[] applicationSystems)
    {
        foreach (IApplicationSystem system in applicationSystems)
        {
            system.Initialize();
            RegisterSystem(system);
        }
    }

    public static void RegisterSystems(IApplicationSystem[] applicationSystems)
    {
        foreach (IApplicationSystem system in applicationSystems)
        {
            RegisterSystem(system);
        }
    }

    public static void RegisterSystem(IApplicationSystem system) 
    {
        Type type = system.GetType();

        if(s_applicationSystems.TryGetValue(type, out IApplicationSystem registred))
        {
            throw new Exception($"Application system of type {type} is already registred");
        }

        s_applicationSystems.Add(type, system);
    }

    public static T GetSystem<T>()
    {
        if(s_applicationSystems.TryGetValue(typeof(T), out IApplicationSystem system))
        {
            return (T)system;
        }

        throw new Exception($"Application system of type {typeof(T)} can't be found. " +
                                   $"Application system must be registred first");
    }

    public static void Shutdown()
    {
        foreach (IApplicationSystem system in s_applicationSystems.Values)
        {
            system.Shutdown();
        }
    }
}
