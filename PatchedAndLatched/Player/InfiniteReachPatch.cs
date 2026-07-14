using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(PlayerManager))]
    public static class InfiniteReachPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void Start_Postfix(PlayerManager __instance)
        {
            if (!PatchedAndLatchedPlugin.InfiniteReach.Value) return;

            foreach (var ext in __instance.pc.reachExtensions)
            {
                if (ext.distance == PatchedAndLatchedPlugin.ReachDistance.Value)
                    return;
            }

            ReachExtension newExt = new ReachExtension();
            newExt.distance = PatchedAndLatchedPlugin.ReachDistance.Value;
            newExt.overrideSquished = true;
            __instance.pc.reachExtensions.Add(newExt);
        }
    }
}