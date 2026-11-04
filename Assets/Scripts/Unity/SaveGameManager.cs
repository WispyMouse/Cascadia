using System;
using System.IO;
namespace Cascadia.Unity
{
    using System.Linq;
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

            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
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

        public async Task<int> GetClearCount(ChallengeDefinition challenge)
        {
            while (!this.loaded)
            {
                await Task.Yield();
            }

            if (string.IsNullOrWhiteSpace(challenge.Identifier))
            {
                Debug.LogError($"Challenge named {challenge.ChallengeName} file name {challenge.name} does not have an identifier.");
            }

            return this.activeSaveGame.GetClearCount(challenge.Identifier);
        }

        public async Task SaveChallengeWin(ChallengeDefinition challenge)
        {
            await this.loadSaveSemaphore.WaitAsync();

            if (string.IsNullOrWhiteSpace(challenge.Identifier))
            {
                Debug.LogError($"Challenge named {challenge.ChallengeName} file name {challenge.name} does not have an identifier.");
            }

            ChallengeClearData existingClearData = this.activeSaveGame.ChallengeClearData.FirstOrDefault(x => x.ChallengeId == challenge.Identifier);

            if (existingClearData != null)
            {
                existingClearData.ClearCountLevel++;
            }
            else
            {
                this.activeSaveGame.ChallengeClearData.Add(new ChallengeClearData() { ChallengeId = challenge.Identifier, ClearCountLevel = 1 });
            }

            string saveString = JsonUtility.ToJson(this.activeSaveGame);
            await File.WriteAllTextAsync(saveGameLocation, saveString);

            this.loadSaveSemaphore.Release();
        }
    }
}
