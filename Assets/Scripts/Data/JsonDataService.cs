using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JsonDataService : IDataService
{
    public bool SaveData<T>(string RelativePath, T Data)
    {
        string path = Application.persistentDataPath + RelativePath;

        try
        {
            if (File.Exists(path))
            {
                Debug.Log("Data exists. Deleting old file and writing a new one");

                File.Delete(path);
            }
            else
            {
                Debug.Log("Writing file for the first time");
            }


            using FileStream stream = File.Create(path);

            stream.Close();

            File.WriteAllText(path, JsonConvert.SerializeObject(Data));

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public T LoadData<T>(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist");
            throw new FileNotFoundException($"{path} does not exist");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw;
        }
    }

    public void DeleteData<T>(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
                Debug.Log($"Data at {path} has been deleted.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete data at {path}: {e.Message} {e.StackTrace}");
                throw;
            }
        }
        else
        {
            Debug.LogWarning($"No file found at {path} to delete.");
        }
    }
}
