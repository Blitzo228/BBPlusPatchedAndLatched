using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    internal static class StaminaHideOnCompletePatch
    {
        private static GameObject? _staminometerObject;
        private static bool _saved = false;

        private static GameObject GetStaminometer()
        {
            if (_staminometerObject != null)
                return _staminometerObject;

            var hud = Singleton<CoreGameManager>.Instance?.GetHud(0);
            if (hud == null) return null;

            var canvas = hud.Canvas();
            if (canvas == null) return null;

            foreach (Transform child in canvas.transform)
            {
                if (child.name == "Staminometer")
                {
                    _staminometerObject = child.gameObject;
                    _saved = true;
                    return _staminometerObject;
                }
            }

            return null;
        }

        [HarmonyPatch(typeof(EnvironmentController), "BeginPlay")]
        [HarmonyPostfix]
        private static void OnBeginPlay(EnvironmentController __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaHideOnComplete.Value) return;

            var staminometer = GetStaminometer();
            if (staminometer == null) return;

            if (!staminometer.activeSelf)
                staminometer.SetActive(true);
        }

        [HarmonyPatch(typeof(BaseGameManager), "AllNotebooks")]
        [HarmonyPostfix]
        private static void OnAllNotebooks(BaseGameManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaHideOnComplete.Value) return;
            if (__instance is TutorialGameManager) return;

            var staminometer = GetStaminometer();
            if (staminometer == null) return;

            staminometer.SetActive(false);
        }
    }
}
