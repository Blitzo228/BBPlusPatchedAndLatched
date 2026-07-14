using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(ITM_BSODA))]
    public static class BSODABreakFirstPrizePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("EntityTriggerEnter")]
        public static void EntityTriggerEnter_Postfix(ITM_BSODA __instance, Entity otherEntity, Collider other, bool validCollision)
        {
            if (!validCollision) return;
            if (!PatchedAndLatchedPlugin.FirstPrizeBreakByBSODA.Value) return;

            FirstPrize? firstPrize = otherEntity?.GetComponent<FirstPrize>();
            if (firstPrize != null)
            {
                firstPrize.CutWires();
            }
        }
    }
}