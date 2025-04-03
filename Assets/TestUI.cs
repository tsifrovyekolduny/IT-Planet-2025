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
        }
    }
}
