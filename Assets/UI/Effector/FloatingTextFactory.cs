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
    [SerializeField] private Vector2 _defaultOffset = new Vector2(0, 50f);

    public void SpawnText(string message, Vector3 position, bool isWorldSpace = true,
                         float duration = -1, Vector2? offset = null,
                         TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        if (_prefab == null || _worldCanvas == null || _screenCanvas == null)
        {
            Debug.LogError("Не назначены префаб или канвасы!");
            return;
        }

        float actualDuration = duration > 0 ? duration : _defaultDuration;
        Vector2 actualOffset = offset ?? _defaultOffset;

        if (isWorldSpace)
        {
            SpawnWorldSpaceText(message, position, actualDuration, actualOffset);
        }
        else
        {
            SpawnScreenSpaceText(message, position, actualDuration, actualOffset, anchor);
        }
    }

    private void SpawnWorldSpaceText(string message, Vector3 worldPosition,
                                   float duration, Vector2 offset)
    {
        var instance = Instantiate(_prefab, _worldCanvas);
        instance.SetupWorldSpace(message, worldPosition, duration, offset);
    }

    private void SpawnScreenSpaceText(string message, Vector2 screenPosition,
                                    float duration, Vector2 offset, TextAnchor anchor)
    {
        // Создаем только Text из префаба
        var newObj = new GameObject();

        var temp = newObj.AddComponent<TextMeshProUGUI>();
        temp = _prefab.TextLabel;

        Instantiate(newObj, _screenCanvas);

        var textInstance = newObj.AddComponent<FloatingText>();
        textInstance.SetupScreenSpace(message, screenPosition + offset, duration, anchor);
    }

    // Удобные врапперы
    public void Spawn(string message, Vector3 worldPosition,
                           Vector2? offset = null)
        => Instance?.SpawnText(message, worldPosition, true, -1, offset);

    public void SpawnScreen(string message, Vector2 screenPosition,
                                 TextAnchor anchor = TextAnchor.MiddleCenter,
                                 Vector2? offset = null)
        => Instance?.SpawnText(message, screenPosition, false, -1, offset, anchor);
}