using Data;
using Extensions.Reactive;
using UnityEngine;

namespace Services.SaveLoad
{
    public class SaveLoadService
    {
        public ReactiveEvent CallSaving { get; private set; } 
        public ReactiveEvent<ReactiveEvent<SaveData.LevelData>> OnSaveLevelHandler { get; private set; }
        public ReactiveEvent<ReactiveEvent<SaveData.PlayerData>> OnSavePlayerHandler { get; private set; }
        
        public ReactiveEvent CallLoading { get; private set; }
        public ReactiveEvent<SaveData> OnLoad { get; private set; }
        
        
        public SaveLoadService()
        {
            CallSaving = new ();
            CallSaving.SubscribeWithSkip(CreateSave);
            CallLoading = new ();
            CallLoading.SubscribeWithSkip(Load);
            
            OnSaveLevelHandler = new();
            OnSavePlayerHandler = new();
            OnLoad = new();
        }

        private void CreateSave()
        {
            SaveData.PlayerData playerData = new();
            SaveData.LevelData levelData = new();
            
            ReactiveEvent<SaveData.LevelData> saveLevelCallback = new(); 
            saveLevelCallback.SubscribeWithSkip(data => levelData = data);
            OnSaveLevelHandler.Notify(saveLevelCallback);

            ReactiveEvent<SaveData.PlayerData> savePlayerCallback = new();
            savePlayerCallback.SubscribeWithSkip(data => playerData = data);
            OnSavePlayerHandler.Notify(savePlayerCallback);
            
            SaveData saveData = new SaveData(playerData, levelData);
            Save(Consts.SaveSlotName, saveData);
        }
        
        private void Save(string slotKey, SaveData saveData)
        {
            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(slotKey, json);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            SaveData save = null;
            if (PlayerPrefs.HasKey(Consts.SaveSlotName))
            {
                string json = PlayerPrefs.GetString(Consts.SaveSlotName);
                save = JsonUtility.FromJson<SaveData>(json);
                OnLoad.Notify(save);
            }
        }

        private bool SaveExist()
        {
            return PlayerPrefs.HasKey(Consts.SaveSlotName);
        }
    }
}