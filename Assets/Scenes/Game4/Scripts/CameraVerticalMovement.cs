using Assets.Scripts.Core.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scenes.Game4.Scripts
{
    internal class CameraVerticalMovement : Assets.Scripts.Core.Effects.CameraVerticalMovement
    {
        public float swipeCooldown = 0.5f; // Задержка после свайпа
        private bool isCooldownActive = false; // Флаг для отслеживания состояния cooldown
        
        protected override void HandleSwipe(SwipeOrientation orientation)
        {
            if (isCooldownActive) return; // Если cooldown активен, игнорируем свайп

            base.HandleSwipe(orientation); // Вызов родительского метода для обработки свайпа

            // Запускаем корутину для cooldown
            StartCoroutine(SwipeCooldown());
        }

        private IEnumerator SwipeCooldown()
        {
            isCooldownActive = true; // Устанавливаем флаг cooldown
            yield return new WaitForSeconds(swipeCooldown); // Ждем заданное время
            isCooldownActive = false; // Сбрасываем флаг cooldown
        }
    }
}
