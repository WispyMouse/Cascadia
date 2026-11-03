using System;
using System.IO;
namespace Cascadia.Unity
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cascadia.DataStructures;
    using UnityEngine;

    public class SaveGameManager : MonoBehaviour
    {
        public static SaveGameManager Singleton { get; private set; } = null;

        private SemaphoreSlim loadSaveSemaphore { get; set; } = new(1);
        private SaveGame activeSaveGame { get; set; }
        private static string saveGameLocation => Application.persistentDataPath + "/savegame.save";
        private bool loaded { get; set; } = false;

        private async void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Singleton = this;

            await this.loadSaveSemaphore.WaitAsync();

            bool saveLoaded = false;

            if (File.Exists(saveGameLocation))
            {
                string contents = File.ReadAllText(saveGameLocation);

                if (!string.IsNullOrEmpty(contents))
                {
                    try
                    {
                        this.activeSaveGame = JsonUtility.FromJson<SaveGame>(contents);
                        saveLoaded = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }

            if (!saveLoaded)
            {
                this.activeSaveGame = new();
            }

            this.loaded = true;

            this.loadSaveSemaphore.Release();
        }

        public async Task<SaveGame> GetSave()
        {
            if (!this.loaded)
            {
                Debug.Log($"Save is not yet loaded. Waiting for it to finish.");
            }

            while (!this.loaded)
            {
                await Task.Yield();
            }

            return this.activeSaveGame;
        }
    }
}
