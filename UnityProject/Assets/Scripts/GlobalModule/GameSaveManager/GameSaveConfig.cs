using System.IO;
using UnityEngine;

public static class GameSaveConfig
{
#if UNITY_EDITOR
    public static readonly string UserSaveFilePath = Path.Combine(Application.persistentDataPath, "User.json");
    public static string GetDuelSceneSavePath(int saveSlotIndex)
    {
        return Path.Combine(Application.persistentDataPath, "Scene", "Duel", $"DuelScene_{saveSlotIndex}.json");
    }
#else
    // TODO 打包版本的存档路径
#endif

    public const string SavableObj_Type_Field_Name = "_type";
    public const string SavableDict_Inner_Dict_Field_Name = "_innerDict";
    public const string SavableSet_Inner_Set_Field_Name = "_innerSet";
    public const string SavableList_Inner_List_Field_Name = "_innerList";
    public const string SavableList_Count_Field_Name = "_count";
}
