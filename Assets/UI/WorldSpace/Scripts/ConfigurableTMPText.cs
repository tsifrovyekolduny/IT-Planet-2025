using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
[ExecuteAlways]
public class ConfigurableTMPText
{
    [Header("Text Settings")]
    [SerializeField] protected TextMeshProUGUI _tmpText;
    [TextArea(3, 5)]
    [SerializeField] protected string _textContent = "TMP Text";
    [SerializeField] protected Color _textColor = Color.white;
    [SerializeField] protected float _textSize = 14;
    [SerializeField] protected TMP_FontAsset _fontAsset;
    [SerializeField] protected bool _autoSize = true;    
    
    public void ConfigureText()
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
}
