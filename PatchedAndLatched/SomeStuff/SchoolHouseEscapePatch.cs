using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (!PatchedAndLatchedPlugin.SchoolHouseEscape.Value) return;
            if (_escapePlayed) return;

            if (__instance.InPitstop()) return;

            Singleton<MusicManager>.Instance.PlayMidi("Level_1_End", loop: true);
            _escapePlayed = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Initialize")]
        public static void Initialize_Prefix(BaseGameManager __instance)
        {
            if (!PatchedAndLatchedPlugin.SchoolHouseEscape.Value) return;

            _escapePlayed = false;
        }
    }
}
