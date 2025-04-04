using UnityEngine;
using TMPro;
using System.Collections;

public class TextHider : HideableUI
{
    [Header("Настройки")]
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private string _hiddenPattern = "_____________";    

    private string _originalText;    

    protected override void Awake()
    {
        base.Awake();        

        // Инициализация текста
        _originalText = _targetText.text;
    }  

    void EncryptText()
    {
        if (_isHidden)
        {
            _targetText.text = _originalText;
        }
        else
        {
            _targetText.text = _hiddenPattern;
        }

        _isHidden = !_isHidden;
    }    
   
    public override void ApplyVisibility(bool show)
    {
        _targetText.alpha = show ? 1f : 0f;        
    }

    public override void ApplyVisibilityWithoutFade(bool show)
    {
        if (show)
        {
            _targetText.text = _originalText;
        }
        else
        {
            _targetText.text = _hiddenPattern;
        }
    }
}