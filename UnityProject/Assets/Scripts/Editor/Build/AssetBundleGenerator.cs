using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleGenerator
{
    [MenuItem("Assets/打包/Build AssetBundles (Windows)")]
    public static void BuildAllAssetBundles()
    {
        // 清空StreamingAssets目录
        string outputPath = BuildConfig.PATH_BUILDIN_ASSETBUNDLE;
        if (!Directory.Exists(outputPath)) {
            Directory.CreateDirectory(outputPath);
        } else {
            foreach (string filePath in Directory.GetFiles(outputPath)) {
                File.Delete(filePath);
            }

            foreach (string subDirectoryPath in Directory.GetDirectories(outputPath)) {
                Directory.Delete(subDirectoryPath, true);
            }
        }

        PackAllJsonCfgFiles();
        PackAllSceneFiles();
        PackAllUIPrefabFiles();

        CompatibilityBuildPipeline.BuildAssetBundles
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
        Debug.Log("AssetBundle打包完成！输出路径：" + outputPath);
    }

    [MenuItem("Assets/打包/打包预处理/检查json表打包标签")]
    public static void PackAllJsonCfgFiles()
    {
        PackAssetsByType(BuildConfig.PATH_PACK_JSON, "TextAsset", BuildConfig.AB_LABEL_JSON);
    }

    [MenuItem("Assets/打包/打包预处理/检查scene资源打包标签")]
    public static void PackAllSceneFiles()
    {
        PackAssetsByType(BuildConfig.PATH_PACK_SCENE, "SceneAsset", BuildConfig.AB_LABEL_SCENE);
    }

    [MenuItem("Assets/打包/打包预处理/检查UI prefab资源打包标签")]
    public static void PackAllUIPrefabFiles()
    {
        PackAssetsByType(BuildConfig.PATH_PACK_UI_PREFAB, "GameObject", BuildConfig.AB_LABEL_UI_PREFAB);
    }

    private static void PackAssetsByType(string rootFolderFullPath, string typeName, string assetBundleName)
    {
        string rootFolderPath = FullPathToAssetPath(rootFolderFullPath);
        if (string.IsNullOrEmpty(rootFolderPath)) {
            Debug.LogWarning($"找不到资源目录：{rootFolderFullPath}");
            return;
        }

        string[] guids = AssetDatabase.FindAssets($"t:{typeName}", new[] { rootFolderPath });
        int newImportCount = 0;
        foreach (string guid in guids) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
                continue;

            if (importer.assetBundleName != assetBundleName) {
                importer.assetBundleName = assetBundleName;
                newImportCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"新增 {newImportCount} 个 {typeName} 资源，设置 AB 标签：{assetBundleName}");
    }

    private static string FullPathToAssetPath(string fullPath)
    {
        string dataPath = Application.dataPath.Replace('\\', '/');
        string normalizedPath = fullPath.Replace('\\', '/');
        if (!normalizedPath.StartsWith(dataPath, StringComparison.OrdinalIgnoreCase))
            return null;

        return "Assets" + normalizedPath.Substring(dataPath.Length);
    }
}
