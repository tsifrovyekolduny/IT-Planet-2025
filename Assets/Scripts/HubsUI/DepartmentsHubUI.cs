using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DepartmentsHubUI : CommonHubUI
{
    [SerializeField] private List<Department> _departments;

    void GoOnDepartment(Department department)
    {
        GameManager.Instance.CurrentDepartment = department;
        // Переходим к хабу направлений
        SceneManager.LoadScene("DirectionsHub");
    }

    public override void InitHubElements()
    {
        foreach(var department in _departments)
        {
            InitHubElement(department.Name, department.Icon, department);
        }
    }

    protected override void MakeLinkButton(Button link, object arg)
    {
        Department department = (Department) arg;

        GameManager.Instance.CurrentDepartment = department;
        link.clicked += () => SceneManager.LoadScene("DirectionsHub");
    }

    public override void InitBackButtonClick()
    {
        return;
    }

    public override void InitHubName()
    {
        hubContainerRoot.Q<Label>("hub-name").text = "Институт Цифры";
    }
}