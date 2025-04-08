using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] const string ROLE_1 = "П";
    [SerializeField] const string ROLE_2 = "Т";
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private TextAsset scriptFile;
    [SerializeField] private VisualTreeAsset messageTemplate;
    [SerializeField] private Texture2D teacherSprite;
    [SerializeField] private Texture2D sholarSprite;
    [SerializeField] private float typingSpeed = 0.3f;

    private VisualElement root;
    private VisualElement messagesContainer;
    private Button nextButton;
    private DialogueParser parser;
    private int currentLine = 0;

    // Все необходимое для печатания анимации
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private Label currentTextLabel;
    private string currentFullText;

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
        InitializeUI();

        // if loaded
        if (currentLine != 0)
        {
            RollToLine(currentLine);
        }
    }

    private void InitializeUI()
    {
        root = uiDocument.rootVisualElement;
        messagesContainer = root.Q<VisualElement>("messages-container");
        nextButton = root.Q<Button>("next-button");
        nextButton.clicked += HandleNextButtonClick;

        Button backButton = root.Q<Button>("back-to-levels-btn");
        backButton.clicked += () => GameManager.Instance.ToLevelsHub();
    }

    private void RollToLine(int endLine)
    {
        int lineIndex = 0;
        while (lineIndex < endLine)
        {
            DialogueLine line = parser.dialogueLines[lineIndex];
            CreateMessage(line, false);
            ++lineIndex;
        }
    }

    void HandleNextButtonClick()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
            currentTextLabel.text = currentFullText;
        }
        else
        {
            // Переходим к следующей реплике
            ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (currentLine >= parser.dialogueLines.Count) return;

        DialogueLine line = parser.dialogueLines[currentLine];
        CreateMessage(line, true);
        currentLine++;
    }

    void GoOnGame(string gameScene)
    {
        SceneManager.LoadScene(gameScene);
        GameManager.Instance.SaveProgress(currentLine, parser.scriptFile.name);
    }

    void CreateMessage(DialogueLine line, bool animate)
    {
        TemplateContainer messageElement = messageTemplate.Instantiate();
        VisualElement messageRoot = messageElement.Q<VisualElement>("template");

        if (line.role == "Сцена")
        {
            CreateGameTransitionButton(line.text);
            return;
        }
        
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

        if (animate)
        {
            // Запускаем анимацию печатания
            currentTextLabel = textLabel;
            currentFullText = line.text;
            typingCoroutine = StartCoroutine(TypeText(line.text, textLabel));
        }
        else
        {
            textLabel.text = line.text;
        }

        StartCoroutine(SmoothScrollToBottom());        
    }

    void CreateGameTransitionButton(string gameScene)
    {
        Button button = new Button();
        button.text = "Перейти к игре";
        button.AddToClassList("action-button");
        button.clicked += () => GoOnGame(gameScene);
        messagesContainer.Add(button);
        StartCoroutine(SmoothScrollToBottom());
    }

    IEnumerator TypeText(string text, Label label)
    {
        isTyping = true;
        label.text = "";

        foreach (char letter in text.ToCharArray())
        {
            label.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
    IEnumerator SmoothScrollToBottom()
    {
        yield return new WaitForEndOfFrame();

        ScrollView scrollView = root.Q<ScrollView>();
        if (scrollView == null) yield break;

        // Получаем актуальные размеры
        float contentHeight = messagesContainer.layout.height;
        float viewportHeight = scrollView.contentViewport.layout.height;

        // Рассчитываем максимально возможное значение скролла
        float maxScrollValue = Mathf.Max(0, contentHeight - viewportHeight);

        // Если контент помещается во вьюпорт - не скроллим
        if (maxScrollValue <= 0) yield break;

        float startValue = scrollView.verticalScroller.value;
        float targetValue = maxScrollValue; // Используем расчетное значение вместо highValue
        float duration = 0.3f;
        float elapsed = 0f;

        // Плавный скролл с учетом ограничений
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            scrollView.verticalScroller.value = Mathf.Min(newValue, maxScrollValue);
            yield return null;
        }

        // Фиксируем конечное положение
        scrollView.verticalScroller.value = targetValue;
    }    
}