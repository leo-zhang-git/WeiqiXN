using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class SavableSimpleDict<TValue> : SavableObj
{
    private readonly Dictionary<string, TValue> _innerDict = new Dictionary<string, TValue>();

    private SavableSimpleDict()
    {

    }

    public static SavableSimpleDict<int> CreateIntDict()
    {
        return new SavableSimpleDict<int>();
    }

    public static SavableSimpleDict<float> CreateFloatDict()
    {
        return new SavableSimpleDict<float>();
    }

    public static SavableSimpleDict<bool> CreateBoolDict()
    {
        return new SavableSimpleDict<bool>();
    }

    public static SavableSimpleDict<string> CreateStringDict()
    {
        return new SavableSimpleDict<string>();
    }

    public bool TryGetValue(string key, out TValue value)
    {
        return _innerDict.TryGetValue(key, out value);
    }

    public void SetValue(string key, TValue value)
    {
        _innerDict[key] = value;
    }

    public void RemoveKey(string key)
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

    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
    {
        return _innerDict.GetEnumerator();
    }

    public IEnumerable<string> Keys
    {
        get
        {
            return _innerDict.Keys;
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            return _innerDict.Values;
        }
    }

    public override JObject SaveObj()
    {
        var jObject = new JObject();
        jObject[GameSaveUtils.SavableObj_Type_Field_Name] = JToken.FromObject(GetType().FullName);

        var innerDictJObj = new JObject();
        jObject[GameSaveUtils.SavableDict_Inner_Dict_Field_Name] = innerDictJObj;

        foreach (var kvp in _innerDict) {
            if (kvp.Value != null) {
                innerDictJObj[kvp.Key] = JToken.FromObject(kvp.Value);
            }
        }

        return jObject;
    }

    public override void LoadObj(JObject jObject)
    {
        _innerDict.Clear();
        var innerDictJToken = jObject[GameSaveUtils.SavableDict_Inner_Dict_Field_Name];
        if (innerDictJToken == null) {
            return;
        }

        var innerDictJObject = (JObject)innerDictJToken;
        foreach (var prop in innerDictJObject.Properties()) {
            string key = prop.Name;
            try {
                TValue value = prop.Value.ToObject<TValue>();
                _innerDict[key] = value;
            }
            catch (Exception ex) {
                Logger.LogError($"Failed to load savable simple dict.", ("key", key), ("err", ex.Message));
            }
        }
    }
}
