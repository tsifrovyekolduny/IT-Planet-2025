using System;
using UnityEngine;
using UnityEngine.Events;

public class TestUI : MonoBehaviour
{
    public UnityEvent unityEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Key down");
            unityEvent.Invoke();            

            UIFloatingLabelSpawner.Instance.SpawnFloatingText(
                "+1",
                LabelAnchor.TopRight,
                new Vector2(50, 50), // Придется придрочиться
                10f);

            UIFloatingLabelSpawner.Instance.SpawnFloatingText(
                "+1",
                LabelAnchor.TopLeft,
                new Vector2(50, 50), // Придется придрочиться
                10f);

            UIFloatingLabelSpawner.Instance.SpawnFloatingText(
                "+1",
                LabelAnchor.BottomLeft,
                new Vector2(-50, -50), // Придется придрочиться
                10f);

            UIFloatingLabelSpawner.Instance.SpawnFloatingText(
                "+1",
                LabelAnchor.BottomRight,
                new Vector2(-50, -50), // Придется придрочиться
                10f);
        }
    }
}
