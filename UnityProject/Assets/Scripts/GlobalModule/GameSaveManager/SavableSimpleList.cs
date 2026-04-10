using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class SavableSimpleList<TValue> : SavableObj
{
    protected List<TValue> _innerList = new List<TValue>();
    public int Count => _innerList.Count;

    public SavableSimpleList()
    {

    }

    public bool TryGetValue(int index, out TValue value)
    {
        if (index < 0 || index >= _innerList.Count) {
            value = default(TValue);
            return false;
        }

        value = _innerList[index];
        return true;
    }

    public void Add(TValue value)
    {
        _innerList.Add(value);
    }

    public bool TrySetValue(int index, TValue value)
    {
        if (index >= 0 && index < _innerList.Count) {
            _innerList[index] = value;
            return true;
        } else {
            return false;
        }
    }

    public void RemoveAt(int index)
    {
        if (index >= 0 && index < _innerList.Count) {
            _innerList.RemoveAt(index);
        }
    }

    public void Remove(TValue value)
    {
        _innerList.Remove(value);
    }

    public void Clear()
    {
        _innerList.Clear();
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        return _innerList.GetEnumerator();
    }

    public override JObject SaveObj()
    {
        var jObject = new JObject();
        jObject[GameSaveUtils.SavableObj_Type_Field_Name] = JToken.FromObject(GetType().FullName);

        var innerListJObject = new JObject();
        jObject[GameSaveUtils.SavableList_Inner_List_Field_Name] = innerListJObject;
        jObject[GameSaveUtils.SavableList_Count_Field_Name] = JToken.FromObject(Count);

        for (int i = 0; i < _innerList.Count; i++) {
            innerListJObject[i.ToString()] = JToken.FromObject(_innerList[i]);
        }

        return jObject;
    }

    public override void LoadObj(JObject jObject)
    {
        _innerList.Clear();
        var innerListCountJToken = jObject[GameSaveUtils.SavableList_Count_Field_Name];
        if (innerListCountJToken != null && innerListCountJToken.Type == JTokenType.Integer) {
            for (int i = 0; i < (int)innerListCountJToken; i++) {
                _innerList.Add(default);
            }
        }

        var innerListJToken = jObject[GameSaveUtils.SavableList_Inner_List_Field_Name];
        if (innerListJToken == null) {
            return;
        }

        var innerListJObject = (JObject)innerListJToken;
        foreach (var prop in innerListJObject.Properties()) {
            string key = prop.Name;
            try {
                TValue value = prop.Value.ToObject<TValue>();
                int index = int.Parse(key);
                if (index >= 0 && index < (int)innerListCountJToken) {
                    _innerList[index] = value;
                }
            }
            catch (Exception ex) {
                _innerList.Clear();
                Logger.LogError($"Failed to load savable simple list.", ("err", ex.Message));
            }
        }
    }
}

public static class SavableSimpleListFactory
{
    public static SavableSimpleList<int> CreateIntList()
    {
        return new SavableSimpleList<int>();
    }

    public static SavableSimpleList<float> CreateFloatList()
    {
        return new SavableSimpleList<float>();
    }

    public static SavableSimpleList<bool> CreateBoolList()
    {
        return new SavableSimpleList<bool>();
    }

    public static SavableSimpleList<string> CreateStringList()
    {
        return new SavableSimpleList<string>();
    }
}
