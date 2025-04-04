using UnityEngine.Events;
using UnityEngine;

namespace Assets.Scenes.Econom_cards.Interfaces
{
    public interface ITouchInputHandler
    {
        public UnityEvent<Vector2> OnSingleTap { get; set; }
        public UnityEvent OnDoubleTap { get; set; }
        public UnityEvent<Vector2> OnSwipe { get; set; }
        public UnityEvent<Vector2> OnTouchEnd { get; set; }
    }
}
