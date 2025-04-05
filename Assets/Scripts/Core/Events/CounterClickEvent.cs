using Assets.Scripts.Core.Interfaces.Events;
using UnityEngine;
using UnityEngine.Events;

public class CounterClickEvent : ICounterClickEvent
{
    private int _countCurrent = 0; // ������� ������� ������
    private int _countForEvent; // ���������� ������ ��� ������������ �������
    private float _lastTimeClick; // ����� ���������� �����
    private float _timeThreshold; // ����� ������� ����� �������

    private UnityEvent _onCountReached; // �������, ������� ����� ������� ��� ���������� ���������� ������

    public UnityEvent OnCountReached { get => _onCountReached; set => _onCountReached = value; }

    // �����������
    public CounterClickEvent(int countForEvent, float timeThreshold)
    {
        _countForEvent = countForEvent;
        _timeThreshold = timeThreshold;
        OnCountReached = new UnityEvent();
    }


    // ����� ��� ��������� �����
    public void Click()
    {
        float currentTime = Time.time; // �������� ������� �����

        // ���������, ������ �� ���������� ������� � ���������� �����
        if (currentTime - _lastTimeClick <= _timeThreshold)
        {
            _countCurrent++; // ����������� ������� ������

            // ���������, �������� �� �� ������������ ���������� ������
            if (_countCurrent >= _countForEvent)
            {
                OnCountReached.Invoke(); // �������� �������
                _countCurrent = 0; // ���������� �������
            }
        }
        else
        {
            // ���� ����� ����� ������� ������ ������, ���������� �������
            _countCurrent = 1; // �������� ����� �������
        }

        _lastTimeClick = currentTime; // ��������� ����� ���������� �����
    }
}