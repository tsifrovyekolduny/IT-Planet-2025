using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;

namespace Assets.Scenes.Econom_cards.Interfaces.Events
{
    interface ICounterClickEvent
    {
        void Click();
        public UnityEvent OnCountReached { get; set; }
    }
}
