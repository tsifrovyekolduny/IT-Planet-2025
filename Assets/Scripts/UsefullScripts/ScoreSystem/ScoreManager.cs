using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Настройки")]
    [SerializeField] private int _currentScore = 0;

    [Header("События")]
    public UnityEvent<int> onScoreChanged = new UnityEvent<int>();

    private void Awake()
    {
        // Исправление устаревшего метода
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

        // Альтернативный вариант инициализации:
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        // Более безопасная проверка дубликатов
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
        Debug.Log($"Score updated: {_currentScore} (+{amount})");
    }

    public int GetCurrentScore() => _currentScore;

    public void ResetScore()
    {
        _currentScore = 0;
        onScoreChanged.Invoke(_currentScore);
    }

    // Оптимизированный метод поиска для редких случаев
    public static ScoreManager FindInstance()
    {
        return Instance ?? FindFirstObjectByType<ScoreManager>();
    }
}