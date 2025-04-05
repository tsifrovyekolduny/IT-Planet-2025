using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.Scripts.Core.Interfaces.Effects
{
    public interface IListObjectMover
    {
        void MoveNext();
        UnityEvent AllObjectsMovedEvent { get; set; }
    }
}
