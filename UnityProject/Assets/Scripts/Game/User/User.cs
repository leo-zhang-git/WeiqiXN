using System.Collections.Generic;
using System.IO;

public class User : SavableObj, ISavableRoot
{
    private static User _instance;
    public static User Instance
    {
        get
        {
            if (_instance == null) {
                _instance = new User();
            }
            return _instance;
        }
    }

    public SavableObj savableObj => this;
    public string saveRootName => "User";
    public string saveFilePath => GameSaveConfig.UserSaveFilePath;
    public List<UserComponentBase> compList = new List<UserComponentBase>();

    public UserComponentUserInfo compUserInfo;

    public void Init()
    {
        compUserInfo = new UserComponentUserInfo(this);

        if (File.Exists(saveFilePath)) {
            Global.Instance.gameSaveManager.LoadData(this);
        } else {
            compUserInfo.CreateNewUser();
            Global.Instance.gameSaveManager.SaveData(this);
        }
    }

    public void Destroy()
    {
        foreach (var comp in compList) {
            comp.OnDestroy();
        }
        _instance = null;
    }
}
