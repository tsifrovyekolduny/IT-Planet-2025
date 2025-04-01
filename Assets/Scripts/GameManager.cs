using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singletone<GameManager>
{    
    [SerializeField]
    private TextAsset ScriptFile { get; set; }
    [SerializeField]
    private int Line { get; set; }
    [SerializeField]
    private string CurrentDirrectionId;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetLevel(TextAsset scriptLevel, int line)
    {
        Debug.Log($"Level set: {scriptLevel.name} on {line}");
        Line = line;
        ScriptFile = scriptLevel;
        SceneManager.LoadScene("dialog");
    }

    public void SetLoadedLevel()
    {        
        var items = ProgressManager.Instance.GetDirectionProgress(CurrentDirrectionId);
        int line = items.level;
        string scriptFilePath = items.script;

        Debug.Log($"Level loading: {scriptFilePath} on {line}");

        string filePath = $"LevelScripts/{CurrentDirrectionId}/" + scriptFilePath;
        TextAsset scriptFile = Resources.Load<TextAsset>(filePath);

        SetLevel(scriptFile, line);
    }

    public void SetDirectionId(string directionId)
    {
        CurrentDirrectionId = directionId;
    }

    public void SaveProgress(int line, string scriptName)
    {
        Debug.Log($"{scriptName} on ${line} progress saved");
        ProgressManager.Instance.UpdateDirectionProgress(CurrentDirrectionId, line, scriptName);
    }

    // Update is called once per frame
    public (TextAsset, int) GetLevel()
    {
        Debug.Log($"Giving level data: {ScriptFile.name} on {Line}");
        return (ScriptFile, Line);
    }

}
