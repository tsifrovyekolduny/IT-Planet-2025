using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class BinaryChoiceSystem : MonoBehaviour
{
    [System.Serializable]
    public class ChoicePair
    {
        public GameObject leftObject;
        public GameObject rightObject;
        public Transform pairContainer;
    }

    [Header("Base Settings")]
    public Transform startObject;
    public Vector3 firstPairOffset = new Vector3(0, 2f, 0);
    public GameObject choicePrefab;

    [Header("Answer Settings")]
    public List<string> correctAnswers = new List<string>();
    public List<string> incorrectAnswers = new List<string>();
    public string defaultAnswerText = "?";
    public bool shuffleAnswers = true;

    [Header("Text Settings")]
    public bool useTextMeshPro = true;
    public Color textColor = Color.white;
    public float textSize = 0.5f;
    public Vector3 textOffset = new Vector3(0, 0, -0.1f);
    public FontStyles textStyle = FontStyles.Bold;
    public TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;

    [Header("Text Wrapping Settings")]
    public float textAreaWidth = 2f;
    public bool autoSizeText = true;
    public float minFontSize = 0.3f;
    public float maxFontSize = 0.7f;
    public float lineSpacing = 0.5f;

    [Header("Spacing Settings")]
    public float horizontalDistance = 2f;
    public float verticalDistance = 2f;
    [Range(0.1f, 2f)] public float objectScale = 0.8f;

    [Header("Visual Settings")]
    public Color neutralColor = Color.gray;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;

    [Header("Animation Settings")]
    [Range(0.1f, 2f)] public float spawnDuration = 0.7f;
    [Range(0.1f, 2f)] public float cameraMoveSpeed = 1f;
    public float newPairDelay = 0.5f;
    public AnimationCurve spawnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private List<ChoicePair> activePairs = new List<ChoicePair>();
    private Transform currentActivePair;
    private bool isInteractable = true;
    private Camera mainCamera;
    private int currentAnswerIndex = 0;
    private bool answersExhausted = false;
    private List<string> shuffledCorrect = new List<string>();
    private List<string> shuffledIncorrect = new List<string>();
    private TMP_FontAsset tmpFontAsset;

    private void Awake()
    {
        LoadFontResources();
    }

    private void LoadFontResources()
    {
        if (useTextMeshPro)
        {
            tmpFontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (tmpFontAsset == null)
            {
                Debug.LogWarning("TMP font not found. Falling back to TextMesh.");
                useTextMeshPro = false;
            }
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        ValidateAnswerLists();
        InitializeSystem();
    }

    private void ValidateAnswerLists()
    {
        if (correctAnswers.Count == 0 || incorrectAnswers.Count == 0)
        {
            Debug.LogError("Answer lists cannot be empty!");
            enabled = false;
            return;
        }

        shuffledCorrect = new List<string>(correctAnswers);
        shuffledIncorrect = new List<string>(incorrectAnswers);

        if (shuffleAnswers)
        {
            ShuffleList(shuffledCorrect);
            ShuffleList(shuffledIncorrect);
        }
    }

    private void ShuffleList(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void InitializeSystem()
    {
        if (startObject == null)
        {
            Debug.LogError("Start Object not assigned!");
            return;
        }

        Vector3 spawnPosition = startObject.position + firstPairOffset;
        CreateNewPair(spawnPosition, null);
    }

    private void CreateNewPair(Vector3 centerPosition, Transform parent)
    {
        if (answersExhausted) return;

        ChoicePair newPair = new ChoicePair();

        GameObject container = new GameObject($"Pair_{activePairs.Count}");
        container.transform.position = centerPosition;
        newPair.pairContainer = container.transform;

        string correctText = GetAnswerText(true);
        string incorrectText = GetAnswerText(false);

        newPair.leftObject = CreateChoiceObject(
            container.transform,
            Vector3.left * horizontalDistance * 0.5f,
            correctText,
            true);

        newPair.rightObject = CreateChoiceObject(
            container.transform,
            Vector3.right * horizontalDistance * 0.5f,
            incorrectText,
            false);

        if (Random.Range(0, 2) == 0)
        {
            (newPair.leftObject, newPair.rightObject) = (newPair.rightObject, newPair.leftObject);
        }

        activePairs.Add(newPair);
        currentActivePair = container.transform;
        currentAnswerIndex++;

        StartCoroutine(AnimateSpawn(container.transform));
        MoveCameraToCurrentPair();
    }

    private GameObject CreateChoiceObject(Transform parent, Vector3 localPosition, string text, bool isCorrect)
    {
        GameObject obj = Instantiate(choicePrefab, parent);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = Vector3.one * objectScale;
        obj.name = $"Choice_{(isCorrect ? "Correct" : "Incorrect")}";

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null) rend.material.color = neutralColor;

        CreateTextComponent(obj, text);

        var clickHandler = obj.AddComponent<ChoiceObjectClick>();
        clickHandler.parentSystem = this;
        clickHandler.isCorrectAnswer = isCorrect;

        return obj;
    }

    private void CreateTextComponent(GameObject parent, string text)
    {
        if (useTextMeshPro)
        {
            CreateTMPText(parent, text);
        }
        else
        {
            CreateRegularText(parent, text);
        }
    }

    private void CreateTMPText(GameObject parent, string text)
    {
        GameObject textObj = new GameObject("TMP_Text");
        textObj.transform.SetParent(parent.transform);
        textObj.transform.localPosition = textOffset;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        RectTransform rt = textObj.GetComponent<RectTransform>();

        // Актуальные настройки переноса текста (2023+)
        tmp.textWrappingMode = TextWrappingModes.Normal; // Замена enableWordWrapping
        tmp.overflowMode = TextOverflowModes.Truncate; // Или OverflowModes.Ellipsis
        tmp.wordWrappingRatios = 0.6f;

        // Настройка контейнера
        rt.sizeDelta = new Vector2(textAreaWidth, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        // Автоматическое масштабирование текста
        if (autoSizeText)
        {
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = minFontSize;
            tmp.fontSizeMax = maxFontSize;
        }
        else
        {
            tmp.fontSize = textSize;
        }

        // Основные параметры текста
        tmp.font = tmpFontAsset;
        tmp.text = text;
        tmp.color = textColor;
        tmp.alignment = textAlignment;
        tmp.fontStyle = textStyle;
        tmp.isOrthographic = true;
        tmp.sortingLayerID = SortingLayer.NameToID("UI");
        tmp.sortingOrder = 1;

        // Принудительное обновление меша
        tmp.ForceMeshUpdate();

        // Подгонка высоты под контент
        if (tmp.textBounds.size.y > rt.sizeDelta.y)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, tmp.textBounds.size.y * 1.1f);
        }

        // Создание материала при необходимости
        if (tmp.fontSharedMaterial == null)
        {
            tmp.fontSharedMaterial = new Material(Shader.Find("TextMeshPro/Distance Field"));
            tmp.fontSharedMaterial.renderQueue = 4000;
        }
    }

    private void CreateRegularText(GameObject parent, string text)
    {
        GameObject textObj = new GameObject("TextMesh");
        textObj.transform.SetParent(parent.transform);
        textObj.transform.localPosition = textOffset;

        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        tm.text = WrapText(text, 20); // 20 chars per line
        tm.color = textColor;
        tm.characterSize = autoSizeText ? Mathf.Clamp(textSize, minFontSize, maxFontSize) : textSize;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.fontSize = 24;
        tm.fontStyle = FontStyle.Bold;
        tm.lineSpacing = lineSpacing;

        MeshRenderer mr = textObj.GetComponent<MeshRenderer>();
        mr.sharedMaterial = tm.font.material;
        mr.sharedMaterial.shader = Shader.Find("GUI/Text Shader");
    }

    private string WrapText(string input, int maxCharsPerLine)
    {
        if (string.IsNullOrEmpty(input) || maxCharsPerLine <= 0)
            return input;

        string[] words = input.Split(' ');
        string result = "";
        string line = "";

        foreach (string word in words)
        {
            if (line.Length > 0 && line.Length + word.Length > maxCharsPerLine)
            {
                result += line + "\n";
                line = "";
            }

            if (line.Length > 0)
                line += " " + word;
            else
                line = word;
        }

        if (line.Length > 0)
            result += line;

        return result;
    }

    private string GetAnswerText(bool isCorrect)
    {
        var answerList = isCorrect ? shuffledCorrect : shuffledIncorrect;

        if (currentAnswerIndex < answerList.Count)
            return answerList[currentAnswerIndex];

        answersExhausted = true;
        return defaultAnswerText;
    }

    public void HandleChoice(GameObject chosenObject, bool isCorrect)
    {
        if (!isInteractable || answersExhausted) return;

        StartCoroutine(ProcessChoice(chosenObject, isCorrect));
    }

    private IEnumerator ProcessChoice(GameObject chosenObject, bool isCorrect)
    {
        isInteractable = false;

        SetObjectColor(chosenObject, isCorrect ? correctColor : incorrectColor);
        yield return new WaitForSeconds(newPairDelay);

        if (isCorrect && !answersExhausted)
        {
            Vector3 newCenterPosition = chosenObject.transform.position + Vector3.up * verticalDistance;
            CreateNewPair(newCenterPosition, chosenObject.transform);
        }

        isInteractable = true;
    }

    private void SetObjectColor(GameObject obj, Color color)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null) rend.material.color = color;

        if (useTextMeshPro)
        {
            TextMeshPro tmp = obj.GetComponentInChildren<TextMeshPro>();
            if (tmp != null) tmp.color = color;
        }
        else
        {
            TextMesh tm = obj.GetComponentInChildren<TextMesh>();
            if (tm != null) tm.color = color;
        }
    }

    private IEnumerator AnimateSpawn(Transform target)
    {
        float elapsed = 0f;
        Vector3 initialScale = target.localScale;
        target.localScale = Vector3.zero;

        while (elapsed < spawnDuration)
        {
            float progress = spawnCurve.Evaluate(elapsed / spawnDuration);
            target.localScale = Vector3.Lerp(Vector3.zero, initialScale, progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = initialScale;
    }

    private void MoveCameraToCurrentPair()
    {
        if (currentActivePair == null) return;

        Vector3 targetPosition = new Vector3(
            currentActivePair.position.x,
            currentActivePair.position.y,
            mainCamera.transform.position.z
        );

        StartCoroutine(MoveCamera(targetPosition));
    }

    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        Vector3 startPosition = mainCamera.transform.position;
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);

        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.05f)
        {
            float progress = (Time.time - startTime) * cameraMoveSpeed / journeyLength;
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
    }

    public void ResetSystem()
    {
        StopAllCoroutines();
        foreach (var pair in activePairs)
        {
            if (pair.pairContainer != null)
                Destroy(pair.pairContainer.gameObject);
        }
        activePairs.Clear();
        currentActivePair = null;
        currentAnswerIndex = 0;
        answersExhausted = false;
        isInteractable = true;
        ValidateAnswerLists();
        InitializeSystem();
    }
}