using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class LevelsMenu : MonoBehaviour
{
    [SerializeField] private LevelScript[] _levelScripts;
    public UIDocument LevelUI;
    public string DirectionStringId;

    private VisualElement root;
    private VisualElement levelsHolder;

    [Serializable]
    private class LevelScript
    {
        public string LevelName;
        public TextAsset Script;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.SetDirectionId(DirectionStringId);
        root = LevelUI.rootVisualElement;
        levelsHolder = root.Q("levels-holder");
        InitButtonsToLevels();

        // for test
        GameManager.Instance.SaveProgress(3, "Example");
    }

    void InitButtonsToLevels()
    {
        root.Q<Button>("continue-btn").clicked += () => GameManager.Instance.SetLoadedLevel();

        foreach (var levelScript in _levelScripts)
        {
            Button button = new Button();
            button.text = levelScript.LevelName;
            button.clicked += () => LoadLecture(levelScript.Script, 0);
            levelsHolder.Add(button);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadLecture(TextAsset scriptFile, int line)
    {
        // Сохраняем имя скрипта для диалоговой сцены
        GameManager.Instance.SetLevel(scriptFile, line);
    }
}

