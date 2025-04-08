using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singletone<GameManager>
{
    [SerializeField]
    private TextAsset ScriptFile { get; set; }
    [SerializeField]
    private int Line { get; set; }

    // TODO так информацию между сценами переносить - лютый кринж

    public Direction CurrentDirection;
    public Department CurrentDepartment;
    public string PreviousLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetLevel(TextAsset scriptLevel, int line)
    {
        Debug.Log($"Level set: {scriptLevel.name} on {line}");
        Line = line;
        ScriptFile = scriptLevel;
        SceneManager.LoadScene("dialog");

        // Обнуляем предыдущий уровень
        PreviousLevel = "";
    }

    public void SetLoadedLevel()
    {
        string scriptFilePath;
        int line;
        TextAsset script;

        if (PreviousLevel != null)
        {
            scriptFilePath = PreviousLevel;
            script = GetScriptByName(scriptFilePath);
            line = script.text.Split('\n').Length;
        }
        else
        {
            var items = ProgressManager.Instance.GetDirectionProgress(CurrentDirection.name);
            if (items.script == null)
            {
                return;
            }
            line = items.level;
            scriptFilePath = items.script;
            script = GetScriptByName(scriptFilePath);
        }

        Debug.Log($"Level loading: {scriptFilePath} on {line}");
        SetLevel(script, line);        
    }
    private TextAsset GetScriptByName(string scriptFilePath)
    {
        string filePath = $"LevelScripts/{CurrentDirection.name}/" + scriptFilePath;
        TextAsset scriptFile = Resources.Load<TextAsset>(filePath);

        return scriptFile;
    }

    public void CompleteSettedLevel()
    {
        int nextLevelIndex = Int32.Parse(ScriptFile.name.Substring(0, 2)) + 1;
        string nextLevelName = nextLevelIndex.ToString().PadLeft(2, '0');
        try
        {
            // Проверка, есть ли такой файл в принципе
            var scriptFile = GetScriptByName(nextLevelName);
            SaveProgress(0, nextLevelName);
            SetLevel(scriptFile, 0);
        }
        catch
        {
            Debug.LogWarning($"All levels on {CurrentDirection.name} cleared");
            ToLevelsHub();
        }
    }

    public void ToLevelsHub()
    {        
        SceneManager.LoadScene("LevelsScene");
    }

    public void SaveProgress(int line, string scriptName)
    {
        (_, string loadedScriptName) = ProgressManager.Instance.GetDirectionProgress(CurrentDirection.name);

        int loadedLevelIndex = Int32.Parse(loadedScriptName.Substring(0, 2));
        int currentLevelIndex = Int32.Parse(scriptName.Substring(0, 2));

        if (currentLevelIndex == loadedLevelIndex)
        {
            Debug.Log($"{scriptName}'s ${line} line changed");
            ProgressManager.Instance.UpdateDirectionProgress(CurrentDirection.name, line, scriptName);
        }
        else if (currentLevelIndex > loadedLevelIndex)
        {
            Debug.Log($"{scriptName} on ${line} progress saved");
            ProgressManager.Instance.UpdateDirectionProgress(CurrentDirection.name, line, scriptName);
        }
        else
        {
            Debug.Log($"{scriptName} not saved, because saved higher level");
        }


    }

    // Update is called once per frame
    public (TextAsset, int) GetLevel()
    {
        Debug.Log($"Giving level data: {ScriptFile.name} on {Line}");
        return (ScriptFile, Line);
    }

}
