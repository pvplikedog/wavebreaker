using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Pathfinding
{
#if UNITY_EDITOR
    /// <summary>Internal utility class for looking up editor resources</summary>
    public static class EditorResourceHelper
    {
	    /// <summary>
	    ///     Path to the editor assets folder for the A* Pathfinding Project. If this path turns out to be incorrect, the script
	    ///     will try to find the correct path
	    ///     See: LoadStyles
	    /// </summary>
	    public static string editorAssets;

        private static Material surfaceMat, lineMat;
        private static Texture2D handlesAALineTex;

        static EditorResourceHelper()
        {
            // Look up editor assets directory when first accessed
            LocateEditorAssets();
        }

        public static Material GizmoSurfaceMaterial
        {
            get
            {
                if (!surfaceMat)
                    surfaceMat =
                        AssetDatabase.LoadAssetAtPath(editorAssets + "/Materials/Navmesh.mat", typeof(Material)) as
                            Material;
                return surfaceMat;
            }
        }

        public static Material GizmoLineMaterial
        {
            get
            {
                if (!lineMat)
                    lineMat = AssetDatabase.LoadAssetAtPath(editorAssets + "/Materials/NavmeshOutline.mat",
                        typeof(Material)) as Material;
                return lineMat;
            }
        }

        public static Texture2D HandlesAALineTexture
        {
            get
            {
                if (!handlesAALineTex) handlesAALineTex = Resources.Load<Texture2D>("handles_aaline");
                return handlesAALineTex;
            }
        }

        /// <summary>Locates the editor assets folder in case the user has moved it</summary>
        public static bool LocateEditorAssets()
        {
#if UNITY_2019_3_OR_NEWER
            var package = PackageInfo.FindForAssembly(typeof(EditorResourceHelper).Assembly);
            if (package != null)
            {
                editorAssets = package.assetPath + "/Editor/EditorAssets";
                if (File.Exists(package.resolvedPath + "/Editor/EditorAssets/AstarEditorSkinLight.guiskin"))
                    return true;

                Debug.LogError("Could not find editor assets folder in package at " + editorAssets +
                               ". Is the package corrupt?");
                return false;
            }
#endif

            var projectPath = Application.dataPath;

            if (projectPath.EndsWith("/Assets")) projectPath = projectPath.Remove(projectPath.Length - "Assets".Length);

            editorAssets = "Assets/AstarPathfindingProject/Editor/EditorAssets";
            if (!File.Exists(projectPath + editorAssets + "/AstarEditorSkinLight.guiskin") &&
                !File.Exists(projectPath + editorAssets + "/AstarEditorSkin.guiskin"))
            {
                //Initiate search

                var sdir = new DirectoryInfo(Application.dataPath);

                var dirQueue = new Queue<DirectoryInfo>();
                dirQueue.Enqueue(sdir);

                var found = false;
                while (dirQueue.Count > 0)
                {
                    var dir = dirQueue.Dequeue();
                    if (File.Exists(dir.FullName + "/AstarEditorSkinLight.guiskin") ||
                        File.Exists(dir.FullName + "/AstarEditorSkin.guiskin"))
                    {
                        // Handle windows file paths
                        var path = dir.FullName.Replace('\\', '/');
                        found = true;
                        // Remove data path from string to make it relative
                        path = path.Replace(projectPath, "");

                        if (path.StartsWith("/")) path = path.Remove(0, 1);

                        editorAssets = path;
                        return true;
                    }

                    var dirs = dir.GetDirectories();
                    for (var i = 0; i < dirs.Length; i++) dirQueue.Enqueue(dirs[i]);
                }

                if (!found)
                {
                    Debug.LogWarning(
                        "Could not locate editor assets folder. Make sure you have imported the package correctly.\nA* Pathfinding Project");
                    return false;
                }
            }

            return true;
        }
    }
#endif
}