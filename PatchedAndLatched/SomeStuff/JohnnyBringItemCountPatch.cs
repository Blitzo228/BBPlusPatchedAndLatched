using HarmonyLib;
using PatchedAndLatched;
using System.Collections.Generic;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(PitstopGameManager))]
    internal static class JohnnyBringItemCountPatch
    {
        [HarmonyPatch("SendJohnny")]
        [HarmonyPostfix]
        private static void IncreaseBringCount(PitstopGameManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableJohnnyBringCount.Value) return;

            int bringCount = PatchedAndLatchedPlugin.JohnnyBringItemCount.Value;
            if (bringCount <= 0) return;

            List<Pickup> pickups = __instance.fieldTripPickups;
            if (pickups == null) return;

            for (int i = 0; i < pickups.Count && i < bringCount; i++)
            {
                if (pickups[i] != null)
                    pickups[i].gameObject.SetActive(true);
            }
        }
    }
}