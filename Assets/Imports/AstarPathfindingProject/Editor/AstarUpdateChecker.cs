using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER
using UnityEngine.Networking;
#endif

namespace Pathfinding
{
    /// <summary>Handles update checking for the A* Pathfinding Project</summary>
    [InitializeOnLoad]
    public static class AstarUpdateChecker
    {
#if UNITY_2018_1_OR_NEWER
        /// <summary>Used for downloading new version information</summary>
        private static UnityWebRequest updateCheckDownload;
#else
		/// <summary>Used for downloading new version information</summary>
		static WWW updateCheckDownload;
#endif

        private static DateTime _lastUpdateCheck;
        private static bool _lastUpdateCheckRead;

        private static Version _latestVersion;

        private static Version _latestBetaVersion;

        /// <summary>Description of the latest update of the A* Pathfinding Project</summary>
        private static string _latestVersionDescription;

        private static bool hasParsedServerMessage;

        /// <summary>Number of days between update checks</summary>
        private const double updateCheckRate = 1F;

        /// <summary>URL to the version file containing the latest version number.</summary>
        private const string updateURL = "http://www.arongranberg.com/astar/version.php";

        /// <summary>Last time an update check was made</summary>
        public static DateTime lastUpdateCheck
        {
            get
            {
                try
                {
                    // Reading from EditorPrefs is relatively slow, avoid it
                    if (_lastUpdateCheckRead) return _lastUpdateCheck;

                    _lastUpdateCheck =
                        DateTime.Parse(EditorPrefs.GetString("AstarLastUpdateCheck", "1/1/1971 00:00:01"),
                            CultureInfo.InvariantCulture);
                    _lastUpdateCheckRead = true;
                }
                catch (FormatException)
                {
                    lastUpdateCheck = DateTime.UtcNow;
                    Debug.LogWarning("Invalid DateTime string encountered when loading from preferences");
                }

                return _lastUpdateCheck;
            }
            private set
            {
                _lastUpdateCheck = value;
                EditorPrefs.SetString("AstarLastUpdateCheck", _lastUpdateCheck.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>Latest version of the A* Pathfinding Project</summary>
        public static Version latestVersion
        {
            get
            {
                RefreshServerMessage();
                return _latestVersion ?? AstarPath.Version;
            }
            private set => _latestVersion = value;
        }

        /// <summary>Latest beta version of the A* Pathfinding Project</summary>
        public static Version latestBetaVersion
        {
            get
            {
                RefreshServerMessage();
                return _latestBetaVersion ?? AstarPath.Version;
            }
            private set => _latestBetaVersion = value;
        }

        /// <summary>Summary of the latest update</summary>
        public static string latestVersionDescription
        {
            get
            {
                RefreshServerMessage();
                return _latestVersionDescription ?? "";
            }
            private set => _latestVersionDescription = value;
        }

        /// <summary>
        ///     Holds various URLs and text for the editor.
        ///     This info can be updated when a check for new versions is done to ensure that there are no invalid links.
        /// </summary>
        private static readonly Dictionary<string, string> astarServerData = new()
        {
            { "URL:modifiers", "http://www.arongranberg.com/astar/docs/modifiers.php" },
            { "URL:astarpro", "http://arongranberg.com/unity/a-pathfinding/astarpro/" },
            { "URL:documentation", "http://arongranberg.com/astar/docs/" },
            { "URL:findoutmore", "http://arongranberg.com/astar" },
            { "URL:download", "http://arongranberg.com/unity/a-pathfinding/download" },
            { "URL:changelog", "http://arongranberg.com/astar/docs/changelog.php" },
            { "URL:tags", "http://arongranberg.com/astar/docs/tags.php" },
            { "URL:homepage", "http://arongranberg.com/astar/" }
        };

        static AstarUpdateChecker()
        {
            // Add a callback so that we can parse the message when it has been downloaded
            EditorApplication.update += UpdateCheckLoop;
            EditorBase.getDocumentationURL = () => GetURL("documentation");
        }


        private static void RefreshServerMessage()
        {
            if (!hasParsedServerMessage)
            {
                var serverMessage = EditorPrefs.GetString("AstarServerMessage");

                if (!string.IsNullOrEmpty(serverMessage))
                {
                    ParseServerMessage(serverMessage);
                    ShowUpdateWindowIfRelevant();
                }
            }
        }

        public static string GetURL(string tag)
        {
            RefreshServerMessage();
            string url;
            astarServerData.TryGetValue("URL:" + tag, out url);
            return url ?? "";
        }

        /// <summary>Initiate a check for updates now, regardless of when the last check was done</summary>
        public static void CheckForUpdatesNow()
        {
            lastUpdateCheck = DateTime.UtcNow.AddDays(-5);

            // Remove the callback if it already exists
            EditorApplication.update -= UpdateCheckLoop;

            // Add a callback so that we can parse the message when it has been downloaded
            EditorApplication.update += UpdateCheckLoop;
        }

        /// <summary>
        ///     Checking for updates...
        ///     Should be called from EditorApplication.update
        /// </summary>
        private static void UpdateCheckLoop()
        {
            // Go on until the update check has been completed
            if (!CheckForUpdates()) EditorApplication.update -= UpdateCheckLoop;
        }

        /// <summary>
        ///     Checks for updates if there was some time since last check.
        ///     It must be called repeatedly to ensure that the result is processed.
        ///     Returns: True if an update check is progressing (WWW request)
        /// </summary>
        private static bool CheckForUpdates()
        {
            if (updateCheckDownload != null && updateCheckDownload.isDone)
            {
                if (!string.IsNullOrEmpty(updateCheckDownload.error))
                {
                    Debug.LogWarning("There was an error checking for updates to the A* Pathfinding Project\n" +
                                     "The error might disappear if you switch build target from Webplayer to Standalone because of the webplayer security emulation\nError: " +
                                     updateCheckDownload.error);
                    updateCheckDownload = null;
                    return false;
                }
#if UNITY_2018_1_OR_NEWER
                UpdateCheckCompleted(updateCheckDownload.downloadHandler.text);
                updateCheckDownload.Dispose();
#else
				UpdateCheckCompleted(updateCheckDownload.text);
#endif
                updateCheckDownload = null;
            }

            // Check if it is time to check for updates
            // Check for updates a bit earlier if we are in play mode or have the AstarPath object in the scene
            // as then the collected statistics will be a bit more accurate
            var offsetMinutes = (Application.isPlaying && Time.time > 60) || AstarPath.active != null ? -20 : 20;
            var minutesUntilUpdate = lastUpdateCheck.AddDays(updateCheckRate).AddMinutes(offsetMinutes)
                .Subtract(DateTime.UtcNow).TotalMinutes;
            if (minutesUntilUpdate < 0) DownloadVersionInfo();

            return updateCheckDownload != null || minutesUntilUpdate < 10;
        }

        private static void DownloadVersionInfo()
        {
            var script = AstarPath.active != null
                ? AstarPath.active
                : GameObject.FindObjectOfType(typeof(AstarPath)) as AstarPath;

            if (script != null)
            {
                script.ConfigureReferencesInternal();
                if ((!Application.isPlaying && (script.data.graphs == null || script.data.graphs.Length == 0)) ||
                    script.data.graphs == null) script.data.DeserializeGraphs();
            }

            var mecanim = GameObject.FindObjectOfType(typeof(Animator)) != null;
            var query = updateURL +
                        "?v=" + AstarPath.Version +
                        "&pro=0" +
                        "&check=" + updateCheckRate + "&distr=" + AstarPath.Distribution +
                        "&unitypro=" + (Application.HasProLicense() ? "1" : "0") +
                        "&inscene=" + (script != null ? "1" : "0") +
                        "&targetplatform=" + EditorUserBuildSettings.activeBuildTarget +
                        "&devplatform=" + Application.platform +
                        "&mecanim=" + (mecanim ? "1" : "0") +
                        "&hasNavmesh=" +
                        (script != null && script.data.graphs.Any(g => g.GetType().Name == "NavMeshGraph") ? 1 : 0) +
                        "&hasPoint=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "PointGraph")
                            ? 1
                            : 0) +
                        "&hasGrid=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "GridGraph")
                            ? 1
                            : 0) +
                        "&hasLayered=" +
                        (script != null && script.data.graphs.Any(g => g.GetType().Name == "LayerGridGraph") ? 1 : 0) +
                        "&hasRecast=" +
                        (script != null && script.data.graphs.Any(g => g.GetType().Name == "RecastGraph") ? 1 : 0) +
                        "&hasGrid=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "GridGraph")
                            ? 1
                            : 0) +
                        "&hasCustom=" + (script != null && script.data.graphs.Any(g =>
                            g != null && !g.GetType().FullName.Contains("Pathfinding."))
                            ? 1
                            : 0) +
                        "&graphCount=" + (script != null ? script.data.graphs.Count(g => g != null) : 0) +
                        "&unityversion=" + Application.unityVersion +
                        "&branch=" + AstarPath.Branch;

#if UNITY_2018_1_OR_NEWER
            updateCheckDownload = UnityWebRequest.Get(query);
            updateCheckDownload.SendWebRequest();
#else
			updateCheckDownload = new WWW(query);
#endif
            lastUpdateCheck = DateTime.UtcNow;
        }

