using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class DialogueSystem : MonoBehaviour
{
    const string ROLE_1 = "П";
    const string ROLE_2 = "Т";
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private TextAsset scriptFile;
    [SerializeField] private VisualTreeAsset messageTemplate;
    [SerializeField] private Texture2D teacherSprite;
    [SerializeField] private Texture2D sholarSprite;

    private VisualElement root;
    private VisualElement messagesContainer;
    private Button nextButton;
    private DialogueParser parser;
    private int currentLine = 0;

    void Start()
    {
        parser = gameObject.AddComponent<DialogueParser>();
        if (scriptFile == null)
        {
            scriptFile = GameManager.Instance.GetLevel().Item1;
            currentLine = GameManager.Instance.GetLevel().Item2;
        }
        parser.scriptFile = scriptFile;
        parser.ParseScript();

        // init UI
        root = uiDocument.rootVisualElement;
        messagesContainer = root.Q<VisualElement>("messages-container");
        nextButton = root.Q<Button>("next-button");
        nextButton.clicked += ShowNextLine;

        // if loaded
        if (currentLine != 0)
        {
            RollToLine(currentLine);
        }
    }

    private void RollToLine(int endLine)
    {
        int lineIndex = 0;
        while (lineIndex < endLine)
        {
            DialogueLine line = parser.dialogueLines[lineIndex];
            CreateMessage(line);
            ++lineIndex;
        }
    }

    void ShowNextLine()
    {
        if (currentLine >= parser.dialogueLines.Count) return;

        DialogueLine line = parser.dialogueLines[currentLine];
        CreateMessage(line);
        currentLine++;
    }

    void GoOnGame(string gameScene)
    {
        SceneManager.LoadScene(gameScene);
        GameManager.Instance.SaveProgress(currentLine, parser.scriptFile.name);
    }

    void CreateMessage(DialogueLine line)
    {
        TemplateContainer messageElement = messageTemplate.Instantiate();
        VisualElement messageRoot = messageElement.Q<VisualElement>("template");

        if (line.role == "Сцена")
        {
            Button button = new Button();
            button.text = "Перейти к игре";
            // button.AddToClassList("scene-button");
            button.clicked += () => GoOnGame(line.text);

            messagesContainer.Add(button);
        }
        else
        {
            string role = line.role == ROLE_1 ? "role1" : "role2";


            messageRoot.AddToClassList($"message-{role}");

            // Заполнение данных
            VisualElement avatar = messageRoot.Q<VisualElement>("avatar");
            Label nameLabel = messageRoot.Q<Label>("title");
            Label textLabel = messageRoot.Q<Label>("body");

            avatar.style.backgroundImage = line.role == ROLE_1 ? teacherSprite : sholarSprite; // Подставляем аватар
            nameLabel.text = line.role;
            textLabel.text = line.text;

            textLabel.AddToClassList($"body-text-{role}");

            messagesContainer.Add(messageRoot);

        }
        ScrollToBottom();

        // Анимация появления
        //message.schedule.Execute(() => {
        //    message.AddToClassList("message-visible");
        //}).StartingIn(10); // Небольшая задержка для корректного применения стилей
    }

    void ScrollToBottom()
    {
        ScrollView scrollView = root.Q<ScrollView>();
        var lastElem = messagesContainer.Children().Last();

        float downValue = scrollView.verticalScroller.highValue + lastElem.style.height.value.value;

        scrollView.verticalScroller.value = scrollView.verticalScroller.highValue > 0 ?
                                             downValue : 0;
    }
}