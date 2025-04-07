// Departments/Direction.asset
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Direction", menuName = "Game/Direction")]
public class Direction : ScriptableObject
{
    public string Name;
    public Texture2D Icon;
    public List<LevelsMenu.LevelScript> Levels; // Уровни направления
}