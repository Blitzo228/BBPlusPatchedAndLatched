using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Baldi), "Praise")]
    internal static class BaldiQuarterRewardPatch
    {
        private static ItemObject? _quarterItem;

        static BaldiQuarterRewardPatch()
        {
            _quarterItem = Resources
                .FindObjectsOfTypeAll<ItemObject>()
                .FirstOrDefault(item => item.itemType == Items.Quarter);
        }

        [HarmonyPostfix]
        private static void Postfix(Baldi __instance)
        {
            if (_quarterItem == null) return;

            var ec = __instance.ec;
            if (ec == null) return;

            Vector3 pos = __instance.transform.position + Vector3.up * 5f;
            var cell = ec.CellFromPosition(pos);
            if (cell == null) return;

            var room = cell.room;
            if (room == null) return;

            Pickup pickup = ec.CreateItem(room, _quarterItem, new Vector2(pos.x, pos.z));
            if (pickup != null)
                pickup.Hide(false);
        }
    }
}
