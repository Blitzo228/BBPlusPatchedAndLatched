using HarmonyLib;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(Principal), "ObservePlayer")]
    internal class PrincipalPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Principal __instance, PlayerManager player)
        {
            if (player.Disobeying && !player.Tagged && player.ruleBreak == "Running" && !IsInHallway(player))
            {
                return false;
            }
            return true;
        }

        private static bool IsInHallway(PlayerManager player)
        {
            Cell val = player.ec.CellFromPosition(((Component)player).transform.position);
            if (val == null || (Object)(object)val.room == (Object)null)
            {
                return false;
            }
            return (int)val.room.category == 1;
        }
    }
}
