using System.IO;
using UnityEngine;

public static class GameSaveConfig
{
#if UNITY_EDITOR
    public static readonly string UserSaveFilePath = Path.Combine(Application.persistentDataPath, "User");
#else
    // TODO 打包版本的存档路径
#endif
}
