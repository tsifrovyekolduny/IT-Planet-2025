using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.Scenes.Econom_cards.Interfaces.Events
{
    internal interface IBrainEvent
    {
        UnityEvent BrainEvent { get; set; }
    }
}
