using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 临时测试用，后续数据类定义改成导表工具自动导出
public class SceneDataType
{
    public string id; // 编号
    public string sceneName; // 名称
    public string sceneType; // 资源编号

    public static Dictionary<string, SceneDataType> SceneDataDict;

    public static SceneDataType GetSceneData(string id)
    {
        if (SceneDataDict == null) {
            SceneDataDict = new Dictionary<string, SceneDataType>();
            //string jsonPath = Path.Combine(CommonPathDefine.jsonCfgPath, $"{typeof(SceneCfg).Name}.json");
            string jsonPath = Path.Combine(Application.dataPath, $"{typeof(SceneDataType).Name}.json");
            try {
                string jsonStr = File.ReadAllText(jsonPath);
                SceneDataDict = JsonConvert.DeserializeObject<Dictionary<string, SceneDataType>>(jsonStr);
            }
            catch (Exception e) {
                Logger.LogError($"Get {typeof(SceneDataType).Name} config failed, sceneId: {id}");
                return null;
            }
        }

        if (SceneDataDict.TryGetValue(id, out SceneDataType data)) {
            return data;
        } else {
            return null;
        }
    }
}
