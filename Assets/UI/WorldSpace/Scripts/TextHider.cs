using UnityEngine;
using TMPro;

public class TextHider : HideableUI
{
    [Header("Настройки")]
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private string _hiddenPattern = "_____________";    

    private string _originalText;    

    protected override void Start()
    {        
        // Инициализация текста
        _originalText = _targetText.text;

        base.Start();        
    }  
   
    public override void ApplyVisibility(bool show)
    {
        _hidingCanvasGroup.alpha = show ? 1f : 0f;        
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