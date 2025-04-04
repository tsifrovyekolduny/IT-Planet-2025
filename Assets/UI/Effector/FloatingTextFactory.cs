using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class FloatingTextSpawner : Singletone<FloatingTextSpawner>
{
    [Header("Настройки")]
    [SerializeField] private FloatingText _prefab;
    [SerializeField] private Transform _worldCanvas;
    [SerializeField] private Transform _screenCanvas;
    [SerializeField] private float _defaultDuration = 1.5f;

    public void SpawnText(string message, Vector3 position, bool isWorldSpace = true, float duration = -1)
    {
        if (_prefab == null || _worldCanvas == null || _screenCanvas == null)
        {
            Debug.LogError("Не назначены префаб или канвасы!");
            return;
        }

        float actualDuration = duration > 0 ? duration : _defaultDuration;
        var targetCanvas = isWorldSpace ? _worldCanvas : _screenCanvas;

        var instance = Instantiate(_prefab, targetCanvas);
        instance.Setup(message, position, actualDuration, isWorldSpace);
    }

    // Удобные врапперы
    public static void Spawn(string message, Vector3 position, bool isWorldSpace = true)
        => Instance?.SpawnText(message, position, isWorldSpace);

    public static void Spawn(string message, Vector3 position, float duration, bool isWorldSpace = true)
        => Instance?.SpawnText(message, position, isWorldSpace, duration);
}