using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

public class GameSaveManager : ModuleBase
{
    public bool savingLock;

    public override void Init()
    {
        savingLock = false;
    }

    public async Task SaveData(ISavableRoot saveRoot)
    {
        if (savingLock) {
            Logger.LogError("Saving lock is being occupied, save object failed.");
            return;
        }

        savingLock = true;
        string saveDirPath = Path.GetDirectoryName(saveRoot.saveFilePath);
        Directory.CreateDirectory(saveDirPath);
        if (!File.Exists(saveRoot.saveFilePath)) {
            File.Create(saveRoot.saveFilePath);
        }

        if (string.IsNullOrEmpty(saveRoot.savableObj.savePath)) {
            saveRoot.savableObj.savePath = saveRoot.saveRootName;
        }
        JObject saveJObject = saveRoot.savableObj.SaveObj();
        await File.WriteAllTextAsync(saveRoot.saveFilePath, saveJObject.ToString());
        savingLock = false;
    }

    public void LoadData(ISavableRoot saveRoot)
    {
        if (!File.Exists(saveRoot.saveFilePath)) {
            Logger.LogError("Save file not exists, load save object failed.", ("saveFilePath", saveRoot.saveFilePath));
            return;
        }
    }
}
