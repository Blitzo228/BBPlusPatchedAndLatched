using HarmonyLib;
using PatchedAndLatched;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SmallChanges.Patches
{
    public static class FieldTripNametagState
    {
        public static bool UsedNametag = false;
    }

    [HarmonyPatch(typeof(FieldTripEntranceRoomFunction), nameof(FieldTripEntranceRoomFunction.StartFieldTrip))]
    public static class Patch_StartFieldTrip
    {
        static bool Prefix(FieldTripEntranceRoomFunction __instance, PlayerManager player, ref bool ___unlocked)
        {
            if (!PatchedAndLatchedPlugin.NametagForFieldTrip.Value) return true;

            if (player.itm.Has(Items.BusPass) || ___unlocked)
            {
                return true;
            }

            if (player.itm.Has(Items.Nametag))
            {
                player.itm.Remove(Items.Nametag);
                Singleton<BaseGameManager>.Instance.CallSpecialManagerFunction(1, __instance.gameObject);
                ___unlocked = true;
                FieldTripNametagState.UsedNametag = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(FieldTripBaseRoomFunction), nameof(FieldTripBaseRoomFunction.StartMinigame))]
    public static class Patch_FieldTripStartMinigame
    {
        static void Prefix(FieldTripBaseRoomFunction __instance, ref float ___itemLimit, List<Pickup> ___pickups)
        {
            if (!PatchedAndLatchedPlugin.NametagForFieldTrip.Value) return;
            if (!FieldTripNametagState.UsedNametag) return;

            ___itemLimit = 1f;

            HashSet<Items> weakTypes = new HashSet<Items>
            {
                Items.DoorLock,
                Items.AlarmClock,
                Items.Quarter,
                Items.Wd40,
                Items.Scissors,
                Items.Points
            };

            List<ItemObject> weakItemObjects = new List<ItemObject>();
            ItemObject[] allItems = Resources.FindObjectsOfTypeAll<ItemObject>();

            foreach (var itemObj in allItems)
            {
                if (weakTypes.Contains(itemObj.itemType))
                {
                    if (itemObj.itemType == Items.Points)
                    {
                        if (itemObj.item != null)
                        {
                            var ytpsComp = itemObj.item.GetComponent<ITM_YTPs>();
                            if (ytpsComp != null)
                            {
                                FieldInfo valueField = typeof(ITM_YTPs).GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
                                if (valueField != null && (int)valueField.GetValue(ytpsComp) == 25)
                                {
                                    weakItemObjects.Add(itemObj);
                                }
                            }
                        }
                    }
                    else
                    {
                        weakItemObjects.Add(itemObj);
                    }
                }
            }

            if (weakItemObjects.Count > 0)
            {
                foreach (Pickup pickup in ___pickups)
                {
                    ItemObject randomWeakItem = weakItemObjects[Random.Range(0, weakItemObjects.Count)];
                    pickup.AssignItem(randomWeakItem);
                }
            }
        }
    }

    [HarmonyPatch(typeof(FieldTripBaseRoomFunction), nameof(FieldTripBaseRoomFunction.EndMinigame))]
    public static class Patch_EndMinigame
    {
        static void Postfix(bool finished)
        {
            if (!PatchedAndLatchedPlugin.NametagForFieldTrip.Value) return;

            if (finished)
            {
                FieldTripNametagState.UsedNametag = false;
            }
        }
    }
}
