using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using XNClient.Logger;

public class TmpSpriteDataType
{
    public string id;  // id
    public float scale;  // glyph的scale
    public float bx;  // glyph的x
    public float by;  // glyph的y
    public float ad;  // glyph的ad

    public static Dictionary<string, TmpSpriteDataType> TmpSpriteDict;

    public static TmpSpriteDataType GetConfigData(string id)
    {
        if (TmpSpriteDict == null) {
            TmpSpriteDict = new Dictionary<string, TmpSpriteDataType>();
            string jsonPath = Path.Combine(GlobalConfig.PATH_CONFIG_JSON, "tmp_sprite", "tmp_sprite.json");
            var jsonObj = JObject.Parse(File.ReadAllText(jsonPath));
            foreach (var property in jsonObj.Properties()) {
                try {
                    var item = property.Value.ToObject<TmpSpriteDataType>();
                    TmpSpriteDict[property.Name] = item;
                }
                catch (Exception ex) {
                    XNLogger.LogError($"读表错误，跳过条目 {property.Name}: {ex.Message}");
                }
            }
        }
        if (TmpSpriteDict.TryGetValue(id, out TmpSpriteDataType data)) {
            return data;
        } else {
            return null;
        }
    }
}