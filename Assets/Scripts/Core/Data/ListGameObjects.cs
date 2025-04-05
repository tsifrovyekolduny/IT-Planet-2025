using Assets.Scripts.Core.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scenes.Econom_cards.Data
{
    public class ListGameObjects :  MonoBehaviour ,IListGameObjects
    {
        [Header("Список объектов")]
        [Rename("Список")]
        [Tooltip("Список")]
        public List<GameObject> InputGameObjectsList;
        public List<GameObject> GameObjectsList { get => InputGameObjectsList; set => InputGameObjectsList = value; }
    }
}
