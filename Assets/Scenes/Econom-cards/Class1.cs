using Assets.Scenes.Econom_cards.Interfaces;
using Assets.Scenes.Econom_cards.Interfaces.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scenes.Econom_cards
{
    interface IListObjectMover
    {
        void MoveNext();
        UnityEvent AllObjectsMoved { get; set; }
    }

    internal class ListObjectMover : MonoBehaviour, IListObjectMover
    {
        private IListGameObjects _listObjects;

        int _currentIndex = 0;

        private UnityEvent _allObjectsMoved = new UnityEvent();

        public UnityEvent AllObjectsMoved
        {
            get
            {
                return _allObjectsMoved;
            }
            set
            {
                _allObjectsMoved = value;
            }
        }

        void start()
        {
            _listObjects = GetComponent<IListGameObjects>();
            HideAllGameObjects();
        }

        private void HideAllGameObjects()
        {
            foreach(var obj in _listObjects.GameObjectsList)
            {
                obj.GetComponent<Renderer>().enabled = false;
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
            obj.GetComponent<Renderer>().enabled = true; // Включаем рендеринг

            // Запускаем корутину для ожидания 2 секунды и перемещения объекта
            StartCoroutine(MoveAndAnimate(obj, originalPosition));
        }

        private IEnumerator MoveAndAnimate(GameObject obj, Vector3 originalPosition)
        {
            // Ждем 2 секунды
            yield return new WaitForSeconds(2f);

            // Отправляем анимацию перемещения объекта на сохранённое
            float duration = 1f; // Длительность анимации
            float elapsedTime = 0f;

            Vector3 startPosition = obj.transform.position;

            while (elapsedTime < duration)
            {
                // Линейная интерполяция для перемещения объекта
                obj.transform.position = Vector3.Lerp(startPosition, originalPosition, (elapsedTime / duration));
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
