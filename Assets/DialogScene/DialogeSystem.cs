using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
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
        TemplateContainer messageElement = messageTemplate.Instantiate();
        VisualElement messageRoot = messageElement.Q<VisualElement>("template");        
        
        messageRoot.AddToClassList(line.role == "Роль1" ? "message-role1" : "message-role2");

        // Заполнение данных
        VisualElement avatar = messageRoot.Q<VisualElement>("avatar");
        Label nameLabel = messageRoot.Q<Label>("title");
        Label textLabel = messageRoot.Q<Label>("body");
        
        avatar.style.backgroundImage = line.role == "Роль1" ? sholarSprite : teacherSprite; // Подставляем аватар
        nameLabel.text = line.role;
        textLabel.text = line.text;

        messagesContainer.Add(messageRoot);
        // ScrollToBottom();

        // Анимация появления
        //message.schedule.Execute(() => {
        //    message.AddToClassList("message-visible");
        //}).StartingIn(10); // Небольшая задержка для корректного применения стилей
    }

    void ScrollToBottom()
    {
        ScrollView scrollView = root.Q<ScrollView>();
        scrollView.scrollOffset = new Vector2(0, scrollView.contentContainer.worldBound.height);
    }
}