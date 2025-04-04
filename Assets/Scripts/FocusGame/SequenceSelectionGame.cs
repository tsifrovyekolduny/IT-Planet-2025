using UnityEngine;
using System.Collections.Generic;

public class SequenceSelectionGame : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private List<GameObject> selectableObjects;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private bool autoSetupObjects = true;

    [Header("Эффекты")]
    [SerializeField] private float cameraShakeIntensity = 0.1f;
    [SerializeField] private float cameraShakeDuration = 0.5f;

    [Header("Анимация завершения")]
    [SerializeField] private float completionSpacing = 2f;
    [SerializeField] private float completionAnimationDuration = 1f;
    [SerializeField] private float distanceFromCamera = 5f; // Дистанция от камеры

    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private int currentExpectedIndex = 0;
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private bool isShaking = false;
    private float shakeTimeRemaining = 0f;
    private bool isCompleted = false;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
        }

        if (autoSetupObjects)
        {
            AutoSetupObjects();
        }

        SaveOriginalProperties();
    }

    private void AutoSetupObjects()
    {
        foreach (var obj in selectableObjects)
        {
            if (obj != null)
            {
                if (obj.GetComponent<Collider>() == null)
                {
                    obj.AddComponent<BoxCollider>();
                }

                var selectable = obj.GetComponent<SelectableObject>() ?? obj.AddComponent<SelectableObject>();
                selectable.gameManager = this;
            }
        }
    }

    private void SaveOriginalProperties()
    {
        foreach (var obj in selectableObjects)
        {
            if (obj != null)
            {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    originalColors[obj] = renderer.material.color;
                }

                originalPositions[obj] = obj.transform.position;
            }
        }
    }

    public void OnObjectSelected(GameObject selectedObject)
    {
        if (isCompleted) return;

        int selectedIndex = selectableObjects.IndexOf(selectedObject);
        if (selectedIndex == -1) return;

        if (selectedIndex == currentExpectedIndex)
        {
            var renderer = selectedObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = selectedColor;
            }

            currentExpectedIndex++;

            if (currentExpectedIndex >= selectableObjects.Count)
            {
                CompleteSequence();
            }
        }
        else
        {
            ResetSelection();
            ShakeCamera();
        }
    }

    private void CompleteSequence()
    {
        isCompleted = true;
        Debug.Log("Последовательность завершена!");

        // Рассчитываем стартовую позицию по центру экрана
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, distanceFromCamera);
        Vector3 worldCenter = mainCamera.ViewportToWorldPoint(screenCenter);

        // Корректируем позицию, чтобы цепочка была вертикальной
        float totalHeight = (selectableObjects.Count - 1) * completionSpacing;
        Vector3 startPosition = worldCenter + Vector3.up * totalHeight * 0.5f;

        // Запускаем анимацию для каждого объекта
        for (int i = 0; i < selectableObjects.Count; i++)
        {
            GameObject obj = selectableObjects[i];
            Vector3 targetPosition = startPosition + Vector3.down * i * completionSpacing;

            LeanTween.move(obj, targetPosition, completionAnimationDuration)
                .setEase(LeanTweenType.easeOutBounce);

            // Поворачиваем объекты лицом к камере
            LeanTween.rotate(obj, mainCamera.transform.eulerAngles, completionAnimationDuration);

            LeanTween.scale(obj, Vector3.one * 1.2f, completionAnimationDuration * 0.5f)
                .setLoopPingPong(1);
        }
    }

    private void ResetSelection()
    {
        currentExpectedIndex = 0;

        foreach (var obj in selectableObjects)
        {
            if (obj != null)
            {
                if (originalColors.TryGetValue(obj, out Color originalColor))
                {
                    var renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = originalColor;
                    }
                }

                if (!isCompleted && originalPositions.TryGetValue(obj, out Vector3 originalPos))
                {
                    obj.transform.position = originalPos;
                    // Также можно сбросить вращение, если нужно
                }
            }
        }
    }

    private void ShakeCamera()
    {
        if (mainCamera != null && !isShaking)
        {
            isShaking = true;
            shakeTimeRemaining = cameraShakeDuration;
        }
    }

    private void Update()
    {
        if (isShaking)
        {
            if (shakeTimeRemaining > 0)
            {
                mainCamera.transform.position = originalCameraPosition + Random.insideUnitSphere * cameraShakeIntensity;
                shakeTimeRemaining -= Time.deltaTime;
            }
            else
            {
                isShaking = false;
                mainCamera.transform.position = originalCameraPosition;
            }
        }
    }
}