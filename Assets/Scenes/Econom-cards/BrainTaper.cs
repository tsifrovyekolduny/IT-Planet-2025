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
    [Rename("Количество тапов для ивента")]
    public int CountTapForEvent;

    [SerializeField]
    [Rename("Время ожидания в секундах нового тапа")]
    public float TimeThresholdNewTap;

    [SerializeField]
    [Rename("Время блокировки после ивента")]
    public float TimeThresholdBlockTapAfterEvent;


    bool _isBrainEventRaised = false;

    private ICounterClickEvent _counterClickEvent;

    private UnityEvent _brainEvent = new UnityEvent();
    public UnityEvent BrainEvent { get => _brainEvent; set => _brainEvent = value; }

    void Start()
    {
        var touchInputHandler = GetComponent<ITouchInputHandler>();
        _shakleEffect = GetComponent<IShakeable>();


        // Проверяем, реализует ли он интерфейс ITouchInputHandler
        if (touchInputHandler is ITouchInputHandler)
        {
            Debug.Log("TouchInputHandler реализует интерфейс ITouchInputHandler.");
        }
        else
        {
            Warning.Info("TouchInputHandler не реализует интерфейс ITouchInputHandler.");
            _isValid = false;
        }

        if (_shakleEffect is IShakeable)
        {
            Debug.Log("shakleEffect реализует интерфейс IShakeable.");
        }
        else
        {
            Warning.Info("shakleEffect не реализует интерфейс IShakeable.");
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

        Debug.Log("Обработано одиночное нажатие в позиции: " + position);
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
        yield return new WaitForSeconds(TimeThresholdBlockTapAfterEvent); // Ждем указанное время

        _isBrainEventRaised = false; // Сбрасываем состояние
        Debug.Log("Состояние восстановлено после блокировки.");
    }
}
