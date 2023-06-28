using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Newtonsoft.Json;


public class LocalizationManager : MonoBehaviour
{
    private string currentLanguage;
    private Dictionary<string, string> localizedText;
    public static bool isReady = false;

    public delegate void ChangeLangText();
    public event ChangeLangText OnLanguageChanged;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
            {
                PlayerPrefs.SetString("Language", "ru_RU");
            }
            else
            {
                PlayerPrefs.SetString("Language", "en_US");
            }
        }
        currentLanguage = PlayerPrefs.GetString("Language");

        LoadLocalizedText(currentLanguage);
    }

    public void LoadLocalizedText(string langName)
    {
        string path = "Assets/Resources/" + langName + ".txt";

        TextAsset textAsset = Resources.Load<TextAsset>(path);

        if (textAsset != null)
        {
            string dataAsJson = textAsset.text;
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
            localizedText = new Dictionary<string, string>();
            currentLanguage = langName;

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");

            OnLanguageChanged();
        }
        else
        {
            throw new Exception("Localization Error!: " + langName + " does not have a .txt resource!");
        }

        isReady = true;

        PlayerPrefs.SetString("Language", langName);
        currentLanguage = PlayerPrefs.GetString("Language");

        OnLanguageChanged?.Invoke();
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        else
        {
            throw new Exception("Localized text with key \"" + key + "\" not found");
        }
    }

    public string CurrentLanguage
    {
        get
        { return currentLanguage; }
        set { LoadLocalizedText(value); }
    }
    public bool IsReady
    {
        get
        {
            return isReady;
        }
    }
}
