using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;

public class LevelsMenu : MonoBehaviour
{
    [Serializable]    
    
    private class LevelScript
    {
        public string LevelName;
        public TextAsset Script;
    }

    [SerializeField] private LevelScript[] _levelScripts;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadLecture(string sceneName, string scriptName)
    {
        // Сохраняем имя скрипта для диалоговой сцены
        SceneManager.LoadScene(sceneName);
    }
}

