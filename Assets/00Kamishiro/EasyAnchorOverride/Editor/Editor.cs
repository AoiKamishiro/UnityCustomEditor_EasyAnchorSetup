/*
 * Copyright (c) 2020 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 *
 */


using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRCSDK2;

namespace Kamishiro.UnityEditor.EasyAnchorOverride
{
    public class MainWindow : EditorWindow
    {
        [MenuItem("Tools/Kamishiro/EasyAnchorOverride", priority = 150)]
        private static void OnEnable()
        {
            MainWindow window = GetWindow<MainWindow>("EasyAnchorOverride");
            window.minSize = new Vector2(400, 360);
            window.Show();
        }

        private VRC_AvatarDescriptor[] avatars = new VRC_AvatarDescriptor[] { };
        private Transform[] anchors = new Transform[] { };
        private bool isFirst = true;

        private void OnGUI()
        {
            if (isFirst)
            {
                avatars = SortAvatars(FindAllAvatarsInScene());
                anchors = SetupAnchorOverrides();
                isFirst = false;
            }

            UIHelper.ShurikenHeader("Easy AnchorOverride");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Language");
            Langs.current = (Langs.Language)EditorGUILayout.EnumPopup(Langs.current);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(Translate.UpdateAvatar(), MessageType.Info);
            if (GUILayout.Button("Update Avtars"))
            {
                avatars = SortAvatars(FindAllAvatarsInScene());
                anchors = SetupAnchorOverrides();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Avatars", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("AnchorOverride", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            if (avatars.Length > 0)
            {
                for (int i = 0; i < avatars.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    try
                    {
                        DrawWindow(i);
                    }
                    catch
                    {
                        avatars = SortAvatars(FindAllAvatarsInScene());
                        anchors = SetupAnchorOverrides();
                        DrawWindow(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.HelpBox(Translate.AnchorNone(), MessageType.Info);
                bool result = CheckAnchor();
                bool btnresult;
                EditorGUI.BeginDisabledGroup(result);
                btnresult = GUILayout.Button("Setup Anchor Override");
                EditorGUI.EndDisabledGroup();
                if (btnresult)
                {
                    DisplayDialog();
                }
            }
            else
            {
                EditorGUILayout.HelpBox(Translate.NoDesctiptor(), MessageType.Info);
            }
            UIHelper.ShurikenHeader("About");
            EditorGUILayout.LabelField("Author: AoiKamishiro / 神代 アオイ", EditorStyles.boldLabel);

            if (GUILayout.Button("Readme"))
            {
                UIHelper.OpenLink(URL.GIUHUB_REPOS);
            }
            Version.DisplayVersion();
        }
        private void DrawWindow(int i)
        {
            EditorGUILayout.LabelField(avatars[i].name);
            anchors[i] = (Transform)EditorGUILayout.ObjectField(anchors[i], typeof(Transform), true);
        }
        private VRC_AvatarDescriptor[] FindAllAvatarsInScene()
        {
            VRC_AvatarDescriptor[] avatars = new VRC_AvatarDescriptor[] { };

            VRC_AvatarDescriptor[] objects = Resources.FindObjectsOfTypeAll(typeof(VRC_AvatarDescriptor)) as VRC_AvatarDescriptor[];

            foreach (VRC_AvatarDescriptor avatar in objects)
            {
                if (avatar.hideFlags != HideFlags.NotEditable && avatar.hideFlags != HideFlags.HideAndDontSave)
                {
                    string path = AssetDatabase.GetAssetOrScenePath(avatar);
                    bool isScene = path.Contains(".unity");
                    if (isScene)
                    {
                        avatars = avatars.Concat(new VRC_AvatarDescriptor[] { avatar }).ToArray();
                    }
                }
            }
            return avatars;
        }
        private VRC_AvatarDescriptor[] SortAvatars(VRC_AvatarDescriptor[] avatarDescriptors)
        {
            List<VRC_AvatarDescriptor> avatars = avatarDescriptors.ToList();
            avatars.Sort((a, b) => (a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex()));
            return avatars.ToArray();
        }
        private Transform[] SetupAnchorOverrides()
        {
            Transform[] anchorOverrides = new Transform[avatars.Length];

            for (int i = 0; i < avatars.Length; i++)
            {
                MeshRenderer[] mesh = avatars[i].GetComponentsInChildren<MeshRenderer>();
                SkinnedMeshRenderer[] skinMesh = avatars[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                Transform[] ac = new Transform[] { };

                if (mesh != null && mesh.Length > 0)
                {
                    for (int j = 0; j < mesh.Length; j++)
                    {
                        ac = ac.Concat(new Transform[] { mesh[j].probeAnchor }).ToArray();
                    }
                }
                if (skinMesh != null && skinMesh.Length > 0)
                {
                    for (int j = 0; j < skinMesh.Length; j++)
                    {
                        ac = ac.Concat(new Transform[] { skinMesh[j].probeAnchor }).ToArray();
                    }
                }

                ac = (ac.ToList().Distinct()).ToArray();

                if (ac.Length == 1)
                {
                    anchorOverrides[i] = ac[0];
                }
                else
                {
                    Transform anchorOverride = avatars[i].transform.Find("AnchorOverride");
                    if (anchorOverride != null)
                    {
                        anchorOverrides[i] = anchorOverride;
                    }
                    else
                    {
                        Transform head = avatars[i].transform.Find("Head");
                        if (head != null)
                        {
                            anchorOverrides[i] = head;
                        }
                    }
                }
            }
            return anchorOverrides;
        }
        private void SetAnchorOverrides()
        {
            for (int i = 0; i < avatars.Length; i++)
            {
                if (anchors[i] != null)
                {
                    MeshRenderer[] mesh = avatars[i].GetComponentsInChildren<MeshRenderer>();
                    SkinnedMeshRenderer[] skinMesh = avatars[i].GetComponentsInChildren<SkinnedMeshRenderer>();

                    if (mesh != null && mesh.Length > 0)
                    {
                        for (int j = 0; j < mesh.Length; j++)
                        {
                            mesh[j].probeAnchor = anchors[i];
                        }
                    }
                    if (skinMesh != null && skinMesh.Length > 0)
                    {
                        for (int j = 0; j < skinMesh.Length; j++)
                        {
                            skinMesh[j].probeAnchor = anchors[i];
                        }
                    }
                }
            }
        }
        private void SetSceneDirty()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetSceneAt(i));
            }
        }
        private void SceneSaver()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                EditorSceneManager.SaveScene(SceneManager.GetSceneAt(i));
            }
        }
        private void DisplayDialog()
        {
            string avtrs = "\n";
            for (int i = 0; i < avatars.Length; i++)
            {
                if (anchors[i] != null)
                {
                    avtrs += "\n" + avatars[i].name;
                }
            }
            bool result = EditorUtility.DisplayDialog("Auto AnchorOverride", Translate.ModAccept() + avtrs, Translate.Continue(), Translate.Cancel());
            if (result)
            {
                SceneSaver();
                SetAnchorOverrides();
                SetSceneDirty();

                avatars = SortAvatars(FindAllAvatarsInScene());
                anchors = SetupAnchorOverrides();
                EditorUtility.DisplayDialog("Auto AnchorOverride", Translate.OperationFin(), "OK");
            }
        }
        private bool CheckAnchor()
        {
            bool rev = false;
            for (int i = 0; i < avatars.Length; i++)
            {
                if (anchors[i] != null)
                {
                    if (!isInChildern(avatars[i].transform, anchors[i]))
                    {
                        rev = true;
                        EditorGUILayout.HelpBox(Translate.OverrideError() + "\n" + avatars[i].name, MessageType.Error);
                    }
                }
            }
            return rev;
        }
        private bool isInChildern(Transform parent, Transform child)
        {
            Transform[] tr = parent.GetComponentsInChildren<Transform>();
            return tr.Contains(child);
        }
    }
}