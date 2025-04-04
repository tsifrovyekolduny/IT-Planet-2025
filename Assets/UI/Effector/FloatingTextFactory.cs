using UnityEngine;
using System.Collections.Generic;

public enum ScreenPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
    CustomWorldPosition // Для кастомных мировых координат
}
public class FloatingTextFactory : MonoBehaviour
{
    public static FloatingTextFactory Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private FloatingText _prefab;
    [SerializeField] private int _initialPoolSize = 10;
    [SerializeField] private Transform _worldCanvas;

    private Queue<FloatingText> _pool = new Queue<FloatingText>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewInstance();
        }
    }

    private FloatingText CreateNewInstance()
    {
        var instance = Instantiate(_prefab, _worldCanvas);
        instance.gameObject.SetActive(false);
        _pool.Enqueue(instance);
        return instance;
    }

    public void ShowText(string message, ScreenPosition position, Vector2 offset = default)
    {
        Vector3 screenPosition = GetScreenPosition(position, offset);
        ShowTextInternal(message, screenPosition);
    }

    public void ShowText(string message, Vector3 worldPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        ShowTextInternal(message, screenPosition);
    }

    private Vector3 GetScreenPosition(ScreenPosition position, Vector2 offset)
    {
        float x = 0, y = 0;

        switch (position)
        {
            case ScreenPosition.TopLeft:
                x = 0; y = 1;
                break;
            case ScreenPosition.TopCenter:
                x = 0.5f; y = 1;
                break;
            case ScreenPosition.TopRight:
                x = 1; y = 1;
                break;
            case ScreenPosition.MiddleLeft:
                x = 0; y = 0.5f;
                break;
            case ScreenPosition.MiddleCenter:
                x = 0.5f; y = 0.5f;
                break;
            case ScreenPosition.MiddleRight:
                x = 1; y = 0.5f;
                break;
            case ScreenPosition.BottomLeft:
                x = 0; y = 0;
                break;
            case ScreenPosition.BottomCenter:
                x = 0.5f; y = 0;
                break;
            case ScreenPosition.BottomRight:
                x = 1; y = 0;
                break;
        }

        // Конвертируем в пиксельные координаты
        Vector3 result = new Vector3(
            x * Screen.width + offset.x,
            y * Screen.height + offset.y,
            0
        );

        return result;
    }

    private void ShowTextInternal(string message, Vector3 screenPosition)
    {
        FloatingText textInstance;

        // Получаем текст из пула или создаем новый
        if (_pool.Count == 0)
        {
            textInstance = CreateNewInstance();
        }
        else
        {
            textInstance = _pool.Dequeue();
        }

        // Настраиваем позицию и текст
        textInstance.transform.position = screenPosition;
        textInstance.gameObject.SetActive(true);
        textInstance.Initialize(message, screenPosition);

        // Логирование для дебага
        Debug.Log($"Showing text: {message} at {screenPosition}");
    }

    public void ReturnToPool(FloatingText text)
    {
        Destroy(text, 4f);
        text.gameObject.SetActive(false);
        _pool.Enqueue(text);
    }
}