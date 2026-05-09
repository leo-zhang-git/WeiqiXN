using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using XNClient.Logger;

public class SavableObjectDict<TObject> : SavableObj where TObject : SavableObj, new()
{
    public readonly Dictionary<string, TObject> _innerDict = new Dictionary<string, TObject>();
    public int Count => _innerDict.Count;

    public SavableObjectDict()
    {

    }

    public bool TryGetValue(string key, out TObject obj)
    {
        return _innerDict.TryGetValue(key, out obj);
    }

    public void SetValue(string key, TObject obj)
    {
        _innerDict[key] = obj;
    }

    public void Remove(string key)
    {
        _innerDict.Remove(key);
    }

    public bool ContainsKey(string key)
    {
        return _innerDict.ContainsKey(key);
    }

    public void Clear()
    {
        _innerDict.Clear();
    }

    public IEnumerator<KeyValuePair<string, TObject>> GetEnumerator()
    {
        return _innerDict.GetEnumerator();
    }

    public IEnumerable<string> Keys => _innerDict.Keys;

    public IEnumerable<TObject> Values => _innerDict.Values;

    public override JObject SaveObj()
    {
        var jObject = new JObject();
        jObject[GameSaveConfig.SavableObj_Type_Field_Name] = JToken.FromObject(GetType().FullName);

        var innerDictJObject = new JObject();
        jObject[GameSaveConfig.SavableDict_Inner_Dict_Field_Name] = innerDictJObject;

        foreach (var kvp in _innerDict) {
            if (kvp.Value == null) {
                continue;
            }

            kvp.Value.savePath = $"{savePath}.{kvp.Key}";
            JObject childJObject = kvp.Value.SaveObj();
            innerDictJObject[kvp.Key] = JToken.FromObject(childJObject);
        }

        return jObject;
    }

    public override void LoadObj(JObject jObject)
    {
        _innerDict.Clear();
        var innerDictJToken = jObject[GameSaveConfig.SavableDict_Inner_Dict_Field_Name];
        if (innerDictJToken == null) {
            return;
        }

        var innerDictJObject = innerDictJToken as JObject;
        if (innerDictJObject == null) {
            XNLogger.LogError("Failed to load savable object dict, inner dict token is invalid.", ("savePath", savePath));
            return;
        }

        foreach (var prop in innerDictJObject.Properties()) {
            string key = prop.Name;
            try {
                var childJObject = prop.Value as JObject;
                if (childJObject == null) {
                    throw new Exception("Child jObject is invalid.");
                }

                var childTypeJToken = childJObject[GameSaveConfig.SavableObj_Type_Field_Name];
                if (childTypeJToken == null) {
                    throw new Exception("Child type name not found.");
                }

                string childTypeFullName = childTypeJToken.ToObject<string>();
                if (childTypeFullName != typeof(TObject).FullName) {
                    throw new Exception($"Child type invalid: {childTypeFullName}");
                }

                TObject childObj = new TObject();
                childObj.savePath = $"{savePath}.{key}";
                childObj.LoadObj(childJObject);
                _innerDict[key] = childObj;
            }
            catch (Exception ex) {
                _innerDict.Clear();
                XNLogger.LogError($"Failed to load savable object dict.", ("key", key), ("err", ex.Message));
            }
        }
    }
}
