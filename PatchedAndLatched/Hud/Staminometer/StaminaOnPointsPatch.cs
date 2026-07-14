using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(CoreGameManager))]
    public static class StaminaOnPointsPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("AddPoints", new[] { typeof(int), typeof(int), typeof(bool), typeof(bool), typeof(bool) })]
        public static void AddPoints_Postfix(int points, int player, bool playAnimation, bool includeInLevelTotal, bool multiply)
        {
            if (!PatchedAndLatchedPlugin.StaminaOnPoints.Value) return;
            if (points <= 0) return;

            PlayerManager pm = Singleton<CoreGameManager>.Instance.GetPlayer(player);
            if (pm == null) return;

            PlayerMovement plm = pm.plm;
            if (plm == null) return;

            float maxStamina = plm.StaminaMax;

            float staminaToAdd = (points / 100f) * maxStamina;

            plm.AddStamina(staminaToAdd, true);
        }
    }
}
