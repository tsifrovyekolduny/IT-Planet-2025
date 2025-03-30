using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public TextAsset scriptFile;
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    void Start()
    {
        ParseScript();
    }

    public void ParseScript()
    {
        string[] lines = scriptFile.text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(':');
            if (parts.Length >= 2)
            {
                DialogueLine dl = new DialogueLine
                {
                    role = parts[0].Trim(),
                    text = parts[1].Trim()
                };
                dialogueLines.Add(dl);
            }
        }
    }
}

[System.Serializable]
public class DialogueLine
{
    public string role;
    public string text;
}
