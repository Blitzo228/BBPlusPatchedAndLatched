using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(DrReflex_Testing))]
    internal static class DrReflexLeavePenaltyPatch
    {
        private static FieldInfo? _drReflexField;
        private static FieldInfo? _playerField;
        private static bool _penaltyApplied = false;

        static DrReflexLeavePenaltyPatch()
        {
            _drReflexField = AccessTools.Field(typeof(DrReflex_Testing), "drReflex");
            _playerField = AccessTools.Field(typeof(DrReflex_Testing), "player");
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static void Prefix(DrReflex_Testing __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableDrReflexLeavePenalty!.Value) return;
            if (_drReflexField == null || _playerField == null) return;

            var drReflex = _drReflexField.GetValue(__instance) as DrReflex;
            var player = _playerField.GetValue(__instance) as PlayerManager;
            if (drReflex == null || player == null) return;

            if (drReflex.PlayerLeft(player) && !_penaltyApplied)
            {
                _penaltyApplied = true;
                player.RuleBreak("Bullying", 5f, 0.8f);
            }
        }

        [HarmonyPatch("Enter")]
        [HarmonyPostfix]
        private static void EnterReset()
        {
            _penaltyApplied = false;
        }
    }
}