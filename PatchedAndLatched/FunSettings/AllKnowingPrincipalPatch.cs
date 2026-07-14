using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Principal))]
    internal static class AllKnowingPrincipalPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void MakeAllKnowing(Principal __instance)
        {
            if (!PatchedAndLatchedPlugin.AllKnowingPrincipalEnabled.Value) return;

            Traverse.Create(__instance).Field("allKnowing").SetValue(true);

            PlayerManager player = CoreGameManager.Instance.GetPlayer(0);
            if (player == null) return;

            Traverse.Create(__instance).Field("targetedPlayer").SetValue(player);

            __instance.behaviorStateMachine.ChangeState(new Principal_ChasingPlayer_AllKnowing(__instance, player));

            __instance.Scold("AfterHours");

            __instance.ec.map.AddArrow(__instance.Navigator.Entity, Color.blue);
        }
    }
}