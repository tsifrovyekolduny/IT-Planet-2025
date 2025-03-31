using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        ProgressManager.Instance.UpdateDirectionProgress("PI", 0, "");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
