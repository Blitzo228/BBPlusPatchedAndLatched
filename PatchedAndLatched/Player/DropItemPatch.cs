using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(ItemManager))]
    internal static class DropItemPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool Prefix(ItemManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableDropItem.Value) return true;

            if (Input.GetKeyDown(KeyCode.R))
            {
                ItemObject item = __instance.items[__instance.selectedItem];
                if (item != __instance.nothing)
                {
                    PlayerManager pm = __instance.pm;
                    Cell cell = pm.ec.CellFromPosition(pm.transform.position);
                    pm.ec.CreateItem(cell.room, item, new Vector2(pm.transform.position.x, pm.transform.position.z));
                    __instance.RemoveItem(__instance.selectedItem);
                }
            }
            return true;
        }
    }
}