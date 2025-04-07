using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Interfaces.Handlers
{
    internal interface IInputHandler
    {
        public UnityEvent<Vector3, GameObject> OnObjectSelected { get; }
        public UnityEvent<Vector3> OnSelectionEmpty { get; }
    }
}
