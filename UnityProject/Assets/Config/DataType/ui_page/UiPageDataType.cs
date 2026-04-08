using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

public class UiPageDataType
{
    public string id;  // id
    public string contextType;  // 所在context层级
    public bool isLoadAsync;  // 是否异步加载

    public static Dictionary<string, UiPageDataType> UiPageDict;

    public static UiPageDataType GetConfigData(string id)
    {
        if (UiPageDict == null) {
            UiPageDict = new Dictionary<string, UiPageDataType>();
            string jsonPath = Path.Combine(GlobalConfig.PATH_CONFIG_JSON, "ui_page", "ui_page.json");
            var jsonObj = JObject.Parse(File.ReadAllText(jsonPath));
            foreach (var property in jsonObj.Properties()) {
                try {
                    var item = property.Value.ToObject<UiPageDataType>();
                    UiPageDict[property.Name] = item;
                }
                catch (Exception ex) {
                    Logger.LogError($"读表错误，跳过条目 {property.Name}: {ex.Message}");
                }
            }
        }
        if (UiPageDict.TryGetValue(id, out UiPageDataType data)) {
            return data;
        } else {
            return null;
        }
    }
}