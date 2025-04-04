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

            FloatingTextSpawner.Instance.ToString();
            FloatingTextSpawner.Instance.SpawnScreen("+1", Vector2.zero, TextAnchor.UpperRight);
            FloatingTextSpawner.Instance.SpawnScreen("+1", Vector2.zero, TextAnchor.UpperLeft);
            FloatingTextSpawner.Instance.SpawnScreen("+1", Vector2.zero, TextAnchor.MiddleCenter);
            FloatingTextSpawner.Instance.SpawnScreen("+1", Vector2.zero, TextAnchor.LowerLeft);
            FloatingTextSpawner.Instance.SpawnScreen("+1", Vector2.zero, TextAnchor.LowerRight);
        }
    }
}
