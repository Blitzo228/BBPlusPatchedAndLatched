using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Baldi), "Praise")]
    internal static class BaldiQuarterRewardPatch
    {
        private static ItemObject? _quarterItem;

        static BaldiQuarterRewardPatch()
        {
            _quarterItem = Resources.Load<ItemObject>("Items/Quarter");
            if (_quarterItem == null)
                _quarterItem = Resources.Load<ItemObject>("Quarter");
            if (_quarterItem == null)
            {
                foreach (var item in Resources.FindObjectsOfTypeAll<ItemObject>())
                {
                    if (item.itemType == Items.Quarter)
                    {
                        _quarterItem = item;
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        private static void Postfix(Baldi __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableBaldiQuarterReward.Value) return;
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
