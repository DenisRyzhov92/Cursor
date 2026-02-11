using System.Collections.Generic;
using IdleClickerKit.Core;
using IdleClickerKit.Monetization;
using UnityEngine;

namespace IdleClickerKit.UI
{
    public sealed class RealMoneyProductListView : MonoBehaviour
    {
        [SerializeField]
        private IdleClickerManager manager;

        [SerializeField]
        private RealMoneyStoreController storeController;

        [SerializeField]
        private RealMoneyProductButtonView productButtonPrefab;

        [SerializeField]
        private Transform contentRoot;

        [SerializeField]
        private bool rebuildOnStart = true;

        private readonly List<RealMoneyProductButtonView> spawnedButtons = new List<RealMoneyProductButtonView>();

        private void Start()
        {
            if (rebuildOnStart)
            {
                Build();
            }
        }

        [ContextMenu("Build Real Money Product Buttons")]
        public void Build()
        {
            if (manager == null || storeController == null || productButtonPrefab == null || contentRoot == null)
            {
                Debug.LogWarning("[IdleClicker] RealMoneyProductListView is not configured.");
                return;
            }

            if (!manager.IsReady)
            {
                manager.Initialize();
            }

            ClearSpawnedButtons();
            var definitions = manager.GetRealMoneyProductDefinitions();
            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                if (definition == null || string.IsNullOrWhiteSpace(definition.productId))
                {
                    continue;
                }

                var instance = Instantiate(productButtonPrefab, contentRoot);
                instance.Bind(storeController, definition);
                spawnedButtons.Add(instance);
            }
        }

        private void ClearSpawnedButtons()
        {
            for (var i = spawnedButtons.Count - 1; i >= 0; i--)
            {
                var button = spawnedButtons[i];
                if (button == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(button.gameObject);
                }
                else
                {
                    DestroyImmediate(button.gameObject);
                }
            }

            spawnedButtons.Clear();
        }
    }
}
