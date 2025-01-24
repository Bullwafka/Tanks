using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class SavingSystem : MonoBehaviour, IApplicationSystem
{
    private string m_savePath;

    public void Initialize()
    {
        // Assigned here instead of field initializer to prevent 
        // MonoBehaviour serialization exceptions
        m_savePath = Application.persistentDataPath + "/saves";

        Directory.CreateDirectory(m_savePath);
    }

    public void Shutdown() { }

    public void Save<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data);

        using (StreamWriter writer = new StreamWriter(m_savePath + $"/{fileName}.json"))
        {
            writer.WriteLine(json);
        }
    }

    // Validation of save version/config version is needed
    public bool TryLoad<T>(string fileName, out T save) where T : new()
    {
        string path = m_savePath + $"/{fileName}.json";
        save = new();

        if (!File.Exists(path))
        {
            return false;
        }

        //Needed to prevent corrupted saves loading
        try
        {
            string json;

            using (StreamReader reader = new StreamReader(path))
            {
                json = reader.ReadLine();
            }

            save = JsonUtility.FromJson<T>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Loading error. Save file may be corrupted and will be deleted. Exception: {ex.Message}");
            DeleteSave(fileName);
            return false;
        }

        return true;
    }

    private void DeleteSave(string fileName)
    {
        File.Delete(m_savePath + $"/{fileName}.json");
    }
}

[Serializable]
public class SaveData
{
    public TanksSaveData[] tanksSaveData;
}

[Serializable]
public struct TanksSaveData
{
    public Vector3 position;
    public Quaternion rotation;
    public bool activeState;
}
