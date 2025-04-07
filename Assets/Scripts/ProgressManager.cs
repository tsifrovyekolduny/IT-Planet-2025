using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class ProgressManager : Singletone<ProgressManager>
{    
    private ProgressData _progress;
    private string SavePath => Path.Combine(Application.persistentDataPath, "progress.dat");
    private const string SaveKey = "GameProgress";

    // Загружаем данные при старте
    void Start()
    {
        LoadProgress();
        Debug.Log($"ProgressManager is running, current data is: {_progress}");
        Debug.Log($"PM is: {ProgressManager.Instance.didStart}");
    }

    public void SaveProgress()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(SavePath, FileMode.Create))
        {
            formatter.Serialize(stream, _progress);
        }
    }

    public void LoadProgress()
    {
        if (File.Exists(SavePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(SavePath, FileMode.Open))
            {
                _progress = (ProgressData)formatter.Deserialize(stream);
            }
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
        LoadProgress();        
        if (_progress.directions.TryGetValue(directionId, out var progress))
        {
            return (progress.currentLine, progress.currentScript);
        }
        return (0, "00"); // Начальное состояние
    }
}



[System.Serializable]
public class ProgressData
{
    public Dictionary<string, DirectionProgress> directions = new Dictionary<string, DirectionProgress>();

    [System.Serializable]
    public class DirectionProgress
    {
        public int currentLine; // Текущий уровень (Y)
        public string currentScript; // Например: "PI-script1"
    }
}