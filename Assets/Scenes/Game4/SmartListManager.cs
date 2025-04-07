using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class SmartListManager : MonoBehaviour
{
    public GameObject itemPrefab; // ������ �������� ������
    public Transform content; // ��������� ��� ��������� ������
    private List<GameObject> items = new List<GameObject>(); // ������ ���������

    void Start()
    {
        // ������ ���������� ���������
        AddItem("������� 1");
        AddItem("������� 2");
        AddItem("������� 3");
    }

    public void AddItem(string itemName)
    {
        GameObject newItem = Instantiate(itemPrefab, content);
        Debug.Log(newItem.GetType().Name);  
        newItem.GetComponentInChildren<TMP_Text>().text = itemName;

        // ��������� ����������� ������� ��� ��������������
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
            // ����������� ��������
            Vector3 mousePosition = Input.mousePosition;

            // �������� �� ��������� �������
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != draggedItem && RectTransformUtility.RectangleContainsScreenPoint(items[i].GetComponent<RectTransform>(), mousePosition))
                {
                    draggedItem.transform.position = mousePosition;

                    // ��������� ������ ��������, ������� �� ����������
                    int originalIndex = items.IndexOf(draggedItem); // �������� ������������ ������

                    // ���� ����� ������ ������ �������������, ���������� ������� �����
                    if (i < originalIndex)
                    {
                        // ����������� ������� �����
                        items.Insert(i, draggedItem); // ��������� ������� �� ����� �����
                        items.RemoveAt(originalIndex + 1); // ������� ������� �� ������� �����
                    }
                    else
                    {
                        // ����������� ������� ����
                        items.Insert(i + 1, draggedItem); // ��������� ������� �� ����� �����
                        items.RemoveAt(originalIndex); // ������� ������� �� ������� �����
                    }

                    // ��������� ������
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
        //    Destroy(child.gameObject); // ������� ������ ��������
        //}

        foreach (var item in items)
        {
            item.transform.SetParent(content);
            item.transform.SetAsLastSibling(); // ������������� �������
        }

        LayoutGroup layoutGroup = content.GetComponent<LayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.SetLayoutVertical(); // ��������� ������������ ������������
            layoutGroup.SetLayoutHorizontal(); // ��������� �������������� ������������
        }

        //// ��������� ������������ ���������
        //VerticalLayoutGroup layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        //if (layoutGroup != null)
        //{
        //    layoutGroup.SetLayoutVertical(); // ��������� ������������ ������������
        //}

        //// ���� � ��� ���� ContentSizeFitter, �� ������ ������� ��� ����������
        //ContentSizeFitter sizeFitter = content.GetComponent<ContentSizeFitter>();
        //if (sizeFitter != null)
        //{
        //    sizeFitter.SetLayoutVertical(); // ��������� ������ ��������
        //}
    }
}