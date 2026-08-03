using HarmonyLib;
using PatchedAndLatched;
using TMPro;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    internal static class StaminaTextPatch
    {
        private static TMP_Text? _staminaText;
        private static TMP_Text? _restText;

        private static readonly string[] PercentStrings = PrecachePercentStrings();

        private static string[] PrecachePercentStrings()
        {
            var cache = new string[101];
            for (int i = 0; i <= 100; i++)
                cache[i] = i + "%";
            return cache;
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText!.Value) return;
            CreateText(__instance);
        }

        [HarmonyPatch("ReInit")]
        [HarmonyPostfix]
        private static void ReInitPostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText!.Value) return;
            if (_staminaText != null) return;
            CreateText(__instance);
        }

        private static void CreateText(HudManager hud)
        {
            if (_staminaText != null) return;

            var staminaNeedle = AccessTools.FieldRefAccess<HudManager, RectTransform>("staminaNeedle").Invoke(hud);
            Transform parentTransform = staminaNeedle != null ? staminaNeedle.parent : hud.Canvas().transform;
            if (parentTransform == null) return;

            Transform? bgTransform = parentTransform.Find("Background");
            Transform staminaTextParent = bgTransform != null ? bgTransform : parentTransform;

            var go = new GameObject("StaminaPercentText");
            go.transform.SetParent(staminaTextParent, false);
            _staminaText = go.AddComponent<TextMeshProUGUI>();
            _staminaText.fontSize = 18;
            _staminaText.color = Color.green;
            _staminaText.alignment = TextAlignmentOptions.Center;
            _staminaText.raycastTarget = false;
            _staminaText.enableWordWrapping = false;
            _staminaText.overflowMode = TextOverflowModes.Overflow;

            RectTransform rect = _staminaText.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(80f, 30f);

            var restGo = new GameObject("StaminaRestText");
            restGo.transform.SetParent(staminaTextParent, false);

            _restText = restGo.AddComponent<TextMeshProUGUI>();
            _restText.fontSize = 20;
            _restText.color = Color.red;
            _restText.alignment = TextAlignmentOptions.Center;
            _restText.raycastTarget = false;
            _restText.enableWordWrapping = false;
            _restText.overflowMode = TextOverflowModes.Overflow;
            _restText.text = GetLocalizedRestText();

            RectTransform restRect = _restText.rectTransform;
            restRect.anchorMin = new Vector2(0.5f, 0.5f);
            restRect.anchorMax = new Vector2(0.5f, 0.5f);
            if (PatchedAndLatchedPlugin.IsCyrillicPlusInstalled)
                restRect.anchorMax = new Vector2(0.8f, 0.5f);
            restRect.pivot = new Vector2(0.5f, 0.5f);
            restRect.localScale = Vector3.one;

            float yOffset = 35f;
            if (bgTransform is RectTransform bgRect)
                yOffset = (bgRect.rect.height / 2f) + 50f;

            restRect.anchoredPosition = new Vector2(0f, yOffset);
            restRect.sizeDelta = new Vector2(200f, 30f);

            _restText.gameObject.SetActive(false);

            UpdateText(1f);
        }

        [HarmonyPatch("SetStaminaValue")]
        [HarmonyPostfix]
        private static void SetStaminaValuePostfix(float val)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText!.Value) return;
            UpdateText(val);
        }

        private static void UpdateText(float ratio)
        {
            if (_staminaText == null) return;

            int percent = Mathf.RoundToInt(Mathf.Max(0f, ratio) * 100f);
            _staminaText.text = (percent >= 0 && percent <= 100) ? PercentStrings[percent] : (percent + "%");

            float colorRatio = Mathf.Clamp01(ratio);
            _staminaText.color = Color.Lerp(Color.red, Color.green, colorRatio);

            if (_restText != null)
            {
                bool restTextEnabled = PatchedAndLatchedPlugin.EnableStaminaRestText!.Value;
                _restText.gameObject.SetActive(restTextEnabled && ratio <= 0f);
            }
        }

        private static string GetLocalizedRestText()
        {
            if (PatchedAndLatchedPlugin.IsCyrillicPlusInstalled)
                return "ТЕБЕ НУЖЕН ОТДЫХ!";
            return "YOU NEED REST!";
        }
    }
}
