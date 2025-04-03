using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways]
public class TMPWithImageController : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI _tmpText;
    [TextArea(3, 5)]
    [SerializeField] private string _textContent = "TMP Text";
    [SerializeField] private Color _textColor = Color.white;
    [SerializeField] private float _textSize = 14;
    [SerializeField] private TMP_FontAsset _fontAsset;
    [SerializeField] private bool _autoSize = true;

    [Header("Image Settings")]
    [SerializeField] protected Image _image;
    [SerializeField] private Sprite _imageSprite;
    [SerializeField] private Vector2 _imageSize = new Vector2(100, 100);
    [SerializeField] private bool _preserveAspect = true;

    [Header("Fallback Background")]
    [SerializeField] private bool _useFallbackColor = true;
    [SerializeField] private Color _fallbackColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    [SerializeField] private Image _backgroundImage;

    private void Start()
    {
        UpdateVisuals();
    }

    private void OnValidate()
    {
        UpdateVisuals();
    }

    public virtual void UpdateVisuals()
    {
        // Настройка текста
        ConfigureText();

        // Настройка изображения или фона
        if (_image != null)
        {
            ConfigureImage();
            _image.gameObject.SetActive(true);
            SetBackgroundVisibility(false);
        }
        else
        {
            SetBackgroundVisibility(true);
            if (_image != null) _image.gameObject.SetActive(false);
        }
    }

    private void ConfigureText()
    {
        _tmpText.text = _textContent;
        _tmpText.color = _textColor;
        _tmpText.fontSize = _textSize;

        if (_fontAsset != null)
            _tmpText.font = _fontAsset;

        if (_autoSize)
        {
            _tmpText.enableAutoSizing = true;
            _tmpText.fontSizeMin = 8;
            _tmpText.fontSizeMax = _textSize;
        }
    }

    public void SetText(string text)
    {
        _tmpText.text = text;
    }

    private void ConfigureImage()
    {
        if (_imageSprite != null)
        {
            _image.sprite = _imageSprite;
            _image.preserveAspect = _preserveAspect;

            _image.rectTransform.sizeDelta = _preserveAspect ?
                new Vector2(_imageSize.x, _imageSize.x * _imageSprite.rect.height / _imageSprite.rect.width) :
                _imageSize;
        }
    }

    private void SetBackgroundVisibility(bool visible)
    {
        if (_backgroundImage == null || !_useFallbackColor)
            return;

        _backgroundImage.color = _fallbackColor;
        _backgroundImage.enabled = visible;

        // Растягиваем на всю область
        if (visible)
        {
            _backgroundImage.rectTransform.anchorMin = Vector2.zero;
            _backgroundImage.rectTransform.anchorMax = Vector2.one;
            _backgroundImage.rectTransform.offsetMin = Vector2.zero;
            _backgroundImage.rectTransform.offsetMax = Vector2.zero;
        }
    }

    // API для внешнего управления
    public void SetFallbackColor(Color color, bool enabled = true)
    {
        _fallbackColor = color;
        _useFallbackColor = enabled;
        UpdateVisuals();
    }
}