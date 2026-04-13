using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

public class ChessBoardDataType
{
    public string id;  // id
    public int boardSize;  // 棋盘大小

    public static Dictionary<string, ChessBoardDataType> ChessBoardDict;

    public static ChessBoardDataType GetConfigData(string id)
    {
        if (ChessBoardDict == null) {
            ChessBoardDict = new Dictionary<string, ChessBoardDataType>();
            string jsonPath = Path.Combine(GlobalConfig.PATH_CONFIG_JSON, "chess_board", "chess_board.json");
            var jsonObj = JObject.Parse(File.ReadAllText(jsonPath));
            foreach (var property in jsonObj.Properties()) {
                try {
                    var item = property.Value.ToObject<ChessBoardDataType>();
                    ChessBoardDict[property.Name] = item;
                }
                catch (Exception ex) {
                    Logger.LogError($"读表错误，跳过条目 {property.Name}: {ex.Message}");
                }
            }
        }
        if (ChessBoardDict.TryGetValue(id, out ChessBoardDataType data)) {
            return data;
        } else {
            return null;
        }
    }
}