using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(BaseGameManager))]
    public static class SchoolHouseEscapePatch
    {
        private static bool _escapePlayed = false;

        [HarmonyPostfix]
        [HarmonyPatch("AllNotebooks")]
        public static void AllNotebooks_Postfix(BaseGameManager __instance)
        {
            if (_escapePlayed) return;
            if (__instance.InPitstop()) return;

            string midiName = (Random.value < 0.1f) ? "CampMinigame_1_1" : "Level_1_End";
            Singleton<MusicManager>.Instance.PlayMidi(midiName, loop: true);
            _escapePlayed = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Initialize")]
        public static void Initialize_Prefix(BaseGameManager __instance)
        {
            _escapePlayed = false;
        }
    }
}