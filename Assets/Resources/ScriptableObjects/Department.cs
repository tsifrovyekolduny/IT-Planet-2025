// Departments/Department.asset
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Department", menuName = "Game/Department")]
public class Department : ScriptableObject
{
    public string Name;
    public Texture2D Icon;
    public List<Direction> Directions; // Направления кафедры
}