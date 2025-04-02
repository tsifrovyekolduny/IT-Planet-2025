using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DirectionHubUI : CommonHubUI
{
    private List<Direction> _directions = new List<Direction>();
    public VisualTreeAsset HubElementUI;

    private void Start()
    {
        _directions = GameManager.Instance.CurrentDepartment.Directions;
        base.Start();
    }
    

    public static void GoOnDirection(Direction direction)
    {
        // Загружаем UI и отображаем направления
        GameManager.Instance.CurrentDirection = direction;
        SceneManager.LoadScene("LevelsScene");
    }    

    protected override void MakeLinkButton(Button link, object arg)
    {
        Direction direction = arg as Direction;
        link.clicked += () => GoOnDirection(direction);
    }

    public override void InitHubElements()
    {
        foreach (var direction in _directions) {
            InitHubElement(direction.name, direction.Icon, direction);
        }
    }

    public override void InitBackButtonClick()
    {
        hubContainerRoot.Q<Button>("back-btn").clicked += () => SceneManager.LoadScene("DepartmentsHub");
        throw new System.NotImplementedException();
    }
}
