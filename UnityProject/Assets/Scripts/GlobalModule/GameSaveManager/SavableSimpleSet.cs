using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class SavableSimpleSet<TValue> : SavableObj
{
    private HashSet<TValue> _innerSet = new HashSet<TValue>();
    public int Count => _innerSet.Count;

    private SavableSimpleSet()
    {

    }

    public static SavableSimpleSet<int> CreateIntSet()
    {
        return new SavableSimpleSet<int>();
    }

    public static SavableSimpleSet<float> CreateFloatSet()
    {
        return new SavableSimpleSet<float>();
    }

    public static SavableSimpleSet<bool> CreateBoolSet()
    {
        return new SavableSimpleSet<bool>();
    }

    public static SavableSimpleSet<string> CreateStringSet()
    {
        return new SavableSimpleSet<string>();
    }

    public void Add(TValue value)
    {
        _innerSet.Add(value);
    }

    public void Remove(TValue value)
    {
        _innerSet.Remove(value);
    }

    public bool Contains(TValue value)
    {
        return _innerSet.Contains(value);
    }

    public void Clear()
    {
        _innerSet.Clear();
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        return _innerSet.GetEnumerator();
    }

    public override JObject SaveObj()
    {
        var jObject = new JObject();
        jObject[GameSaveUtils.SavableObj_Type_Field_Name] = JToken.FromObject(GetType().FullName);

        var innerSetJObject = new JObject();
        jObject[GameSaveUtils.SavableSet_Inner_Set_Field_Name] = innerSetJObject;

        int index = 0;
        foreach (var value in _innerSet) {
            if (value != null) {
                innerSetJObject[index.ToString()] = JToken.FromObject(value);
                index++;
            }
        }

        return jObject;
    }

    public override void LoadObj(JObject jObject)
    {
        _innerSet.Clear();
        var innerSetJToken = jObject[GameSaveUtils.SavableSet_Inner_Set_Field_Name];
        if (innerSetJToken == null) {
            return;
        }

        var innerSetJObject = (JObject)innerSetJToken;
        foreach (var prop in innerSetJObject.Properties()) {
            try {
                TValue value = prop.Value.ToObject<TValue>();
                _innerSet.Add(value);
            }
            catch (Exception ex) {
                _innerSet.Clear();
                Logger.LogError($"Failed to load savable simple set.", ("err", ex.Message));
            }
        }
    }
}
