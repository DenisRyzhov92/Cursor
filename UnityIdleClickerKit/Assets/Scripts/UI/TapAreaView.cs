using IdleClickerKit.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IdleClickerKit.UI
{
    public sealed class TapAreaView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private IdleClickerManager manager;

        [Tooltip("Additional taps per second while finger is held down.")]
        [SerializeField]
        [Min(0f)]
        private float holdTapRate = 0f;

        private bool isHolding = false;
        private float holdTimer = 0f;

        private void Update()
        {
            if (!isHolding || manager == null || holdTapRate <= 0f)
            {
                return;
            }

            var interval = 1f / holdTapRate;
            holdTimer += Time.unscaledDeltaTime;

            while (holdTimer >= interval)
            {
                holdTimer -= interval;
                manager.Tap();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (manager == null)
            {
                return;
            }

            isHolding = true;
            holdTimer = 0f;
            manager.Tap();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
            holdTimer = 0f;
        }
    }
}
