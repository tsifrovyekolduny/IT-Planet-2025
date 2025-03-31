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
        root = LevelUI.rootVisualElement;
        levelsHolder = root.Q("levels-holder");
        InitButtonsToLevels();
    }

    void InitButtonsToLevels()
    {
        foreach(var levelScript in _levelScripts)
        {
            Button button = new Button();
            button.text = levelScript.LevelName;
            button.clicked += () => LoadLecture("dialog");
            levelsHolder.Add(button);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadLecture(string sceneName)
    {
        // Сохраняем имя скрипта для диалоговой сцены
        SceneManager.LoadScene(sceneName);
    }
}

