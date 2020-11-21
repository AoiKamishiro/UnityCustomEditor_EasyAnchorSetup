/*
 * Copyright (c) 2020 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 *
 */
 
#if VRC_SDK_VRCSDK2
using VRCSDK2;
#endif
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Kamishiro.UnityEditor.EasyAnchorSetup
{
    public static class VRCSDK
    {
#if VRC_SDK_VRCSDK2
        public static GameObject[] GetAllAvatars()
        {
            GameObject[] rev = new GameObject[] { };

            VRC_AvatarDescriptor[] avatarDescriptors = Resources.FindObjectsOfTypeAll(typeof(VRC_AvatarDescriptor)) as VRC_AvatarDescriptor[];

            foreach (VRC_AvatarDescriptor a in avatarDescriptors)
            {
                if (a.hideFlags != HideFlags.NotEditable && a.hideFlags != HideFlags.HideAndDontSave)
                {
                    string path = AssetDatabase.GetAssetOrScenePath(a);
                    bool isScene = path.Contains(".unity");
                    if (isScene)
                    {
                        rev = rev.Concat(new GameObject[] { a.gameObject }).ToArray();
                    }
                }
            }
            return rev;
        }
#endif
#if VRC_SDK_VRCSDK3
        public static GameObject[] GetAllAvatars()
        {
            GameObject[] rev = new GameObject[] { };

            VRCAvatarDescriptor[] avatarDescriptors = Resources.FindObjectsOfTypeAll(typeof(VRCAvatarDescriptor)) as VRCAvatarDescriptor[];

            foreach (VRCAvatarDescriptor a in avatarDescriptors)
            {
                if (a.hideFlags != HideFlags.NotEditable && a.hideFlags != HideFlags.HideAndDontSave)
                {
                    string path = AssetDatabase.GetAssetOrScenePath(a);
                    bool isScene = path.Contains(".unity");
                    if (isScene)
                    {
                        rev = rev.Concat(new GameObject[] { a.gameObject }).ToArray();
                    }
                }
            }
            return rev;
        }
#endif

    }
}