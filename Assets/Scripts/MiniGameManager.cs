using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MiniGameManager : MonoBehaviour
{
    public UIDocument GameMenu;
    public UIDocument HintPanel;
    [SerializeField, SerializeReference]    
    public float HintLifeTime = 10f;
    public List<string> Hints;

    private IEnumerator<string> _hintsEnum;
    private Button _completeButton;
    private IMiniGame _miniGame;
    private Label _hintText;
    private Coroutine _hintCoroutine;
    private bool _isComplete = false;
    
    void Start()
    {
        var ss = FindObjectsOfType<MonoBehaviour>().OfType<IMiniGame>();
        foreach (IMiniGame s in ss)
        {
            _miniGame = s;
        }        
        if(_miniGame == null)
        {
            Debug.LogError("������ IMiniGame ���������� �������� ���������");
        }
        _hintsEnum = Hints.GetEnumerator();
        InitializeUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (_miniGame.CheckForComplete() && !_isComplete)
        {
            _isComplete = true;
            _completeButton.SetEnabled(true);
        }
    }

    void InitializeUI()
    {
        var root = GameMenu.rootVisualElement;
        _completeButton = root.Q<Button>("complete-btn");
        var backButton = root.Q<Button>("back-to-chat-btn");
        var hintButton = root.Q<Button>("hint-btn");


        _completeButton.SetEnabled(false);
        hintButton.clicked += () => ShowHint();
        _hintText = HintPanel.rootVisualElement.Q<Label>("hint-text");        
        _completeButton.clicked += () => GameManager.Instance.CompleteSettedLevel();
        backButton.clicked += () => GameManager.Instance.SetLoadedLevel();

        HintPanel.rootVisualElement.visible = false;
    }    

    public void ShowHint()
    {
        _hintsEnum.MoveNext();
        string hint = _hintsEnum.Current;
        if(hint == Hints.Last())
        {
            _hintsEnum.Reset();
        }

        _hintText.text = hint;
        HintPanel.rootVisualElement.visible = true;

        if (_hintCoroutine != null)
            StopCoroutine(_hintCoroutine);

        _hintCoroutine = StartCoroutine(HideHintAfterDelay(HintLifeTime));
    }

    private IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HintPanel.rootVisualElement.visible = false;
        _hintCoroutine = null;
    }
       
}
