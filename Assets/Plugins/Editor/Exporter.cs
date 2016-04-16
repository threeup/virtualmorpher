using UnityEngine;
using System.Collections;
using UnityEditor;
	
public class Exporter {
	[MenuItem("Assets/ExportWithSettings")]
	static void Export () {
		AssetDatabase.ExportPackage ("Assets", "TankZone5.unitypackage", 
ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies);
	}
}