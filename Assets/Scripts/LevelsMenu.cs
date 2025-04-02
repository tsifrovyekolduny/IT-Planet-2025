using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;

public class LevelsMenu : MonoBehaviour
{
    [SerializeField] private List<LevelScript> _levelScripts;
    public UIDocument LevelUI;
    public VisualTreeAsset LevelTemplate;

    private VisualElement root;
    private VisualElement levelsHolder;
    private ProgressBar progressBar;

    [Serializable]
    public class LevelScript
    {
        public string LevelName;
        public TextAsset Script;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitLevelScripts();
        InitializeUI();
        RefreshLevelsDisplay();
    }

    void InitLevelScripts()
    {
        Direction direction = GameManager.Instance.CurrentDirection;
        _levelScripts = direction.Levels;    
    }

    void InitializeUI()
    {
        root = LevelUI.rootVisualElement;
        levelsHolder = root.Q<VisualElement>("levels-holder");
        progressBar = root.Q<ProgressBar>("progress-bar");

        root.Q<Button>("back-btn").clicked += () => SceneManager.LoadScene("DirectionsHub");
        root.Q<Label>("hub-name").text = GameManager.Instance.CurrentDirection.Name;
        root.Q<Button>("continue-btn").clicked += () => GameManager.Instance.SetLoadedLevel();
    }

    void RefreshLevelsDisplay()
    {
        levelsHolder.Clear();

        // �������� ������� ��������
        var (savedLine, scriptName) = ProgressManager.Instance.GetDirectionProgress(GameManager.Instance.CurrentDirection.name);
        LevelScript currentLevelScript = _levelScripts.Where(d => d.Script.name == scriptName).First();
        int currentLevel = _levelScripts.IndexOf(currentLevelScript);
        int scriptEndlineCount = currentLevelScript.Script.text.Split('\n').Length;

        float firstPart = currentLevel;
        if (_levelScripts[0].LevelName == "00")
        {
            firstPart += 1;
        }
        float secondPart = savedLine / scriptEndlineCount;

        float progressValue = (firstPart + secondPart) / _levelScripts.Count;        
        progressBar.value = progressValue * 100;
        progressBar.title = $"��������: {Mathf.Round(progressValue * 100)}%";

        // ������� ������ �������
        for (int i = 0; i < _levelScripts.Count; i++)
        {
            var level = _levelScripts[i];
            TemplateContainer levelElement = LevelTemplate.Instantiate();

            // ������� �������� � �������
            Button statusButton = levelElement.Q<Button>("status-button");
            Label nameLabel = levelElement.Q<Label>("name-label");
            Label descriptionLabel = levelElement.Q<Label>("description-label");

            // ��������� ������
            nameLabel.text = level.LevelName;
            // descriptionLabel.text = level.description; ���� ��� ��������

            // ���������� ������ ������
            if (i < currentLevel)
            {
                statusButton.text = "��������";
                statusButton.AddToClassList("completed");
                statusButton.clicked += () => LoadLevel(level.Script, 0);
            }
            else if (i == currentLevel)
            {
                statusButton.text = "������";
                statusButton.AddToClassList("current");
                statusButton.clicked += () => LoadLevel(level.Script, 0);
            }
            else
            {
                statusButton.text = "�� ��������";
                statusButton.AddToClassList("locked");
                statusButton.SetEnabled(false);
            }

            levelsHolder.Add(levelElement);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadLevel(TextAsset scriptFile, int line)
    {
        // ��������� ��� ������� ��� ���������� �����
        GameManager.Instance.SetLevel(scriptFile, line);
    }
}

