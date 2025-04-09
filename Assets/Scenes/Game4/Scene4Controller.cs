using UnityEngine;
using Assets.Scripts.Interfaces.Controllers;
using System;
public class Scene4Controller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera MainCamera = null;
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

    public void OnErrorClick()
    {
        Debug.Log("One Error click");
        IZoomeController zoomeController = MainCamera.GetComponent<IZoomeController>();
        if (zoomeController == null) return;
        zoomeController.ReturnZoom();
    }

    public void OnSuccsedClick()
    {

    }
}
