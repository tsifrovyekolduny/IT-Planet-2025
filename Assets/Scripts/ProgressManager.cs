using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class ProgressManager : Singletone<ProgressManager>
{
    private static ProgressData _progress;
    private const string SaveKey = "GameProgress";

    // Загружаем данные при старте
    ProgressManager()
    {
        LoadProgress();
    }

    public void SaveProgress()
    {
        string json = JsonUtility.ToJson(_progress);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            _progress = JsonUtility.FromJson<ProgressData>(json);
        }
        else
        {
            _progress = new ProgressData();
        }
    }

    // Обновляем прогресс для направления
    public void UpdateDirectionProgress(string directionId, int line, string script)
    {
        LoadProgress();
        if (!_progress.directions.ContainsKey(directionId))
        {
            _progress.directions[directionId] = new ProgressData.DirectionProgress();
        }

        _progress.directions[directionId].currentLine= line;
        _progress.directions[directionId].currentScript = script;
        SaveProgress();
    }

    // Получаем текущий прогресс
    public (int level, string script) GetDirectionProgress(string directionId)
    {
        if (_progress.directions.TryGetValue(directionId, out var progress))
        {
            return (progress.currentLine, progress.currentScript);
        }
        return (0, null); // Начальное состояние
    }
}



[Serializable]
public class ProgressData
{
    public Dictionary<string, DirectionProgress> directions = new Dictionary<string, DirectionProgress>();

    [Serializable]
    public class DirectionProgress
    {
        public int currentLine; // Текущий уровень (Y)
        public string currentScript; // Например: "PI-script1"
    }
}