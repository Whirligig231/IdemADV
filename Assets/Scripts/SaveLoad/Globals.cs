using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Globals
{
    private static Dictionary<string, string> sessionData;
    private static Dictionary<string, string> saveData;
    private static bool initialized = false;

    private static void Initialize()
    {
        if (initialized)
            return;

        sessionData = new Dictionary<string, string>();
        saveData = new Dictionary<string, string>();

        string fname = Application.persistentDataPath + "/saveData.dat";
        if (File.Exists(fname))
        {
            StreamReader reader = new StreamReader(fname);
            string currentKey = "";
            string currentValue = "";
            while (!reader.EndOfStream)
            {
                string thisLine = reader.ReadLine();
                if (thisLine.StartsWith("=> "))
                {
                    if (currentKey != "")
                        saveData[currentKey] = currentValue;
                    currentKey = thisLine.Substring(3);
                    currentValue = "";
                }
                else
                {
                    if (currentValue != "")
                        currentValue += "\n";
                    currentValue += thisLine;
                }
            }

            if (currentKey != "")
                saveData[currentKey] = currentValue;

            reader.Close();
        }

        initialized = true;
    }

    private static void Save()
    {
        string fname = Application.persistentDataPath + "/saveData.dat";
        StreamWriter writer = new StreamWriter(fname);

        foreach (string identifier in saveData.Keys)
        {
            writer.WriteLine("=> " + identifier);
            writer.WriteLine(saveData[identifier]);
        }

        writer.Close();
    }

    public static void SetSessionVariable(string key, string value)
    {
        Initialize();
        sessionData[key] = value;
    }

    public static string GetSessionVariable(string key)
    {
        Initialize();
        if (!sessionData.ContainsKey(key))
            return null;
        return sessionData[key];
    }

    public static void SetSaveVariable(string key, string value)
    {
        Initialize();
        saveData[key] = value;
        Save();
    }

    public static string GetSaveVariable(string key)
    {
        Initialize();
        if (!saveData.ContainsKey(key))
            return null;
        return saveData[key];
    }
}
