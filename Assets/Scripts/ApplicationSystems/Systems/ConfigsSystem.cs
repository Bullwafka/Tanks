using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ConfigsSystem : MonoBehaviour, IApplicationSystem
{
#if UNITY_EDITOR
    [Serializable]
    public class InspectorConfigHolder
    {
        [SerializeReference] public List<BaseConfig> configs;

        public InspectorConfigHolder(Type type)
        {
            configs = new List<BaseConfig>();
            configs.Add((BaseConfig)Activator.CreateInstance(type));
        }
    }

    [SerializeReference] public List<InspectorConfigHolder> Configs;

    [ContextMenu("FillConfigs")]
    public void FillConfigs()
    {
        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(BaseConfig))
                ).ToArray();

        foreach (Type type in types)
        {
            Configs.Add(new InspectorConfigHolder(type));
        }
    }

    [ContextMenu("SaveConfigIds")]
    public void SaveConfigIds()
    {
        m_configIds = new List<string>();
        foreach (InspectorConfigHolder configsHolder in Configs)
        {
            foreach (BaseConfig config in configsHolder.configs)
            {
                m_configIds.Add(config.id);
            }
        }
    }
#endif

    // used Application.dataPath to get possibility 
    // to change configs without rebuilding the project
    private readonly string m_configsPath = Application.dataPath + "/Configs";
    private List<BaseConfig> m_loadedConfigs;
    [SerializeField] private List<string> m_configIds;

    public void Initialize()
    {
        m_loadedConfigs = new();
        Directory.CreateDirectory(m_configsPath);
    }

    public void Shutdown() {}

    public T GetConfig<T>(string id) where T : BaseConfig, new()
    {
        BaseConfig loaded = m_loadedConfigs.FirstOrDefault(c => c.id == id);
        if (loaded != null)
        {
            return (T)loaded;
        }

        return GetParsedConfig<T>(id);
    }

    private T GetParsedConfig<T>(string id) where T : BaseConfig, new()
    {
        string path = m_configsPath + $"/{id}.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        return CreateConfig<T>(id, path);
    }

    private T CreateConfig<T>(string id, string path) where T : BaseConfig, new()
    {
        T config = new T();
        config.id = id;

        string json = JsonUtility.ToJson(config);

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLineAsync(json);
        }

        return config;
    }
}

/// <summary>
/// All configs must have default values
/// </summary>
[Serializable]
public class BaseConfig
{
    public string id;
}

public class TankConfig : BaseConfig
{
    public float moveSpeed = 8;
    public float rotationSpeed = 1.5f;
    public float reloadTime = 1;
}

public class GameConfig : BaseConfig
{
    public int enemiesCount = 7;
    public float respawnTime = 3;
}

public class EnemyDefaultBehaviorConfig : BaseConfig
{
    [Serializable]
    public class Duration
    {
        public float minValue;
        public float maxValue;

        public Duration(float min, float max)
        {
            minValue = min;
            maxValue = max;
        }
    }

    public Duration forwardMoveDuration = new(1, 2);
    public Duration backwardMoveDuration = new(0.5f, 0.75f);
    public Duration rotatedMoveDuration = new(1.5f, 2.5f);
    public Duration rotationDuration = new(1, 2.5f);
    public Duration idleStateDuration = new(1, 3);
}
