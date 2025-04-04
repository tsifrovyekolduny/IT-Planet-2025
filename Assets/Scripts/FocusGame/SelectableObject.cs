using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public SequenceSelectionGame gameManager;

    private void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.OnObjectSelected(gameObject);
        }
    }
}