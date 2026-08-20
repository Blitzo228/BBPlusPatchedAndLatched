using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(ItemManager), "UpdateSelect")]
    internal static class ItemNameSuffixPatch
    {
        private static FieldInfo? _itemTitleField;

        static ItemNameSuffixPatch()
        {
            _itemTitleField = AccessTools.Field(typeof(HudManager), "itemTitle");
        }

        [HarmonyPostfix]
        private static void AddSuffix(ItemManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableItemNameSuffix!.Value) return;

            if (_itemTitleField == null) return;
            var hud = Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber);
            if (hud == null) return;
            var titleText = _itemTitleField.GetValue(hud) as TMP_Text;
            if (titleText == null) return;

            if (!titleText.text.EndsWith(":3"))
                titleText.text += ":3";
        }
    }
}
