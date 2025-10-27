using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ArduinoJsonManager : MonoBehaviour
{
    [Space, Header("Setting Folder Paths"), SerializeField]
    List<string> SettingFolderPaths;
    [Space, Header("Setting Json Path"), SerializeField]
    string SettingJsonPath;

    [Space, Header("Arduino Board Data"), SerializeField]
    public ArduinoBoardData ArduinoBoardData = new ArduinoBoardData();

    void Awake()
    {
        SettingFolderPaths.Add(Application.streamingAssetsPath);
        SettingFolderPaths.Add(Application.streamingAssetsPath + "/Setting Json");
        SettingFolderPaths.Add(Application.streamingAssetsPath + "/Setting Json/Arduino");
        SettingJsonPath = Application.streamingAssetsPath + "/Setting Json/Arduino/Arduino Board Data.json";

        CheckFolder();
        CheckJson();
    }

    void CheckFolder()
    {
        foreach (string folder in SettingFolderPaths)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }

    void CheckJson()
    {
        if (!File.Exists(SettingJsonPath))
        {
            WriteJson(SettingJsonPath);
        }
        ReadJson(SettingJsonPath);
    }

    public void WriteJson(string path)
    {
        ArduinoBoardData ArduinoBoardData = new ArduinoBoardData()
        {
            FriendlyName = "Arduino",
            iBaudRates = 115200
        };

        string settingdata = JsonUtility.ToJson(ArduinoBoardData);

        StreamWriter file = new StreamWriter(path);
        file.Write(settingdata);
        file.Close();
    }

    public void ReadJson(string path)
    {
        using (StreamReader streamreader = File.OpenText(path))
        {
            string settingdata = streamreader.ReadToEnd();
            streamreader.Close();
            ArduinoBoardData = JsonUtility.FromJson<ArduinoBoardData>(settingdata);
        }
    }
}

[Serializable]
public class ArduinoBoardData
{
    public string FriendlyName;
    public int iBaudRates;
}
