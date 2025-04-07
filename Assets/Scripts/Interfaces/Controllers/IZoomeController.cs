using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces.Controllers
{
    internal interface IZoomeController
    {
        void StartZoom(Vector3 worldPosition, GameObject targetObject);
        void ReturnZoom();
        bool IsFocused { get; }
    }
}
