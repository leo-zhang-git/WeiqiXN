using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

public class GameSaveManager : ModuleBase
{
    public bool savingLock;

    public override void Init()
    {
        savingLock = false;
    }

    public void SaveData(ISavableRoot saveRoot)
    {
        if (savingLock) {
            Logger.LogError("Saving lock is being occupied, save data failed.");
            return;
        }

        string saveDirPath = Path.GetDirectoryName(saveRoot.saveFilePath);
        Directory.CreateDirectory(saveDirPath);
        if (!File.Exists(saveRoot.saveFilePath)) {
            File.Create(saveRoot.saveFilePath).Close();
        }

        if (string.IsNullOrEmpty(saveRoot.savableObj.savePath)) {
            saveRoot.savableObj.savePath = saveRoot.saveRootName;
        }
        JObject saveJObject = saveRoot.savableObj.SaveObj();
        File.WriteAllText(saveRoot.saveFilePath, saveJObject.ToString());
        Logger.LogInfo("Save data success.", ("saveRoot", saveRoot.saveRootName), ("saveFile", saveRoot.saveFilePath));
    }

    public async Task SaveDataAsync(ISavableRoot saveRoot)
    {
        if (savingLock) {
            Logger.LogError("Saving lock is being occupied, save data async failed.");
            return;
        }

        savingLock = true;
        Global.Instance.uiManager.ShowPage<SavingPopup>();
        string saveDirPath = Path.GetDirectoryName(saveRoot.saveFilePath);
        Directory.CreateDirectory(saveDirPath);
        if (!File.Exists(saveRoot.saveFilePath)) {
            File.Create(saveRoot.saveFilePath).Close();
        }

        if (string.IsNullOrEmpty(saveRoot.savableObj.savePath)) {
            saveRoot.savableObj.savePath = saveRoot.saveRootName;
        }
        JObject saveJObject = saveRoot.savableObj.SaveObj();
        await File.WriteAllTextAsync(saveRoot.saveFilePath, saveJObject.ToString());
        savingLock = false;
        Global.Instance.uiManager.ClosePage<SavingPopup>();
        Logger.LogInfo("Save data async success.", ("saveRoot", saveRoot.saveRootName), ("saveFile", saveRoot.saveFilePath));
    }

    public void LoadData(ISavableRoot saveRoot)
    {
        if (!File.Exists(saveRoot.saveFilePath)) {
            Logger.LogError("Save file not exists, load save data failed.", ("saveFilePath", saveRoot.saveFilePath));
            return;
        }

        if (string.IsNullOrEmpty(saveRoot.savableObj.savePath)) {
            saveRoot.savableObj.savePath = saveRoot.saveRootName;
        }
        string jsonStr = File.ReadAllText(saveRoot.saveFilePath);
        try {
            JObject jObject = JObject.Parse(jsonStr);
            saveRoot.savableObj.LoadObj(jObject);
            Logger.LogInfo("Load data success.", ("saveRoot", saveRoot.saveRootName), ("saveFile", saveRoot.saveFilePath));
        }
        catch (Exception ex) {
            Logger.LogError("Load data failed.", ("saveRoot", saveRoot.saveRootName), ("saveFile", saveRoot.saveFilePath), ("err", ex.Message));
        }
    }
}

