/*
 * Copyright (c) 2020 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 *
 */

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Networking;

namespace Kamishiro.UnityEditor.EasyAnchorOverride
{
    internal static class Langs
    {
        public static Language current = Language.Japanese;
        public enum Language
        {
            Japanese,
            English
        }
    }

    public class Version
    {
        public static int versionInt;
        private const string version = "v1.00";
        private static UnityWebRequest www;
        private const string localver = "akautoanchoroverride_version_local";
        private const string remotever = "akautoanchoroverride_version_remote";
        private const string needUpdate = "akautoanchoroverride_need_update";

        [DidReloadScripts(0)]
        private static void CheckVersion()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            int.TryParse(version.Substring(1), out int verint);
            versionInt = verint * 100;
            // Check Local Version
            string localVersion = EditorUserSettings.GetConfigValue(localver) ?? "";

            if (!localVersion.Equals(version))
            {
                // Update Materiams
                //ArktoonMigrator.Migrate();
            }
            // Set Local Version
            EditorUserSettings.SetConfigValue(localver, version);
            // Get Remote Version
            www = UnityWebRequest.Get(URL.GITHUB_VERCHECK);

#if UNITY_2017_OR_NEWER
            www.SendWebRequest();
#else
#pragma warning disable 0618
            www.Send();
#pragma warning restore 0618
#endif

            EditorApplication.update += EditorUpdate;
            EditorUserSettings.SetConfigValue(needUpdate, NeedUpdate().ToString());
        }


        private static void EditorUpdate()
        {
            while (!www.isDone) return;

#if UNITY_2017_OR_NEWER
                if (www.isNetworkError || www.isHttpError) {
                    Debug.Log(www.error);
                } else {
                    UpdateHandler(www.downloadHandler.text);
                }
#else
#pragma warning disable 0618
            if (www.isError)
            {
                Debug.Log(www.error);
            }
            else
            {
                UpdateHandler(www.downloadHandler.text);
            }
#pragma warning restore 0618
#endif

            EditorApplication.update -= EditorUpdate;
        }

        private static void UpdateHandler(string apiResult)
        {
            GitJson git = JsonUtility.FromJson<GitJson>(apiResult);
            string version = git.tag_name;
            EditorUserSettings.SetConfigValue(remotever, version);
        }

        private static bool NeedUpdate()
        {
            bool needUpdate = false;
            bool parseLocal = double.TryParse((EditorUserSettings.GetConfigValue(localver)).Substring(1), out double localVer);
            bool parseRemote = double.TryParse((EditorUserSettings.GetConfigValue(remotever)).Substring(1), out double remoteVer);
            if (parseLocal && parseRemote && (localVer < remoteVer))
            {
                needUpdate = true;
            }
            return needUpdate;
        }
        public static void DisplayVersion()
        {
            EditorGUILayout.LabelField("Local Version: " + EditorUserSettings.GetConfigValue(localver));
            EditorGUILayout.LabelField("Remote Version: " + EditorUserSettings.GetConfigValue(remotever));
            if (bool.TryParse(EditorUserSettings.GetConfigValue(needUpdate), out bool needupdate) && needupdate)
            {
                if (GUILayout.Button("GitHub Release"))
                {
                    UIHelper.OpenLink(URL.GITHUB_RELEASE);
                }
                if (GUILayout.Button("Booth Page"))
                {
                    UIHelper.OpenLink(URL.BOOTH_PAGE);
                }
            }
        }
        public class GitJson
        {
            public string tag_name;
        }
    }
}