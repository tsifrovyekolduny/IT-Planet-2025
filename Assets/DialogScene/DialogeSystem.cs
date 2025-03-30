using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private TextAsset scriptFile;

    private VisualElement root;
    private VisualElement messagesContainer;
    private Button nextButton;
    private DialogueParser parser;
    private int currentLine = 0;

    void Start()
    {
        parser = gameObject.AddComponent<DialogueParser>();
        parser.scriptFile = scriptFile;
        parser.ParseScript();

        root = uiDocument.rootVisualElement;
        messagesContainer = root.Q<VisualElement>("messages-container");
        nextButton = root.Q<Button>("next-button");

        nextButton.clicked += ShowNextLine;
    }

    void ShowNextLine()
    {
        if (currentLine >= parser.dialogueLines.Count) return;

        DialogueLine line = parser.dialogueLines[currentLine];
        CreateMessage(line);
        currentLine++;
    }

    void CreateMessage(DialogueLine line)
    {
        Label message = new Label(line.text);
        message.AddToClassList("message");
        message.AddToClassList(line.role == "Роль1" ? "message-role1" : "message-role2");

        messagesContainer.Add(message);
        ScrollToBottom();

        // Анимация появления
        message.schedule.Execute(() => {
            message.AddToClassList("message-visible");
        }).StartingIn(10); // Небольшая задержка для корректного применения стилей
    }

    void ScrollToBottom()
    {
        ScrollView scrollView = root.Q<ScrollView>();
        scrollView.scrollOffset = new Vector2(0, scrollView.contentContainer.worldBound.height);
    }
}