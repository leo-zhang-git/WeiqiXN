using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using XNClient.Logger;

public class GameSaveManager : ModuleBase
{
    public bool savingLock;

    public override void Init()
    {
        savingLock = false;
    }

    public void SaveData(SavableObj savableObj, string saveFilePath)
    {
        if (savingLock) {
            XNLogger.LogError("Saving lock is being occupied, save data failed.");
            return;
        }

        string saveRootName = Path.GetFileNameWithoutExtension(saveFilePath);
        string saveDirPath = Path.GetDirectoryName(saveFilePath);
        Directory.CreateDirectory(saveDirPath);
        if (!File.Exists(saveFilePath)) {
            File.Create(saveFilePath).Close();
        }

        if (string.IsNullOrEmpty(savableObj.savePath)) {
            savableObj.savePath = saveRootName;
        }
        JObject saveJObject = savableObj.SaveObj();
        File.WriteAllText(saveFilePath, saveJObject.ToString());
        XNLogger.LogInfo("Save data success.", ("saveRootName", saveRootName), ("saveFilePath", saveFilePath));
    }

    public async Task SaveDataAsync(SavableObj savableObj, string saveFilePath)
    {
        if (savingLock) {
            XNLogger.LogError("Saving lock is being occupied, save data async failed.");
            return;
        }

        savingLock = true;
        Global.Instance.uiManager.ShowPage<SavingPopup>();
        string saveRootName = Path.GetFileNameWithoutExtension(saveFilePath);
        string saveDirPath = Path.GetDirectoryName(saveFilePath);
        Directory.CreateDirectory(saveDirPath);
        if (!File.Exists(saveFilePath)) {
            File.Create(saveFilePath).Close();
        }

        if (string.IsNullOrEmpty(savableObj.savePath)) {
            savableObj.savePath = saveRootName;
        }
        JObject saveJObject = savableObj.SaveObj();
        await File.WriteAllTextAsync(saveFilePath, saveJObject.ToString());
        savingLock = false;
        Global.Instance.uiManager.ClosePage<SavingPopup>();
        XNLogger.LogInfo("Save data async success.", ("saveRootName", saveRootName), ("saveFilePath", saveFilePath));
    }

    public void LoadData(SavableObj savableObj, string saveFilePath)
    {
        if (!File.Exists(saveFilePath)) {
            XNLogger.LogError("Save file not exists, load save data failed.", ("saveFilePath", saveFilePath));
            return;
        }

        string saveRootName = Path.GetFileNameWithoutExtension(saveFilePath);
        if (string.IsNullOrEmpty(savableObj.savePath)) {
            savableObj.savePath = saveRootName;
        }
        string jsonStr = File.ReadAllText(saveFilePath);
        try {
            JObject jObject = JObject.Parse(jsonStr);
            savableObj.LoadObj(jObject);
            XNLogger.LogInfo("Load data success.", ("saveRootName", saveRootName), ("saveFilePath", saveFilePath));
        }
        catch (Exception ex) {
            XNLogger.LogError("Load data failed.", ("saveRootName", saveRootName), ("saveFilePath", saveFilePath), ("err", ex.Message));
        }
    }
}


