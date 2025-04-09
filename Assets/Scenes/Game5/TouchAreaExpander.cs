using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TouchAreaExpander : MonoBehaviour
{
    [SerializeField] private RectTransform expandTarget;
    [SerializeField] private Vector2 expandSize = new Vector2(30, 30);

    private void Awake()
    {
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null && expandTarget != null)
        {
            collider.size = expandTarget.rect.size + expandSize;
        }
    }
}