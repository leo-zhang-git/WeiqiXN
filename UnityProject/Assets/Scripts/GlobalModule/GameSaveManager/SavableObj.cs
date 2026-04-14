using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using XNClient.Logger;

public class SavableObj
{
    public string savePath = string.Empty;
    private HashSet<SavableObj> visitedSavableObjs = new HashSet<SavableObj>();

    public virtual JObject SaveObj()
    {
        var jObject = new JObject();
        if (string.IsNullOrEmpty(savePath)) {
            XNLogger.LogError("Save path not set, save obj failed.", ("objType", GetType().Name));
            return jObject;
        }

        Type saveType = GetType();
        jObject[GameSaveConfig.SavableObj_Type_Field_Name] = JToken.FromObject(saveType.FullName);
        visitedSavableObjs.Add(this);

        foreach (var field in saveType.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(SavableField<>)) {
                var valueType = field.FieldType.GetGenericArguments()[0];
                if (!CheckValidSaveType(valueType)) {
                    XNLogger.LogError("Invalid type for savableField, skip save field.", ("objType", GetType().Name), ("fieldName", field.Name), ("valueType", valueType.Name));
                    continue;
                }
                // Extract TValue inside savableField
                var savableFieldProp = field.GetValue(this).GetType().GetProperty("value");
                var savableFieldVal = savableFieldProp.GetValue(field.GetValue(this));
                if (savableFieldVal != null) {
                    jObject[field.Name] = JToken.FromObject(savableFieldVal);
                }
            } else if (typeof(SavableObj).IsAssignableFrom(field.FieldType)) {
                if (field.GetCustomAttribute<SkipSavableCheckAttribute>() != null) {
                    continue;
                }

                var childObj = (SavableObj)field.GetValue(this);
                if (childObj != null) {
                    if (visitedSavableObjs.Contains(childObj)) {
                        XNLogger.LogError("Savable field chain checked, skip save child obj.", ("fieldName", field.Name));
                        continue;
                    }

                    childObj.savePath = $"{savePath}.{field.Name}";
                    childObj.visitedSavableObjs = new HashSet<SavableObj>(visitedSavableObjs);
                    JObject childJObject = childObj.SaveObj();
                    jObject[field.Name] = JToken.FromObject(childJObject);
                }
            }
        }

        visitedSavableObjs.Clear();
        return jObject;
    }

    public virtual void LoadObj(JObject jObject)
    {
        if (string.IsNullOrEmpty(savePath)) {
            XNLogger.LogError("Save path not set, load obj failed.", ("objType", GetType().Name));
            return;
        }

        if (jObject[GameSaveConfig.SavableObj_Type_Field_Name] == null) {
            XNLogger.LogError("Type name of jObject not found, load jObject failed.", ("typeFullName", GetType().FullName));
            return;
        }
        Type loadType = Type.GetType((string)jObject[GameSaveConfig.SavableObj_Type_Field_Name].ToObject(typeof(string)));
        if (loadType == null || loadType.FullName != GetType().FullName) {
            XNLogger.LogError("Load type name invalid, load jObject failed.", ("typeFullName", GetType().FullName));
            return;
        }

        foreach (var field in loadType.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
            if (jObject[field.Name] != null) {
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(SavableField<>)) {
                    var valueType = field.FieldType.GetGenericArguments()[0];
                    if (!CheckValidSaveType(valueType)) {
                        XNLogger.LogError("Invalid type for savableField, skip load field.", ("objType", GetType().Name), ("fieldName", field.Name), ("valueType", valueType.Name));
                        continue;
                    }
                    var value = jObject[field.Name].ToObject(valueType);
                    field.GetValue(this).GetType().GetProperty("value").SetValue(field.GetValue(this), value);
                } else if (typeof(SavableObj).IsAssignableFrom(field.FieldType)) {
                    var childJObject = jObject[field.Name] as JObject;
                    var childObj = (SavableObj)field.GetValue(this);
                    if (childJObject != null && childObj != null) {
                        childObj.savePath = $"{savePath}.{field.Name}";
                        childObj.LoadObj(childJObject);
                    }
                }
            }
        }
    }

    public bool CheckValidSaveType(Type valueType)
    {
        if (valueType == typeof(int) ||
            valueType == typeof(float) ||
            valueType == typeof(bool) ||
            valueType == typeof(string)) {
            return true;
        }
        return false;
    }
}

