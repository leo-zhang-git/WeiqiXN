using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AssetBundleGenerator
{
    [MenuItem("Assets/打包/打PC包")]
    public static void BuildWindows()
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
        PackAllModelFiles();
        PackAllSceneFiles();
        PackAllUIPrefabFiles();

        BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
        // 开启了DisableWriteTypeTree后编辑器会无法正常序列化AB包，仅在打正式包时开启
        if (BuildConfig.BUILD_BUNDLE_DISABLE_WRITE_TYPE_TREE) {
            options |= BuildAssetBundleOptions.DisableWriteTypeTree;
        }

        options |= BuildAssetBundleOptions.UseContentHash;
        options |= BuildAssetBundleOptions.DisableLoadAssetByFileName;  // 要求必须通过完整路径查ab包资源
        options |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;
        options |= BuildAssetBundleOptions.ChunkBasedCompression;

        var manifest = CompatibilityBuildPipeline.BuildAssetBundles(outputPath, options, BuildTarget.StandaloneWindows64);
        if (manifest != null) {
            AssetDatabase.Refresh();
            Debug.Log("AssetBundle打包完成！输出路径：" + outputPath);
        } else {
            throw new Exception($"Build windows asset bundle failed, outputPath: {outputPath}.");
        }

        PrepareBuildRootDirectory(BuildConfig.BUILD_PATH_ROOT);
        // c#先转c++再转windows可执行文件，注意这里vs需要装windows10/11 sdk
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
        // 最高级别IL2CPP优化
        PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Standalone, Il2CppCompilerConfiguration.Master);
        // 只输出脚本层堆栈
        PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
        var scenes = new[] { "Assets/Scenes/Main.unity" };
        string buildOutputPath = Path.GetFullPath(BuildConfig.BUILD_PATH_WINDOWS);
        var buildOptions = BuildOptions.CompressWithLz4HC | BuildOptions.Development;
        BuildReport report = BuildPipeline.BuildPlayer(scenes, buildOutputPath, BuildTarget.StandaloneWindows64, buildOptions);
        if (report.summary.result != BuildResult.Succeeded) {
            throw new Exception($"Build windows player failed, outputPath: {buildOutputPath}, result: {report.summary.result}.");
        }

        Debug.Log($"Windows Player打包完成！输出路径：{buildOutputPath}");
    }

    [MenuItem("Assets/打包/打包预处理/检查json表打包标签")]
    public static void PackAllJsonCfgFiles()
    {
        PackAssetsByType(BuildConfig.PATH_PACK_JSON, "TextAsset", BuildConfig.AB_LABEL_JSON);
    }

    [MenuItem("Assets/打包/打包预处理/检查model资源打包标签")]
    public static void PackAllModelFiles()
    {
        string rootFolderPath = FullPathToAssetPath(BuildConfig.PATH_PACK_MODEL);
        if (string.IsNullOrEmpty(rootFolderPath) || !Directory.Exists(BuildConfig.PATH_PACK_MODEL)) {
            Debug.LogWarning($"找不到资源目录：{BuildConfig.PATH_PACK_MODEL}");
            return;
        }

        string[] modelFolderFullPaths = Directory.GetDirectories(BuildConfig.PATH_PACK_MODEL);
        int newImportCount = 0;
        int packedModelTypeCount = 0;

        foreach (string folderFullPath in modelFolderFullPaths) {
            string folderPath = FullPathToAssetPath(folderFullPath);
            if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
                continue;

            string modelTypeName = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(modelTypeName))
                continue;

            string assetBundleName = $"{BuildConfig.AB_LABEL_MODEL}_{modelTypeName}".ToLowerInvariant();
            string[] assetGuids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });
            int modelAssetCount = 0;
            foreach (string assetGuid in assetGuids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;

                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null)
                    continue;

                if (importer.assetBundleName != assetBundleName) {
                    importer.assetBundleName = assetBundleName;
                    newImportCount++;
                }

                modelAssetCount++;
            }

            if (modelAssetCount > 0) {
                packedModelTypeCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"新增/更新 {newImportCount} 个model资源标签，按类型打包 {packedModelTypeCount} 个目录");
    }

    [MenuItem("Assets/打包/打包预处理/检查scene资源打包标签")]
    public static void PackAllSceneFiles()
    {
        PackAssetsByType(BuildConfig.PATH_PACK_SCENE, "SceneAsset", BuildConfig.AB_LABEL_SCENE);

        string rootFolderPath = FullPathToAssetPath(BuildConfig.PATH_PACK_SCENE);
        if (string.IsNullOrEmpty(rootFolderPath) || !Directory.Exists(BuildConfig.PATH_PACK_SCENE)) {
            Debug.LogError($"找不到资源目录：{BuildConfig.PATH_PACK_SCENE}");
            return;
        }

        string[] sceneFolderFullPaths = Directory.GetDirectories(BuildConfig.PATH_PACK_SCENE);
        int newImportCount = 0;
        int packedSceneCount = 0;

        // 逐个按文件夹分场景资源包
        foreach (string folderFullPath in sceneFolderFullPaths) {
            string folderPath = FullPathToAssetPath(folderFullPath);
            if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
                continue;

            string sceneName = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(sceneName))
                continue;

            string assetBundleName = $"{BuildConfig.AB_LABEL_SCENE}_{sceneName}".ToLowerInvariant();
            string[] assetGuids = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });
            int sceneAssetCount = 0;
            foreach (string assetGuid in assetGuids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;

                if (string.Equals(Path.GetExtension(assetPath), ".unity", StringComparison.OrdinalIgnoreCase))
                    continue;

                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null)
                    continue;

                if (importer.assetBundleName != assetBundleName) {
                    importer.assetBundleName = assetBundleName;
                    newImportCount++;
                }

                sceneAssetCount++;
            }

            if (sceneAssetCount > 0) {
                packedSceneCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"新增/更新 {newImportCount} 个资源标签，按场景打包 {packedSceneCount} 个目录");
    }

    [MenuItem("Assets/打包/打包预处理/检查UI资源打包标签")]
    public static void PackAllUIPrefabFiles()
    {
        PackAssetsByType(BuildConfig.PATH_PACK_UI_PREFAB, "GameObject", BuildConfig.AB_LABEL_UI_PREFAB);
        PackAssetsByType(BuildConfig.PATH_PACK_UI_TEXTUER, "Texture2D", BuildConfig.AB_LABEL_UI_TEXTURE);
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

    private static void PrepareBuildRootDirectory(string buildRootPath)
    {
        string fullBuildRootPath = Path.GetFullPath(buildRootPath);
        if (Directory.Exists(fullBuildRootPath)) {
            Directory.Delete(fullBuildRootPath, true);
        }

        Directory.CreateDirectory(fullBuildRootPath);
    }
}
