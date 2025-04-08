using UnityEngine;

public class ChoiceObjectClick : MonoBehaviour
{
    [HideInInspector] public BinaryChoiceSystem parentSystem;
    [HideInInspector] public bool isCorrectAnswer;

    private void OnMouseDown()
    {
        if (parentSystem != null)
        {
            parentSystem.HandleChoice(gameObject, isCorrectAnswer);
        }
    }
}