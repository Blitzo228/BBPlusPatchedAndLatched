using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    internal static class MainMenuTestMapsPatch
    {
        private static FieldInfo? _seedInputField;

        static MainMenuTestMapsPatch()
        {
            _seedInputField = AccessTools.Field(typeof(MainMenu), "seedInput");
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPostfix(MainMenu __instance)
        {

            if (_seedInputField != null)
            {
                var seedInput = _seedInputField.GetValue(__instance) as GameObject;
                if (seedInput != null)
                    seedInput.SetActive(true);
            }

            foreach (Transform child in __instance.transform)
            {
                string name = child.name;
                if (name == "StartTest" || name == "StartTest_1")
                    child.gameObject.SetActive(true);
            }
        }
    }
}