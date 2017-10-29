using UnityEngine;
using UnityEditor;

public class ExportAsset : MonoBehaviour {
	[MenuItem("Custom Editor/BuildAssetBundles")]
	static void CreatAssetBundles(){
		BuildPipeline.BuildAssetBundles ("Assets/StreamingAssets",BuildAssetBundleOptions.None,BuildTarget.Android);
	}

}
