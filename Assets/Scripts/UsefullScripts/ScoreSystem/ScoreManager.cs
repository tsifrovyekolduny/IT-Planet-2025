using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour, IMiniGame
{
    public static ScoreManager Instance { get; private set; }

    [Header("���������")]
    [Rename("������� ���������� �����")]
    [SerializeField] private int _currentScore = 0;
    [Rename("������������ ���������� �����")]
    [SerializeField] private int _maxScore = 0;

    [Header("�������")]
    public UnityEvent<int> onScoreChanged = new UnityEvent<int>();


    private void Awake()
    {
        // ����������� ����������� ������
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // �������������� ������� �������������:
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        // ����� ���������� �������� ����������
        var existingInstances = FindObjectsByType<ScoreManager>(FindObjectsSortMode.None);
        if (existingInstances.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddScore(int amount)
    {
        _currentScore += amount;
        onScoreChanged.Invoke(_currentScore);
        if (_currentScore >= _maxScore)
        {
            CheckForComplete();
        }
    }

    public int GetCurrentScore() => _currentScore;

    public void ResetScore()
    {
        _currentScore = 0;
        onScoreChanged.Invoke(_currentScore);
    }

    public bool CheckForComplete()
    {
        return true;
    }

    // ���������������� ����� ������ ��� ������ �������
    public static ScoreManager FindInstance()
    {
        return Instance ?? FindFirstObjectByType<ScoreManager>();
    }
}