        /// <summary>Handles the data from the update page</summary>
        private static void UpdateCheckCompleted(string result)
        {
            EditorPrefs.SetString("AstarServerMessage", result);
            ParseServerMessage(result);
            ShowUpdateWindowIfRelevant();
        }

        private static void ParseServerMessage(string result)
        {
            if (string.IsNullOrEmpty(result)) return;

            hasParsedServerMessage = true;

#if ASTARDEBUG
			Debug.Log("Result from update check:\n"+result);
#endif

            var splits = result.Split('|');
            latestVersionDescription = splits.Length > 1 ? splits[1] : "";

            if (splits.Length > 4)
            {
                // First 4 are just compatibility fields
                var fields = splits.Skip(4).ToArray();

                // Take all pairs of fields
                for (var i = 0; i < fields.Length / 2 * 2; i += 2)
                {
                    var key = fields[i];
                    var val = fields[i + 1];
                    astarServerData[key] = val;
                }
            }

            try
            {
                latestVersion = new Version(astarServerData["VERSION:branch"]);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not parse version\n" + ex);
            }

            try
            {
                latestBetaVersion = new Version(astarServerData["VERSION:beta"]);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Could not parse version\n" + ex);
            }
        }

        private static void ShowUpdateWindowIfRelevant()
        {
#if !ASTAR_ATAVISM
            try
            {
                DateTime remindDate;
                var remindVersion = new Version(EditorPrefs.GetString("AstarRemindUpdateVersion", "0.0.0.0"));
                if (latestVersion == remindVersion &&
                    DateTime.TryParse(EditorPrefs.GetString("AstarRemindUpdateDate", "1/1/1971 00:00:01"),
                        out remindDate))
                {
                    if (DateTime.UtcNow < remindDate)
                        // Don't remind yet
                        return;
                }
                else
                {
                    EditorPrefs.DeleteKey("AstarRemindUpdateDate");
                    EditorPrefs.DeleteKey("AstarRemindUpdateVersion");
                }
            }
            catch
            {
                Debug.LogError("Invalid AstarRemindUpdateVersion or AstarRemindUpdateDate");
            }

            var skipVersion = new Version(EditorPrefs.GetString("AstarSkipUpToVersion", AstarPath.Version.ToString()));

            if (AstarPathEditor.FullyDefinedVersion(latestVersion) !=
                AstarPathEditor.FullyDefinedVersion(skipVersion) && AstarPathEditor.FullyDefinedVersion(latestVersion) >
                AstarPathEditor.FullyDefinedVersion(AstarPath.Version))
            {
                EditorPrefs.DeleteKey("AstarSkipUpToVersion");
                EditorPrefs.DeleteKey("AstarRemindUpdateDate");
                EditorPrefs.DeleteKey("AstarRemindUpdateVersion");

                AstarUpdateWindow.Init(latestVersion, latestVersionDescription);
            }
#endif
        }
    }
}