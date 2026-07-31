using HarmonyLib;
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
            bool isDropPressed = PatchedAndLatchedPlugin.IsRewiredCompatInstalled
                ? InputManager.Instance.GetDigitalInput("DropItem", true)
                : Input.GetKeyDown(KeyCode.R);

            if (isDropPressed)
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
