using Assets.Scenes.Econom_cards.Interfaces;
using Assets.Scenes.Econom_cards.Interfaces.Events;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BrainTaper : MonoBehaviour, IBrainEvent
{
    private bool _isValid = true;
    private IShakeable _shakleEffect = null;

    [SerializeField]
    [Rename("���������� ����� ��� ������")]
    public int CountTapForEvent;

    [SerializeField]
    [Rename("����� �������� � �������� ������ ����")]
    public float TimeThresholdNewTap;

    [SerializeField]
    [Rename("����� ���������� ����� ������")]
    public float TimeThresholdBlockTapAfterEvent;


    bool _isBrainEventRaised = false;

    private ICounterClickEvent _counterClickEvent;

    private UnityEvent _brainEvent = new UnityEvent();
    public UnityEvent BrainEvent { get => _brainEvent; set => _brainEvent = value; }

    void Start()
    {
        var touchInputHandler = GetComponent<ITouchInputHandler>();
        _shakleEffect = GetComponent<IShakeable>();


        // ���������, ��������� �� �� ��������� ITouchInputHandler
        if (touchInputHandler is ITouchInputHandler)
        {
            Debug.Log("TouchInputHandler ��������� ��������� ITouchInputHandler.");
        }
        else
        {
            Warning.Info("TouchInputHandler �� ��������� ��������� ITouchInputHandler.");
            _isValid = false;
        }

        if (_shakleEffect is IShakeable)
        {
            Debug.Log("shakleEffect ��������� ��������� IShakeable.");
        }
        else
        {
            Warning.Info("shakleEffect �� ��������� ��������� IShakeable.");
            _isValid = false;
        }
        if (_isValid == false) return;

        touchInputHandler.OnSingleTap.AddListener(HandleSingleTap);

        _counterClickEvent = new CounterClickEvent(CountTapForEvent, TimeThresholdNewTap);
        _counterClickEvent.OnCountReached.AddListener(RaiseEvent);
    }

    void Update()
    {
        if (_isValid == false) return;
    }

    private void HandleSingleTap(Vector2 position)
    {
        if (_isBrainEventRaised) return;

        Debug.Log("���������� ��������� ������� � �������: " + position);
        _shakleEffect.StartShake();
        _counterClickEvent.Click();
    }

    private void RaiseEvent()
    {
        if (_isBrainEventRaised) return;
        _isBrainEventRaised = true;
        _brainEvent.Invoke();
        StartCoroutine(ResetBrainEvent());

    }

    private IEnumerator ResetBrainEvent()
    {
        yield return new WaitForSeconds(TimeThresholdBlockTapAfterEvent); // ���� ��������� �����

        _isBrainEventRaised = false; // ���������� ���������
        Debug.Log("��������� ������������� ����� ����������.");
    }
}
