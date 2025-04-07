using Assets.Scripts.Core.Interfaces;
using Assets.Scripts.Core.Interfaces.Events;
using UnityEngine;

public class Extension : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    HideableUI ui;
    void Start()
    {
        var touchInputHandler = GetComponent<ITouchInputHandler>();
        ui = GetComponent<HideableUI>();
        touchInputHandler.OnSingleTap.AddListener(HandleSingleTap);
    }

    void Update()
    {
    }

    private void HandleSingleTap(Vector2 position)
    {
        //Debug.Log("Обработано одиночное нажатие в позиции: " + position);
        ui.ToggleVisibility();
    }
}
