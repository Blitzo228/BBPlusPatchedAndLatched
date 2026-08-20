using HarmonyLib;
using System.Reflection;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    internal static class ScissorsBullyingPatch
    {
        private static bool _cutGum;
        private static bool _cutFirstPrize;
        private static bool _cutBaldi;
        private static int _jumpropesBefore;
        private static FieldInfo _audSnipField;

        static ScissorsBullyingPatch()
        {
            _audSnipField = AccessTools.Field(typeof(ITM_Scissors), "audSnip");
        }

        [HarmonyPatch(typeof(Gum), "Cut")]
        [HarmonyPrefix]
        private static void OnGumCut() => _cutGum = true;

        [HarmonyPatch(typeof(FirstPrize), "CutWires")]
        [HarmonyPrefix]
        private static void OnFirstPrizeCut() => _cutFirstPrize = true;

        [HarmonyPatch(typeof(Baldi), "BreakRuler")]
        [HarmonyPrefix]
        private static void OnBaldiRulerBreak() => _cutBaldi = true;

        [HarmonyPatch(typeof(ITM_Scissors), "Use")]
        [HarmonyPrefix]
        private static void OnScissorsUsePrefix(PlayerManager pm)
        {
            _jumpropesBefore = pm.jumpropes.Count;
        }

        [HarmonyPatch(typeof(ITM_Scissors), "Use")]
        [HarmonyPostfix]
        private static void OnScissorsUsePostfix(ITM_Scissors __instance, PlayerManager pm, ref bool __result)
        {
            if (!PatchedAndLatchedPlugin.EnableScissorsBullying.Value) return;
            if (!__result) return;

            if (_cutGum)
            {
                _cutGum = false;
                return;
            }

            bool shouldPunish = (pm.jumpropes.Count < _jumpropesBefore) || _cutFirstPrize || _cutBaldi;

            _cutFirstPrize = false;
            _cutBaldi = false;

            if (shouldPunish)
                pm.RuleBreak("Bullying", 5f);
        }

        [HarmonyPatch(typeof(ITM_Scissors), "Use")]
        [HarmonyPostfix]
        private static void PlaySnipSound(ITM_Scissors __instance, bool __result)
        {
            if (!__result) return;
            if (_audSnipField == null) return;
            var snipSound = _audSnipField.GetValue(__instance) as SoundObject;
            if (snipSound != null)
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(snipSound);
        }
    }
}