using Assets.Scripts.Core.Interfaces.Effects;
using Assets.Scripts.Core.Interfaces.Models;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scenes.Econom_cards
{
    internal class ListObjectMover : MonoBehaviour, IListObjectMover
    {
        [SerializeField] private float waitTime = 2f; // Время ожидания перед перемещением
        [SerializeField] private float animationDuration = 1f; // Длительность анимации

        private IListGameObjects _listObjects;
        private int _currentIndex = 0;
        private UnityEvent _allObjectsMoved = new UnityEvent();
        private bool isValid = true;

        public UnityEvent AllObjectsMovedEvent
        {
            get => _allObjectsMoved;
            set => _allObjectsMoved = value;
        }

        void Start()
        {
            _listObjects = GetComponent<IListGameObjects>();

            if (_listObjects is IListGameObjects)
            {
                Debug.Log("_listObjects реализует интерфейс IListGameObjects.");
            }
            else
            {
                Debug.LogWarning("_listObjects не реализует интерфейс IListGameObjects.");
                isValid = false;
            }

            HideAllGameObjects();
        }

        private void HideAllGameObjects()
        {
            foreach (var obj in _listObjects.GameObjectsList)
            {
                obj.GetComponent<Canvas>().enabled = false;
            }
        }

        private void MoveObject(GameObject obj)
        {
            // Запоминаем все параметры transform
            Vector3 originalPosition = obj.transform.position;
            Quaternion originalRotation = obj.transform.rotation;
            Vector3 originalScale = obj.transform.localScale;

            // Получаем текущую позицию
            Vector3 targetPosition = transform.position;

            // Устанавливаем объект поверх текущего, даём пользователю возможность прочитать (включаем рендеринг)
            obj.transform.position = targetPosition;
            obj.SetActive(true); // Убедитесь, что объект активен
            obj.GetComponent<Canvas>().enabled = true;

            // Запускаем корутину для ожидания и перемещения объекта
            StartCoroutine(MoveAndAnimate(obj, originalPosition));
        }

        private IEnumerator MoveAndAnimate(GameObject obj, Vector3 originalPosition)
        {
            // Ждем заданное время
            yield return new WaitForSeconds(waitTime);

            // Отправляем анимацию перемещения объекта на сохранённое
            float elapsedTime = 0f;
            Vector3 startPosition = obj.transform.position;

            while (elapsedTime < animationDuration)
            {
                // Линейная интерполяция для перемещения объекта
                obj.transform.position = Vector3.Lerp(startPosition, originalPosition, (elapsedTime / animationDuration));
                elapsedTime += Time.deltaTime;
                yield return null; // Ждем следующего кадра
            }

            // Убедитесь, что объект точно на конечной позиции
            obj.transform.position = originalPosition;
        }

        void IListObjectMover.MoveNext()
        {
            if (_currentIndex < _listObjects.GameObjectsList.Count)
            {
                MoveObject(_listObjects.GameObjectsList[_currentIndex]);
                ++_currentIndex;
            }
            else
            {
                _allObjectsMoved.Invoke();
            }
        }
    }
}