using Assets.Scripts.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.Scripts.Interfaces.Controllers
{
    public interface ISwipeController
    {
        public UnityEvent<SwipeOrientation> OnSwipe { get; set; }
        public float MaxVerticalMovement { get; }
        public float SwipeSmoothness { get; }
    }
}
