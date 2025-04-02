using UnityEngine;

public class test_dialog : MonoBehaviour, IMiniGame
{
    private bool _isClicked = false;
    public bool CheckForComplete()
    {
        return _isClicked;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            _isClicked = true;
            Debug.Log("game complete");
        }
    }
}
