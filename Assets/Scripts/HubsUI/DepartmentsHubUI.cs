using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DepartmentsHubUI : CommonHubUI
{
    [SerializeField] private UIDocument _uiDoc;
    [SerializeField] private List<Department> _departments;

    void Start()
    {
        base.Start();
    }

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
}