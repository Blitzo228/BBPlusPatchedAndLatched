using HarmonyLib;
using PatchedAndLatched;
using SmallChanges.Patches;
using System.Collections.Generic;
using System.Reflection;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(FieldTripBaseRoomFunction))]
    internal static class CampingItemLimitPatch
    {
        private static FieldInfo _itemLimitField;
        private static FieldInfo _itemsCollectedField;
        private static FieldInfo _pickupsToDisableField;

        static CampingItemLimitPatch()
        {
            _itemLimitField = AccessTools.Field(typeof(FieldTripBaseRoomFunction), "itemLimit");
            _itemsCollectedField = AccessTools.Field(typeof(FieldTripBaseRoomFunction), "itemsCollected");
            _pickupsToDisableField = AccessTools.Field(typeof(FieldTripBaseRoomFunction), "pickupsToDisable");
        }

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OverrideItemLimitInitialize(FieldTripBaseRoomFunction __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableCampingItemLimit.Value) return;
            if (_itemLimitField == null) return;
            if (FieldTripNametagState.UsedNametag) return;

            _itemLimitField.SetValue(__instance, PatchedAndLatchedPlugin.CampingItemPickupLimit.Value);
        }

        [HarmonyPatch("StartMinigame")]
        [HarmonyPostfix]
        private static void OverrideItemLimitStartMinigame(FieldTripBaseRoomFunction __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableCampingItemLimit.Value) return;
            if (_itemLimitField == null) return;
            if (FieldTripNametagState.UsedNametag) return;

            _itemLimitField.SetValue(__instance, PatchedAndLatchedPlugin.CampingItemPickupLimit.Value);
        }

        [HarmonyPatch("ItemCollected")]
        [HarmonyPrefix]
        private static bool PrefixItemCollected(FieldTripBaseRoomFunction __instance, Pickup pickup, int player)
        {
            if (!PatchedAndLatchedPlugin.EnableCampingItemLimit.Value) return true;
            if (FieldTripNametagState.UsedNametag) return true;

            if (_itemsCollectedField != null)
            {
                int current = (int)_itemsCollectedField.GetValue(__instance);
                _itemsCollectedField.SetValue(__instance, current + 1);
            }

            if (_pickupsToDisableField != null)
            {
                var list = _pickupsToDisableField.GetValue(__instance) as List<Pickup>;
                if (list != null && list.Contains(pickup))
                {
                    list.Remove(pickup);
                }
            }
            return false;
        }
    }
}