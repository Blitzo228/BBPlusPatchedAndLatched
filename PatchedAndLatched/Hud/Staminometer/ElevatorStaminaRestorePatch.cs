using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Elevator))]
    internal static class ElevatorStaminaRestorePatch
    {
        [HarmonyPatch("SetState")]
        [HarmonyPostfix]
        private static void OnElevatorSetState(Elevator __instance, ElevatorState state)
        {
            if (!PatchedAndLatchedPlugin.EnableElevatorStaminaRestore.Value) return;
            if (state != ElevatorState.OutOfOrder) return;
            for (int i = 0; i < Singleton<CoreGameManager>.Instance.setPlayers; i++)
            {
                var player = Singleton<CoreGameManager>.Instance.GetPlayer(i);
                if (player != null)
                {
                    player.plm.stamina = player.plm.StaminaMax;
                    Singleton<CoreGameManager>.Instance.GetHud(i).SetStaminaValue(1f);
                }
            }
        }
    }
}