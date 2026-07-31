using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(NPC))]
    internal static class LethalTouchPatch
    {
        [HarmonyPatch("VirtualOnTriggerEnter")]
        [HarmonyPostfix]
        private static void VirtualOnTriggerEnterPostfix(NPC __instance, Entity otherEntity, Collider other)
        {
            CheckLethalTouch(__instance, otherEntity, other);
        }

        [HarmonyPatch("VirtualOnTriggerStay")]
        [HarmonyPostfix]
        private static void VirtualOnTriggerStayPostfix(NPC __instance, Entity otherEntity, Collider other)
        {
            CheckLethalTouch(__instance, otherEntity, other);
        }

        private static void CheckLethalTouch(NPC npc, Entity otherEntity, Collider other)
        {
            if (other.CompareTag("Player") || (otherEntity != null && otherEntity.CompareTag("Player")))
            {
                Baldi? baldi = npc.ec?.GetBaldi();
                if (baldi == null && npc is Baldi b)
                    baldi = b;

                if (baldi == null) return;

                var core = Singleton<CoreGameManager>.Instance;
                if (core != null)
                {
                    core.EndGame(other.transform, baldi);
                }
            }
        }
    }
}
