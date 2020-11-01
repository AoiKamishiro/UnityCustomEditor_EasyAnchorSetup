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
#if VRC_SDK_VRCSDK2
#endif
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif

namespace Kamishiro.UnityEditor.EasyAnchorSetup
{
    public class MainWindow : EditorWindow
    {
        [MenuItem("Tools/Kamishiro/EasyAnchorSetup", priority = 150)]
        private static void OnEnable()
        {
            MainWindow window = GetWindow<MainWindow>("EasyAnchorSetup");
            window.minSize = new Vector2(400, 360);
            window.Show();
        }

        private GameObject[] avatars = new GameObject[] { };
        private Transform[] anchors = new Transform[] { };
        private bool isFirst = true;

        private void OnGUI()
        {
            if (isFirst)
            {
                UpdateList();
                isFirst = false;
            }

            UIHelper.ShurikenHeader("EasyAnchorSetup");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Language");
            Langs.current = (Langs.Language)EditorGUILayout.EnumPopup(Langs.current);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(Translate.UpdateAvatar(), MessageType.Info);
            if (GUILayout.Button("Update Avtars"))
            {
                UpdateList();
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
                        DrawAvatarList(i);
                    }
                    catch
                    {
                        UpdateList();
                        DrawAvatarList(i);
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
                    ConfirmDialog();
                }
            }
            else
            {
                EditorGUILayout.HelpBox(Translate.NoDesctiptor(), MessageType.Info);
            }
            UIHelper.ShurikenHeader("About");
            EditorGUILayout.LabelField("Author: AoiKamishiro / 神城アオイ", EditorStyles.boldLabel);

            if (GUILayout.Button("Readme"))
            {
                UIHelper.OpenLink(URL.GIUHUB_REPOS);
            }
            Version.DisplayVersion();
        }
        private void DrawAvatarList(int i)
        {
            EditorGUILayout.LabelField(avatars[i].name);
            anchors[i] = (Transform)EditorGUILayout.ObjectField(anchors[i], typeof(Transform), true);
        }
        private GameObject[] SortGameObjects(GameObject[] gameObjects)
        {
            List<GameObject> rev = gameObjects.ToList();
            rev.Sort((a, b) => (a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex()));
            return rev.ToArray();
        }
        private Transform[] GetCurrentAnchors()
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
                anchorOverrides[i] = ac.Length == 1 ? ac[0] : null;
            }
            return anchorOverrides;
        }
        private void UpdateAnchors()
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
        private void SaveScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                EditorSceneManager.SaveScene(SceneManager.GetSceneAt(i));
            }
        }
        private void ConfirmDialog()
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
                SaveScene();
                UpdateAnchors();
                SetSceneDirty();
                UpdateList();
                EditorUtility.DisplayDialog("Auto AnchorOverride", Translate.OperationFin(), "OK");
            }
        }
        private void UpdateList()
        {
            avatars = SortGameObjects(VRCSDK.GetAllAvatars());
            anchors = GetCurrentAnchors();
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