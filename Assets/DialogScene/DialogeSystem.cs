using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
//using DG.Tweening; // Для анимаций (нужен Asset DOTween)

public class DialogueSystem : MonoBehaviour
{
    public GameObject messagePrefab;
    public Transform contentParent;
    public Button nextButton;

    private DialogueParser parser;
    private int currentLine = 0;

    void Start()
    {
        parser = GetComponent<DialogueParser>();
        nextButton.onClick.AddListener(ShowNextLine);
    }

    void ShowNextLine()
    {
        if (currentLine >= parser.dialogueLines.Count)
        {
            return;
        };

        DialogueLine line = parser.dialogueLines[currentLine];
        CreateMessage(line);
        currentLine++;
    }

    void CreateMessage(DialogueLine line)
    {
        GameObject newMessage = Instantiate(messagePrefab, contentParent);
        TMP_Text messageText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = line.text;

        // Настройка позиции и цвета в зависимости от роли
        RectTransform rect = newMessage.GetComponent<RectTransform>();
        Image bg = newMessage.GetComponent<Image>();

        //if (line.role == "Роль1")
        //{
        //    rect.pivot = new Vector2(1, 0);
        //    rect.anchorMin = rect.anchorMax = new Vector2(1, 0);
        //    bg.color = new Color(0.2f, 0.6f, 1f); // Синий
        //}
        //else
        //{
        //    rect.pivot = new Vector2(0, 0);
        //    rect.anchorMin = rect.anchorMax = new Vector2(0, 0);
        //    bg.color = new Color(0.3f, 0.8f, 0.4f); // Зелёный
        //}

        // Анимация
        newMessage.transform.localScale = Vector3.zero;
        //newMessage.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);

        // Автопрокрутка
        Canvas.ForceUpdateCanvases();
        contentParent.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        contentParent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        //contentParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }
}