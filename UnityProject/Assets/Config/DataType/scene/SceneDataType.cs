using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

public class SceneDataType
{
    public string id;  // id
    public string unitySceneName;  // Unity中scene资源名
    public string sceneType;  // 对应c#场景类型

    public static Dictionary<string, SceneDataType> SceneDict;

    public static SceneDataType GetConfigData(string id)
    {
        if (SceneDict == null) {
            SceneDict = new Dictionary<string, SceneDataType>();
            string jsonPath = Path.Combine(GlobalConfig.PATH_CONFIG_JSON, "scene", "scene.json");
            var jsonObj = JObject.Parse(File.ReadAllText(jsonPath));
            foreach (var property in jsonObj.Properties()) {
                try {
                    var item = property.Value.ToObject<SceneDataType>();
                    SceneDict[property.Name] = item;
                }
                catch (Exception ex) {
                    Console.WriteLine($"读表错误，跳过条目 {property.Name}: {{ex.Message}}");
                }
            }
        }
        if (SceneDict.TryGetValue(id, out SceneDataType data)) {
            return data;
        } else {
            return null;
        }
    }
}