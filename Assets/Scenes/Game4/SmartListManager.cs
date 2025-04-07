using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class SmartListManager : MonoBehaviour
{
    public GameObject itemPrefab; // Префаб элемента списка
    public Transform content; // Контейнер для элементов списка
    private List<GameObject> items = new List<GameObject>(); // Список элементов

    void Start()
    {
        // Пример добавления элементов
        AddItem("Элемент 1");
        AddItem("Элемент 2");
        AddItem("Элемент 3");
    }

    public void AddItem(string itemName)
    {
        GameObject newItem = Instantiate(itemPrefab, content);
        Debug.Log(newItem.GetType().Name);  
        newItem.GetComponentInChildren<TMP_Text>().text = itemName;

        // Добавляем обработчики событий для перетаскивания
        EventTrigger trigger = newItem.GetComponent<EventTrigger>();
        EventTrigger.Entry entryBeginDrag = new EventTrigger.Entry();
        entryBeginDrag.eventID = EventTriggerType.BeginDrag;
        entryBeginDrag.callback.AddListener((data) => { OnBeginDrag(newItem); });
        trigger.triggers.Add(entryBeginDrag);

        EventTrigger.Entry entryEndDrag = new EventTrigger.Entry();
        entryEndDrag.eventID = EventTriggerType.EndDrag;
        entryEndDrag.callback.AddListener((data) => { OnEndDrag(newItem); });
        trigger.triggers.Add(entryEndDrag);

        items.Add(newItem); 
    }

    private GameObject draggedItem;
    private int originalIndex;

    public void OnBeginDrag(GameObject item)
    {
        draggedItem = item;
        originalIndex = items.IndexOf(item);
    }

    public void OnEndDrag(GameObject item)
    {
        draggedItem = null;
    }

    public void Update()
    {
        if (draggedItem != null)
        {
            // Перемещение элемента
            Vector3 mousePosition = Input.mousePosition;

            // Проверка на изменение порядка
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != draggedItem && RectTransformUtility.RectangleContainsScreenPoint(items[i].GetComponent<RectTransform>(), mousePosition))
                {
                    draggedItem.transform.position = mousePosition;

                    // Сохраняем индекс элемента, который мы перемещаем
                    int originalIndex = items.IndexOf(draggedItem); // Получаем оригинальный индекс

                    // Если новый индекс меньше оригинального, перемещаем элемент вверх
                    if (i < originalIndex)
                    {
                        // Переместить элемент вверх
                        items.Insert(i, draggedItem); // Вставляем элемент на новое место
                        items.RemoveAt(originalIndex + 1); // Удаляем элемент из старого места
                    }
                    else
                    {
                        // Переместить элемент вниз
                        items.Insert(i + 1, draggedItem); // Вставляем элемент на новое место
                        items.RemoveAt(originalIndex); // Удаляем элемент из старого места
                    }

                    // Обновляем список
                    PopulateList();
                    break;
                }
            }
        }
    }

    private void PopulateList()
    {
        //foreach (Transform child in content)
        //{
        //    Destroy(child.gameObject); // Удаляем старые элементы
        //}

        foreach (var item in items)
        {
            item.transform.SetParent(content);
            item.transform.SetAsLastSibling(); // Устанавливаем порядок
        }

        LayoutGroup layoutGroup = content.GetComponent<LayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.SetLayoutVertical(); // Обновляем вертикальное расположение
            layoutGroup.SetLayoutHorizontal(); // Обновляем горизонтальное расположение
        }

        //// Обновляем расположение элементов
        //VerticalLayoutGroup layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        //if (layoutGroup != null)
        //{
        //    layoutGroup.SetLayoutVertical(); // Обновляем вертикальное расположение
        //}

        //// Если у вас есть ContentSizeFitter, вы можете вызвать его обновление
        //ContentSizeFitter sizeFitter = content.GetComponent<ContentSizeFitter>();
        //if (sizeFitter != null)
        //{
        //    sizeFitter.SetLayoutVertical(); // Обновляем размер контента
        //}
    }
}