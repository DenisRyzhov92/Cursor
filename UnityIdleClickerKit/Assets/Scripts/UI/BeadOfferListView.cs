using System.Collections.Generic;
using IdleClickerKit.Core;
using UnityEngine;

namespace IdleClickerKit.UI
{
    public sealed class BeadOfferListView : MonoBehaviour
    {
        [SerializeField]
        private IdleClickerManager manager;

        [SerializeField]
        private BeadOfferButtonView offerButtonPrefab;

        [SerializeField]
        private Transform contentRoot;

        [SerializeField]
        private bool rebuildOnStart = true;

        private readonly List<BeadOfferButtonView> spawnedButtons = new List<BeadOfferButtonView>();

        private void Start()
        {
            if (rebuildOnStart)
            {
                Build();
            }
        }

        [ContextMenu("Build Bead Offer Buttons")]
        public void Build()
        {
            if (manager == null || offerButtonPrefab == null || contentRoot == null)
            {
                Debug.LogWarning("[IdleClicker] BeadOfferListView is not configured.");
                return;
            }

            if (!manager.IsReady)
            {
                manager.Initialize();
            }

            ClearSpawnedButtons();
            var definitions = manager.GetBeadExchangeOfferDefinitions();
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
