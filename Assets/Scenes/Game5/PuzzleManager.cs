using UnityEngine;
using System.Linq;

public class PuzzleManager : MonoBehaviour, IMiniGame
{
    public static PuzzleManager Instance;

    [Header("Настройки")]
    [SerializeField] private PuzzlePiece[] pieces;
    [SerializeField] private bool shuffleOnStart = true;

    private bool _isComplete = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (shuffleOnStart)
        {
            ShufflePieces();
        }
    }

    public void ShufflePieces()
    {
        if (pieces == null || pieces.Length == 0) return;

        // Получаем границы Canvas
        Rect canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;

        // Рассчитываем безопасные зоны
        float excludedTopPercent = 0.12f; // 12% сверху
        float excludedTop = canvasRect.height * excludedTopPercent;

        Vector2 minPosition = new Vector2(
            -canvasRect.width / 2 + 100,
            -canvasRect.height / 2 + 100);

        Vector2 maxPosition = new Vector2(
            canvasRect.width / 2 - 100,
            canvasRect.height / 2 - 100 - excludedTop); // Исключаем верхние 12%

        // Сохраняем правильные позиции
        foreach (var piece in pieces)
        {
            piece.CorrectPosition = piece.GetComponent<RectTransform>().anchoredPosition;
        }

        // Перемешиваем с учетом новых границ
        foreach (var piece in pieces)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y)
            );

            piece.GetComponent<RectTransform>().anchoredPosition = randomPos;
        }
    }


    public void CheckPuzzleComplete()
    {
        if (pieces.All(p => p.IsInPlace))
        {
            Debug.Log("Пазл собран!");
            _isComplete = true;
        }
    }

    // Для кнопки "Перемешать"
    public void ShufflePiecesButton()
    {
        ShufflePieces();
    }

    public bool CheckForComplete()
    {
        return _isComplete;
    }
}