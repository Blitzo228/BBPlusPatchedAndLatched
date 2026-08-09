using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    internal static class BSODAReplacePatch
    {
        private static ItemObject? _regularBsoda;

        private static void FindRegularBsoda()
        {
            if (_regularBsoda != null) return;

            var allItems = Resources.FindObjectsOfTypeAll<ItemObject>();
            foreach (var item in allItems)
            {
                if (item.itemType == Items.Bsoda)
                {
                    _regularBsoda = item;
                    break;
                }
            }

            if (_regularBsoda == null && Singleton<PlayerFileManager>.Instance != null)
            {
                foreach (var item in Singleton<PlayerFileManager>.Instance.itemObjects)
                {
                    if (item.itemType == Items.Bsoda)
                    {
                        _regularBsoda = item;
                        break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ItemManager), "AddItem", typeof(ItemObject))]
        [HarmonyPrefix]
        private static void PrefixAddItem(ref ItemObject item)
        {
            if (item == null) return;
            if (item.itemType != Items.DietBsoda) return;

            FindRegularBsoda();
            if (_regularBsoda != null)
                item = _regularBsoda;
        }

        [HarmonyPatch(typeof(ItemManager), "AddItem", typeof(ItemObject), typeof(Pickup))]
        [HarmonyPrefix]
        private static void PrefixAddItemWithPickup(ref ItemObject item, Pickup pickup)
        {
            if (item == null) return;
            if (item.itemType != Items.DietBsoda) return;

            FindRegularBsoda();
            if (_regularBsoda != null)
                item = _regularBsoda;
        }

        [HarmonyPatch(typeof(LevelBuilder), "CreateItem", typeof(RoomController), typeof(ItemObject), typeof(Vector2), typeof(bool), typeof(bool))]
        [HarmonyPrefix]
        private static void PrefixCreateItem(ref ItemObject item)
        {
            if (item == null) return;
            if (item.itemType != Items.DietBsoda) return;

            FindRegularBsoda();
            if (_regularBsoda != null)
                item = _regularBsoda;
        }

        [HarmonyPatch(typeof(ItemManager), "SetItem")]
        [HarmonyPrefix]
        private static void PrefixSetItem(ref ItemObject item, int slot)
        {
            if (item == null) return;
            if (item.itemType != Items.DietBsoda) return;

            FindRegularBsoda();
            if (_regularBsoda != null)
                item = _regularBsoda;
        }
    }
}
