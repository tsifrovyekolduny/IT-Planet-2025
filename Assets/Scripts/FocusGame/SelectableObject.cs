using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public SequenceSelectionGame gameManager;

    private void Start()
    {
        var touchInputHandler = gameObject.GetComponent<TouchInputHandler>() ?? gameObject.AddComponent<TouchInputHandler>();
        touchInputHandler.OnSingleTap.AddListener(CheckTouch);
    }

    //private void Update()
    //{
    //    // Обработка касаний на мобильных устройствах
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);

    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            CheckTouch(touch.position);
    //        }
    //    }
    //}

    //private void OnMouseDown()
    //{
    //    // Обработка кликов мышью (для редактора и ПК)
    //    if (gameManager != null)
    //    {
    //        gameManager.OnObjectSelected(gameObject);
    //    }
    //}

    private void CheckTouch(Vector2 touchPosition)
    {
        // Преобразуем позицию касания в мировые координаты
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touchPosition);

        // Проверяем, было ли касание по этому объекту
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (gameManager != null)
            {
                gameManager.OnObjectSelected(gameObject);
            }
        }
    }
}