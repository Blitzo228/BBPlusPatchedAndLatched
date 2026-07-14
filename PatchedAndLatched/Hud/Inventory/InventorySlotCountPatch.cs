using HarmonyLib;
using PatchedAndLatched;
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
            if (!PatchedAndLatchedPlugin.CustomInventorySlots.Value) return;

            int targetSize = PatchedAndLatchedPlugin.InventorySlotCount.Value;
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

            if (__instance.selectedItem > __instance.maxItem)
            {
                __instance.selectedItem = __instance.maxItem;
                __instance.UpdateSelect();
            }

            if (Input.GetKeyDown(KeyCode.Alpha6) && __instance.maxItem >= 5)
            {
                __instance.selectedItem = 5;
                __instance.UpdateSelect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7) && __instance.maxItem >= 6)
            {
                __instance.selectedItem = 6;
                __instance.UpdateSelect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8) && __instance.maxItem >= 7)
            {
                __instance.selectedItem = 7;
                __instance.UpdateSelect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9) && __instance.maxItem >= 8)
            {
                __instance.selectedItem = 8;
                __instance.UpdateSelect();
            }
        }

        private static void ApplySlotCount(ItemManager __instance)
        {
            if (!PatchedAndLatchedPlugin.CustomInventorySlots.Value) return;

            int targetSize = PatchedAndLatchedPlugin.InventorySlotCount.Value;
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
