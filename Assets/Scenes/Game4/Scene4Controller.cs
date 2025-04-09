using UnityEngine;
using Assets.Scripts.Interfaces.Controllers;
using System;
public class Scene4Controller : MonoBehaviour, IMiniGame
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera MainCamera = null;
    private bool _isFinished = false;
    void Start()
    {
        if (MainCamera == null)
        {
            MainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnReturnZoonClick()
    {
        Debug.Log("One OnReturnZoonClick");
        IZoomeController zoomeController = MainCamera.GetComponent<IZoomeController>();
        if (zoomeController == null) return;
        zoomeController.ReturnZoom();
    }

    public void OnErrorClick()
    {
        Debug.Log("One Error click");
        IZoomeController zoomeController = MainCamera.GetComponent<IZoomeController>();
        if (zoomeController == null) return;
        zoomeController.ReturnZoom();
    }

    public void OnSucceedClick()
    {
        _isFinished = true;
    }

    public bool CheckForComplete()
    {
        return _isFinished;
    }
}
