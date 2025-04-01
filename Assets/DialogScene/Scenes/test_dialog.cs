using UnityEngine;

public class test_dialog : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            GameManager.Instance.CompleteSettedLevel();
            Debug.Log("game complete");
        }
    }
}
