using UnityEngine;
using System.Linq;

public class PuzzleManager : MonoBehaviour, IMiniGame
{
    public static PuzzleManager Instance;

    [Header("���������")]
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

        // �������� ������� ������� �������������
        Rect canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect;
        Vector2 minPosition = new Vector2(-canvasRect.width / 2 + 100, -canvasRect.height / 2 + 100);
        Vector2 maxPosition = new Vector2(canvasRect.width / 2 - 100, canvasRect.height / 2 - 100);

        // ��������� ���������� �������
        foreach (var piece in pieces)
        {
            piece.CorrectPosition = piece.GetComponent<RectTransform>().anchoredPosition;
        }

        // ������������ � ������ ������ ������
        foreach (var piece in pieces)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y)
            );
            Debug.Log($"Shuffling pieces. Canvas size: {canvasRect.width}x{canvasRect.height}");
            Debug.Log($"Random position for piece: {randomPos}");
            piece.GetComponent<RectTransform>().anchoredPosition = randomPos;
        }

    }


    public void CheckPuzzleComplete()
    {
        if (pieces.All(p => p.IsInPlace))
        {
            Debug.Log("���� ������!");
            _isComplete = true;
        }
    }

    // ��� ������ "����������"
    public void ShufflePiecesButton()
    {
        ShufflePieces();
    }

    public bool CheckForComplete()
    {
        return _isComplete;
    }
}