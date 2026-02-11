using System.Collections.Generic;
using IdleClickerKit.Core;
using UnityEngine;

namespace IdleClickerKit.UI
{
    public sealed class UpgradeListView : MonoBehaviour
    {
        [SerializeField]
        private IdleClickerManager manager;

        [SerializeField]
        private UpgradeButtonView upgradeButtonPrefab;

        [SerializeField]
        private Transform contentRoot;

        [SerializeField]
        private bool rebuildOnStart = true;

        private readonly List<UpgradeButtonView> spawnedButtons = new List<UpgradeButtonView>();

        private void Start()
        {
            if (rebuildOnStart)
            {
                Build();
            }
        }

        [ContextMenu("Build Upgrade Buttons")]
        public void Build()
        {
            if (manager == null || upgradeButtonPrefab == null || contentRoot == null)
            {
                Debug.LogWarning("[IdleClicker] UpgradeListView is not configured.");
                return;
            }

            if (!manager.IsReady)
            {
                manager.Initialize();
            }

            ClearSpawnedButtons();
            var definitions = manager.GetUpgradeDefinitions();
            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                if (definition == null || string.IsNullOrWhiteSpace(definition.id))
                {
                    continue;
                }

                var instance = Instantiate(upgradeButtonPrefab, contentRoot);
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
