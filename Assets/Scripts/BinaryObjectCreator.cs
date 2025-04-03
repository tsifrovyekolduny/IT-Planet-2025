using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    [Header("TMP Text Settings")]
    [Tooltip("Base text size (0.01-0.5 for mobile)")]
    [Range(0.01f, 0.5f)] public float textSize = 0.1f;
    [Tooltip("Text area width in world units")]
    public float textAreaWidth = 1.5f;
    public Color textColor = Color.white;
    public Vector3 textOffset = new Vector3(0, 0, -0.1f);
    public FontStyles textStyle = FontStyles.Bold;
    public TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;

    [Header("Auto Size Settings")]
    public bool autoSizeText = true;
    [Range(0.01f, 0.3f)] public float minFontSize = 0.08f;
    [Range(0.1f, 0.8f)] public float maxFontSize = 0.3f;
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

    private const float TMP_SCALE_FACTOR = 0.01f;
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
        tmpFontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (tmpFontAsset == null)
        {
            Debug.LogError("TMP font not found! Please import TMP Essentials");
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

        bool isCorrectOnLeft = Random.Range(0, 2) == 0;

        newPair.leftObject = CreateChoiceObject(
            container.transform,
            Vector3.left * horizontalDistance * 0.5f,
            isCorrectOnLeft ? correctText : incorrectText,
            isCorrectOnLeft);

        newPair.rightObject = CreateChoiceObject(
            container.transform,
            Vector3.right * horizontalDistance * 0.5f,
            isCorrectOnLeft ? incorrectText : correctText,
            !isCorrectOnLeft);

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

        CreateTMPText(obj, text);

        var clickHandler = obj.AddComponent<ChoiceObjectClick>();
        clickHandler.parentSystem = this;
        clickHandler.isCorrectAnswer = isCorrect;

        return obj;
    }

    private void CreateTMPText(GameObject parent, string text)
    {
        GameObject textObj = new GameObject("TMP_Text");
        textObj.transform.SetParent(parent.transform);
        textObj.transform.localPosition = textOffset;

        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(textAreaWidth / TMP_SCALE_FACTOR, 0);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one * TMP_SCALE_FACTOR;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.font = tmpFontAsset;
        tmp.text = text;
        tmp.color = textColor;
        tmp.alignment = textAlignment;
        tmp.fontStyle = textStyle;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.lineSpacing = lineSpacing;

        tmp.fontSize = textSize / TMP_SCALE_FACTOR;
        tmp.enableAutoSizing = autoSizeText;

        if (autoSizeText)
        {
            tmp.fontSizeMin = minFontSize / TMP_SCALE_FACTOR;
            tmp.fontSizeMax = maxFontSize / TMP_SCALE_FACTOR;
        }

        tmp.isOrthographic = true;
        tmp.ForceMeshUpdate();

        if (tmp.textBounds.size.y > 0)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, tmp.textBounds.size.y);
        }
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

        TextMeshPro tmp = obj.GetComponentInChildren<TextMeshPro>();
        if (tmp != null) tmp.color = color;
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!EditorApplication.isPlaying || activePairs.Count == 0) return;

        EditorApplication.delayCall += () => {
            if (this != null)
                UpdateAllPairs();
        };
    }
#endif

    private void UpdateAllPairs()
    {
        foreach (var pair in activePairs)
        {
            if (pair.pairContainer == null) continue;

            // Update positions
            pair.leftObject.transform.localPosition = Vector3.left * horizontalDistance * 0.5f;
            pair.rightObject.transform.localPosition = Vector3.right * horizontalDistance * 0.5f;

            // Update scales
            pair.pairContainer.localScale = Vector3.one * objectScale;
            pair.leftObject.transform.localScale = Vector3.one * objectScale;
            pair.rightObject.transform.localScale = Vector3.one * objectScale;

            // Update visuals
            UpdateChoiceObjectVisuals(pair.leftObject);
            UpdateChoiceObjectVisuals(pair.rightObject);

            // Update texts
            UpdateTextComponent(pair.leftObject);
            UpdateTextComponent(pair.rightObject);

            // Update colors
            SetObjectColor(pair.leftObject, neutralColor);
            SetObjectColor(pair.rightObject, neutralColor);
        }

        MoveCameraToCurrentPair();
    }

    private void UpdateChoiceObjectVisuals(GameObject obj)
    {
        // Update renderer if exists
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = neutralColor;
        }
    }

    private void UpdateTextComponent(GameObject obj)
    {
        TextMeshPro tmp = obj.GetComponentInChildren<TextMeshPro>();
        if (tmp == null) return;

        tmp.fontSize = textSize / TMP_SCALE_FACTOR;
        tmp.enableAutoSizing = autoSizeText;
        tmp.fontSizeMin = minFontSize / TMP_SCALE_FACTOR;
        tmp.fontSizeMax = maxFontSize / TMP_SCALE_FACTOR;
        tmp.color = textColor;
        tmp.fontStyle = textStyle;
        tmp.alignment = textAlignment;
        tmp.lineSpacing = lineSpacing;

        RectTransform rt = tmp.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(textAreaWidth / TMP_SCALE_FACTOR, 0);
        rt.localPosition = textOffset;

        tmp.ForceMeshUpdate();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BinaryChoiceSystem))]
    public class BinaryChoiceSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BinaryChoiceSystem system = (BinaryChoiceSystem)target;

            if (GUILayout.Button("Force Update All Pairs"))
            {
                system.UpdateAllPairs();
            }

            if (GUILayout.Button("Reset System"))
            {
                system.ResetSystem();
            }
        }
    }
#endif
}