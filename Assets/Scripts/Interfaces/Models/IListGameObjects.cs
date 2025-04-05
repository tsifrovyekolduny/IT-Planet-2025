using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core.Interfaces.Models
{
    public interface IListGameObjects
    {
        public List<GameObject> GameObjectsList { get; set; }
    }
}
