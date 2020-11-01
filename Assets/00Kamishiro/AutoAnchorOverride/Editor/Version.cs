/*
 * Copyright (c) 2020 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 *
 * This program uses the following code, which is provided under the MIT License
 * https://github.com/synqark/Arktoon-Shaders
 * 
 */

using System.CodeDom;

namespace Kamishiro.UnityEditor.AutoAnchorOverride
{
    public class Version : MonoBehaviour
    {
        public static int versionInt;
        private const string version = "v1.00";
        private const string url = "https://api.github.com/repos/AoiKamishiro/UnityCustomEditor_AutoAnchorOverride/releases/latest";
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
            www = UnityWebRequest.Get(url);

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
            EditorGUILayout.LabelField(AKSStyles.localVer + EditorUserSettings.GetConfigValue(localver));
            EditorGUILayout.LabelField(AKSStyles.remoteVer + EditorUserSettings.GetConfigValue(CodeRemoveEventStatement));
            if (bool.TryParse(EditorUserSettings.GetConfigValue(needUpdate), out bool needupdate) && needupdate)
            {
                //EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(AKSStyles.btnUpdate)) { AKSUIHelper.OpenLink(AKSStyles.linkRelease); }
                if (GUILayout.Button(AKSStyles.btnBooth)) { AKSUIHelper.OpenLink(AKSStyles.linkBooth); }
                //EditorGUILayout.EndHorizontal();
            }
        }

        public class GitJson
        {
            public string tag_name;
        }
    }
}