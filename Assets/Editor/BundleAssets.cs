using System.IO;
using UnityEditor;

public class CreateAssetBundles
{
  [MenuItem("Assets/Build AssetBundles")]
  static void BuildAllAssetBundles()
  {
    string outputPath = "AssetBundles";

    // Ensure the output directory exists
    Directory.CreateDirectory(outputPath);

    BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
  }
}
