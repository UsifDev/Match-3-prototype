using System.IO;
using UnityEngine;

public class GeneralSettingsManager : MonoBehaviour
{
    // Data to persist
    public bool isMusicOn = true;
    public bool isSoundOn = true;


    // Singleton Pattern
    public static GeneralSettingsManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int highScore;
    }
    public void Save()
    {
        SaveData data = new SaveData();
        
        // Add data to persist across sessions below like this: data.name = name;


        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // load data from storage below like this: name = data.name;
        }
    }
}
