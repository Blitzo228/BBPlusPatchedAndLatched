using HarmonyLib;
using PatchedAndLatched;
using System.Collections;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    internal static class StaminaHideOnCompletePatch
    {
        private static GameObject? _staminometerObject;
        private static CanvasGroup? _canvasGroup;
        private static Coroutine? _fadeCoroutine;

        private static GameObject? GetStaminometer()
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
                    _canvasGroup = _staminometerObject.GetComponent<CanvasGroup>();
                    if (_canvasGroup == null)
                        _canvasGroup = _staminometerObject.AddComponent<CanvasGroup>();
                    _canvasGroup.alpha = 1f;
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

            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;

            if (_fadeCoroutine != null && __instance != null)
                __instance.StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        [HarmonyPatch(typeof(BaseGameManager), "AllNotebooks")]
        [HarmonyPostfix]
        private static void OnAllNotebooks(BaseGameManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaHideOnComplete.Value) return;
            if (__instance is TutorialGameManager) return;

            var staminometer = GetStaminometer();
            if (staminometer == null) return;

            if (_fadeCoroutine != null)
                __instance.StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = __instance.StartCoroutine(FadeOutStaminometer());
        }

        private static IEnumerator FadeOutStaminometer()
        {
            if (_canvasGroup == null) yield break;

            float duration = 1f;
            float elapsed = 0f;
            float startAlpha = _canvasGroup.alpha;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            if (_staminometerObject != null)
                _staminometerObject.SetActive(false);

            _fadeCoroutine = null;
        }
    }
}