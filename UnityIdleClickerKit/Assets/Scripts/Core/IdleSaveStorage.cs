using System;
using System.IO;
using IdleClickerKit.Data;
using UnityEngine;

namespace IdleClickerKit.Core
{
    public static class IdleSaveStorage
    {
        private const string DefaultSaveFileName = "idle_clicker_save.json";

        public static IdleSaveData Load(string saveFileName = DefaultSaveFileName)
        {
            var path = GetPath(saveFileName);
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var raw = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return null;
                }

                return JsonUtility.FromJson<IdleSaveData>(raw);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[IdleClicker] Failed to read save file: {exception.Message}");
                return null;
            }
        }

        public static void Save(IdleSaveData saveData, string saveFileName = DefaultSaveFileName)
        {
            if (saveData == null)
            {
                return;
            }

            var path = GetPath(saveFileName);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                var raw = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(path, raw);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[IdleClicker] Failed to save progress: {exception.Message}");
            }
        }

        public static void Delete(string saveFileName = DefaultSaveFileName)
        {
            var path = GetPath(saveFileName);
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                File.Delete(path);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[IdleClicker] Failed to delete save file: {exception.Message}");
            }
        }

        public static string GetPath(string saveFileName = DefaultSaveFileName)
        {
            var fileName = string.IsNullOrWhiteSpace(saveFileName) ? DefaultSaveFileName : saveFileName;
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }
}
