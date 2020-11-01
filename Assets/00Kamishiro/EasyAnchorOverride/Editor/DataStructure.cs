/*
 * Copyright (c) 2020 AoiKamishiro
 * 
 * This code is provided under the MIT license.
 *
 */

namespace Kamishiro.UnityEditor.EasyAnchorOverride
{
    internal static class Translate
    {
        private const string ContinueEN = "Continue";
        private const string ContinueJP = "続行";
        private const string CancelEN = "Cancel";
        private const string CancelJP = "中止";
        private const string updateAvatarJP = "Update Avatar を押してアバター一覧を更新します。";
        private const string UpdateAvatarEN = "Press Update Avatar to update your avatar list.";
        private const string AnchorNoneJP = "AnchorOverride が None になっている項目はスキップされます。";
        private const string AnchorNoneEN = "Items with AnchorOverride set to None will be skipped.";
        private const string NoDescriptorJP = "VRC_AvatarDesctipterが見つかりませんでした。";
        private const string NoDesctiptorEN = "VRC_AvatarDesctipter was not found.";
        private const string ModAcceptJP = "シーンを保存し、以下のアバターのAnchorOverrideの設定を変更します。宜しいですか？";
        private const string ModscceptEN = "Save the scene and change the AnchorOverride settings for your avatar below. Is that alright?";
        private const string OverrideErrorJP = "AnchorOverrideがアバターの子にありません。アバター内のオブジェクトを指定してください。";
        private const string OverrideErrorEN = "AnchorOverride is not in my avatar's children. Please specify an object in your avatar.";
        private const string OperationFinEn = "Th process is completed.";
        private const string OperationFinJP = "処理が完了しました。";

        public static string UpdateAvatar()
        {
            return Langs.current == Langs.Language.English ? UpdateAvatarEN : updateAvatarJP;
        }
        public static string AnchorNone()
        {
            return Langs.current == Langs.Language.English ? AnchorNoneEN : AnchorNoneJP;
        }
        public static string NoDesctiptor()
        {
            return Langs.current == Langs.Language.English ? NoDesctiptorEN : NoDescriptorJP;
        }
        public static string ModAccept()
        {
            return Langs.current == Langs.Language.English ? ModscceptEN : ModAcceptJP;
        }
        public static string OverrideError()
        {
            return Langs.current == Langs.Language.English ? OverrideErrorEN : OverrideErrorJP;
        }
        public static string OperationFin()
        {
            return Langs.current == Langs.Language.English ? OperationFinEn : OperationFinJP;
        }
        public static string Continue()
        {
            return Langs.current == Langs.Language.English ? ContinueEN : ContinueJP;
        }
        public static string Cancel()
        {
            return Langs.current == Langs.Language.English ? CancelEN : CancelJP;
        }
    }
    internal static class URL
    {
        public const string GIUHUB_REPOS = "https://github.com/AoiKamishiro/UnityCustomEditor_EasyAnchorOverride";
        public const string GITHUB_RELEASE = "https://github.com/AoiKamishiro/UnityCustomEditor_EasyAnchorOverride/releases";
        public const string GITHUB_VERCHECK = "https://api.github.com/repos/AoiKamishiro/UnityCustomEditor_EasyAnchorOverride/releases/latest";
        public const string BOOTH_PAGE = "https://kamishirolab.booth.pm/items/2494327";
    }
    static class UIText
    {
        //public const string 
    }
}