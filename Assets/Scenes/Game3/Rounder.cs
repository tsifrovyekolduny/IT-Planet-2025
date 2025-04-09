using UnityEngine;
using UnityEngine.UI;

public class RotateImage : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f; // Скорость вращения в градусах/кадр
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime * 60f);
    }
}