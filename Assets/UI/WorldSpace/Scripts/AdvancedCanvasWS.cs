using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

[ExecuteAlways]
public class AdvancedTMPElement : TMPWithImageController
{
    [Header("Layout")]
    [SerializeField] private RectTransform _contentRect;
    [SerializeField] private Vector2 _imageOffset = Vector2.zero;
    [SerializeField][Range(0.5f, 2f)] private float _imageScale = 1f;

    public override void UpdateVisuals()
    {
        base.UpdateVisuals();

        // Позиция и масштаб картинки
        if (_image != null)
        {
            _image.rectTransform.anchoredPosition = _imageOffset;
            _image.rectTransform.localScale = Vector3.one * _imageScale;
        }
    }

    public void SetImageSettings(Vector2 offset, float scale)
    {
        _imageOffset = offset;
        _imageScale = scale;
        UpdateVisuals();
    }
}