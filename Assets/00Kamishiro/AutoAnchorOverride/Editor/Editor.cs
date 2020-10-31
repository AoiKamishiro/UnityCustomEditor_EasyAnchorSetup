using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRCSDK2;

namespace Kamishiro.UnityEditor.AutoAnchorOverride
{
    public class MainWindow : EditorWindow
    {
        [MenuItem("Tools/Kamishiro/AutoAnchorOverride", priority = 150)]
        private static void OnEnable()
        {
            MainWindow window = GetWindow<MainWindow>("AutoAnchorOverride");
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
            EditorGUILayout.HelpBox("Update Avatar を押してアバター一覧を更新します。", MessageType.Info);
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

                EditorGUILayout.HelpBox("AnchorOverride が None になっている項目はスキップされます。", MessageType.Info);
                bool result = CheckAnchor();
                bool btnresult = false;
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
                EditorGUILayout.HelpBox("VRC_AvatarDesctipterが見つかりませんでした。", MessageType.Info);
            }
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
            bool result = EditorUtility.DisplayDialog("Auto AnchorOverride", "シーンを保存し、以下のアバターのAnchorOverrideの設定を変更します。宜しいですか？" + avtrs, "続行", "中止");
            if (result)
            {
                SceneSaver();
                SetAnchorOverrides();
                SetSceneDirty();

                avatars = SortAvatars(FindAllAvatarsInScene());
                anchors = SetupAnchorOverrides();
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
                        EditorGUILayout.HelpBox("AnchorOverrideがアバターの子にありません。アバター内のオブジェクトを指定してください。\n" + avatars[i].name, MessageType.Error);
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