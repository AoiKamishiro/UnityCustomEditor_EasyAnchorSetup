/*
 * Copyright (c) 2021 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 *
 * This program uses the following code, which is provided under the MIT License
 * https://github.com/synqark/Arktoon-Shaders
 * 
 */

using UnityEditor;
using UnityEngine;

namespace Kamishiro.UnityEditor.EasyAnchorSetup
{
    public static class UIHelper
    {
        private static readonly int HEADER_HEIGHT = 22;
        private static Rect DrawShuriken(string title, Vector2 contentOffset)
        {
            GUIStyle style = new GUIStyle("ShurikenModuleTitle")
            {
                margin = new RectOffset(0, 0, 8, 0),
                font = new GUIStyle(EditorStyles.boldLabel).font,
                border = new RectOffset(15, 7, 4, 4),
                fixedHeight = HEADER_HEIGHT,
                contentOffset = contentOffset
            };
            Rect rect = GUILayoutUtility.GetRect(16f, HEADER_HEIGHT, style);
            GUI.Box(rect, title, style);
            return rect;
        }
        public static void ShurikenHeader(string title)
        {
            DrawShuriken(title, new Vector2(6f, -2f));
        }
        public static bool ShurikenFoldout(string title, bool display)
        {
            Rect rect = DrawShuriken(title, new Vector2(20f, -2f));
            Event e = Event.current;
            Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }
            return display;
        }
        public static void OpenLink(string link)
        {
            Application.OpenURL(link);
        }
    }
}