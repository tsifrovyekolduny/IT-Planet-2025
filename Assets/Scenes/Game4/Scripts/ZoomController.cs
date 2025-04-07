using Assets.Scripts.Interfaces.Effects;
using Assets.Scripts.Interfaces.Handlers;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.UIElements.ToolbarMenu;

namespace Assets.Scenes.Game4.Scripts
{
    internal class ZoomController : Assets.Scripts.Core.Controllers.ZoomController
    {
        private IInputHandler inputHandler;
        private GameObject selectedGameObject;
        protected override void Start()
        {
            base.Start();
            inputHandler = GetComponent<IInputHandler>();
            inputHandler.OnObjectSelected.AddListener(OnObjectSelectedHandler);
            inputHandler.OnSelectionEmpty.AddListener(OnSelectionEmptyHandler);
            OnZoomStart.AddListener(OnZoomStartHandler);
            OnReleaseEnd.AddListener(OnZoomReturnEndHandler);
        }

        protected override void Update() { 
            base.Update();
        }

        void OnSelectionEmptyHandler(Vector3 pos)
        {
            if (IsFocused)
            {
                ReturnZoom();
            }
        }

        void OnObjectSelectedHandler(Vector3 vector3, GameObject obj)
        {
            // Блокируем повтороное нажатие фокуса
            if (IsFocused) return;
            lastZoomedObject = obj;

            HidableFormVariant variant = lastZoomedObject.GetComponent<HidableFormVariant>();

            if (variant != null)
            {
                // Делаем подписку
                SubsribeHidableFormVariant(variant);
            }
            StartZoom(obj.transform.position, obj);
        }

        void SubsribeHidableFormVariant(HidableFormVariant variant)
        {
            OnZoomEnd.AddListener(OnZoomeEndHidableFormVariantHandler);
            OnReleaseStart.RemoveListener(UnSubsrcibeHidableFormVariant);
        }

        void OnZoomeEndHidableFormVariantHandler()
        {
            var obj = lastZoomedObject.GetComponent<HidableFormVariant>();
            obj.UpdateShowWithState(false);
        }

        void UnSubsrcibeHidableFormVariant()
        {
            var obj = lastZoomedObject.GetComponent<HidableFormVariant>();
            obj.UpdateShowWithState(true);
            OnZoomEnd.RemoveListener(OnZoomeEndHidableFormVariantHandler);
            OnReleaseStart.RemoveListener(UnSubsrcibeHidableFormVariant);
        }

        void OnZoomStartHandler()
        {
            var mover = GetComponent<ICameraMover>();
            if (mover != null)
            {
                mover.IsBlockingMoving = true;
            }
        }

        void OnZoomReturnEndHandler()
        {
            var mover = GetComponent<ICameraMover>();
            if(mover != null)
            {
                mover.IsBlockingMoving = false;
            }
        }

        public void ReturnZoom()
        {
            base.ReturnZoom();
        }
    }
}