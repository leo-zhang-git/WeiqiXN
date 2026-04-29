using System.IO;
using UnityEditor;

public class AssetBundleGenerator
{
    [MenuItem("Assets/Build AssetBundles (Windows)")]
    private static void BuildAllAssetBundles()
    {
        // 1. 设置输出路径
        string outputPath = "Assets/AssetBundles/Windows";

        // 2. 若目录不存在，则创建它
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        // 3. 执行打包
        BuildPipeline.BuildAssetBundles(outputPath,
                                        BuildAssetBundleOptions.None,
                                        BuildTarget.StandaloneWindows);

        // 4. 刷新编辑器，显示打包好的文件
        AssetDatabase.Refresh();
        UnityEngine.Debug.Log("AssetBundle打包完成！输出路径：" + outputPath);
    }
}
