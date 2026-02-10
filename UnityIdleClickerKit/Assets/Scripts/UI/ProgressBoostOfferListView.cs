using System.Collections.Generic;
using IdleClickerKit.Core;
using UnityEngine;

namespace IdleClickerKit.UI
{
    public sealed class ProgressBoostOfferListView : MonoBehaviour
    {
        [SerializeField]
        private IdleClickerManager manager;

        [SerializeField]
        private ProgressBoostOfferButtonView offerButtonPrefab;

        [SerializeField]
        private Transform contentRoot;

        [SerializeField]
        private bool rebuildOnStart = true;

        private readonly List<ProgressBoostOfferButtonView> spawnedButtons = new List<ProgressBoostOfferButtonView>();

        private void Start()
        {
            if (rebuildOnStart)
            {
                Build();
            }
        }

        [ContextMenu("Build Progress Boost Buttons")]
        public void Build()
        {
            if (manager == null || offerButtonPrefab == null || contentRoot == null)
            {
                Debug.LogWarning("[IdleClicker] ProgressBoostOfferListView is not configured.");
                return;
            }

            if (!manager.IsReady)
            {
                manager.Initialize();
            }

            ClearSpawnedButtons();
            var definitions = manager.GetProgressBoostOfferDefinitions();
            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                if (definition == null || string.IsNullOrWhiteSpace(definition.id))
                {
                    continue;
                }

                var instance = Instantiate(offerButtonPrefab, contentRoot);
                instance.Bind(manager, definition.id, definition.title, definition.description);
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
