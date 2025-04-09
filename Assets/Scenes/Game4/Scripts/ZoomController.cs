using Assets.Scripts.Interfaces.Effects;
using Assets.Scripts.Interfaces.Handlers;
using Unity.VisualScripting;
using UnityEngine;
// using UnityEditor.UIElements;

namespace Assets.Scenes.Game4.Scripts
{
    internal class ZoomController : Assets.Scripts.Core.Controllers.ZoomController
    {
        private IInputHandler inputHandler;
        private HidableFormVariant newObjectToSelect = null;
        private GameObject newGameObjectToSelect = null;

        protected override void Start()
        {
            base.Start();
            inputHandler = GetComponent<IInputHandler>();
            inputHandler.OnObjectSelected.AddListener(OnObjectSelectedHandler);
            inputHandler.OnSelectionEmpty.AddListener(OnSelectionEmptyHandler);
            OnZoomStart.AddListener(OnZoomStartHandler);
            OnReleaseEnd.AddListener(OnZoomReturnEndHandler);
        }

        protected override void Update()
        {
            base.Update();
        }

        void OnSelectionEmptyHandler(Vector3 pos)
        {

            ReturnZoom();
        }

        void OnObjectSelectedHandler(Vector3 vector3, GameObject obj)
        {
            // Блокируем повтороное нажатие фокуса
            if (IsFocused)
            {
                HidableFormVariant nextObjectToSelect = lastZoomedObject.GetComponentInParent<HidableFormVariant>();

                if (nextObjectToSelect != null)
                {
                    newObjectToSelect = nextObjectToSelect;
                    newGameObjectToSelect = obj;

                    preZoomPosition = new Vector3((transform.position.x + obj.transform.position.x)/2, (transform.position.y + obj.transform.position.y)/2, returnStartPosition.z);
                    ReturnZoom();
                    return;
                }
            }
            lastZoomedObject = obj;

            HidableFormVariant variant = lastZoomedObject.GetComponentInParent<HidableFormVariant>();

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
            OnReleaseStart.AddListener(UnSubsrcibeHidableFormVariant);
        }

        void OnZoomeEndHidableFormVariantHandler()
        {
            var obj = lastZoomedObject.GetComponentInParent<HidableFormVariant>();
            obj.UpdateShowWithState(false);
        }

        void UnSubsrcibeHidableFormVariant()
        {
            var obj = lastZoomedObject.GetComponentInParent<HidableFormVariant>();
            obj.UpdateShowWithState(true);
            OnZoomEnd.RemoveListener(OnZoomeEndHidableFormVariantHandler);
            OnReleaseStart.RemoveListener(UnSubsrcibeHidableFormVariant);
        }

        void OnZoomStartHandler()
        {
            var mover = GetComponentInParent<ICameraMover>();
            if (mover != null)
            {
                mover.IsBlockingMoving = true;
            }
        }

        void OnZoomReturnEndHandler()
        {

            if (newObjectToSelect != null)
            {
                SubsribeHidableFormVariant(newObjectToSelect);
                StartZoom(newGameObjectToSelect.transform.position, newGameObjectToSelect);
                newObjectToSelect = null;
                newGameObjectToSelect = null;
                return;
            }

            var mover = GetComponent<ICameraMover>();

            if (mover != null)
            {
                mover.IsBlockingMoving = false;
            }
        }

        protected override void ReturnZoom()
        {
            if (IsFocused)
            {
                base.ReturnZoom();
            }
        }
    }
}