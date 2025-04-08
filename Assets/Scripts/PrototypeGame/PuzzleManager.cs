using UnityEngine;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Настройки")]
    [SerializeField] private PuzzlePiece[] pieces;
    [SerializeField] private bool shuffleOnStart = true;

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

        if (shuffleOnStart)
        {
            ShufflePieces();
        }
    }

    public void ShufflePieces()
    {
        // Получаем границы области перемешивания
        Rect canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;
        Vector2 minPosition = new Vector2(-canvasRect.width / 2 + 100, -canvasRect.height / 2 + 100);
        Vector2 maxPosition = new Vector2(canvasRect.width / 2 - 100, canvasRect.height / 2 - 100);

        // Сохраняем правильные позиции
        foreach (var piece in pieces)
        {
            piece.CorrectPosition = piece.GetComponent<RectTransform>().anchoredPosition;
        }

        // Перемешиваем с учетом границ экрана
        foreach (var piece in pieces)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y)
            );
            piece.GetComponent<RectTransform>().anchoredPosition = randomPos;
            piece.ResetPiece();
        }
    }


    public void CheckPuzzleComplete()
    {
        if (pieces.All(p => p.IsInPlace))
        {
            Debug.Log("Пазл собран!");
            // Дополнительные действия при завершении
            // Например:
            // ShowVictoryScreen();
            // PlayConfettiEffect();
        }
    }

    // Для кнопки "Перемешать"
    public void ShufflePiecesButton()
    {
        ShufflePieces();
    }
}