using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(ItemManager))]
    internal static class InventorySlotCountPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Awake_Postfix(ItemManager __instance)
        {
            ApplySlotCount(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateTargetInventorySize")]
        private static void UpdateTargetInventorySize_Prefix(ItemManager __instance)
        {
            ApplySlotCount(__instance);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void Update_Postfix(ItemManager __instance)
        {
            int targetSize = PatchedAndLatchedPlugin.InventorySlotCount!.Value;
            targetSize = Mathf.Clamp(targetSize, 1, 9);
            int targetMaxItem = targetSize - 1;

            if (__instance.maxItem != targetMaxItem)
            {
                __instance.maxItem = targetMaxItem;
                Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber).UpdateInventorySize(targetSize);

                for (int i = 0; i <= __instance.maxItem; i++)
                {
                    Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber).UpdateItemIcon(i, __instance.items[i].itemSpriteSmall);
                }
            }
        }

        private static void ApplySlotCount(ItemManager __instance)
        {
            int targetSize = PatchedAndLatchedPlugin.InventorySlotCount!.Value;
            targetSize = Mathf.Clamp(targetSize, 1, 9);
            int targetMaxItem = targetSize - 1;

            if (__instance.maxItem != targetMaxItem || __instance.defaultInventorySize != targetSize)
            {
                __instance.maxItem = targetMaxItem;
                __instance.defaultInventorySize = targetSize;
                Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber).UpdateInventorySize(targetSize);
                for (int i = 0; i <= __instance.maxItem; i++)
                {
                    Singleton<CoreGameManager>.Instance.GetHud(__instance.pm.playerNumber).UpdateItemIcon(i, __instance.items[i].itemSpriteSmall);
                }

                if (__instance.selectedItem > __instance.maxItem)
                    __instance.selectedItem = __instance.maxItem;
                __instance.UpdateSelect();
            }
        }
    }
}
