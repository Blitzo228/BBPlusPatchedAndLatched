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
        private static float _updateTimer = 0f;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText.Value) return;
            CreateText(__instance);
        }

        [HarmonyPatch("ReInit")]
        [HarmonyPostfix]
        private static void ReInitPostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText.Value) return;
            if (_staminaText != null) return;
            CreateText(__instance);
        }

        private static void CreateText(HudManager hud)
        {
            if (_staminaText != null) return;

            var canvas = hud.Canvas();
            if (canvas == null) return;

            var go = new GameObject("StaminaPercentText");
            go.transform.SetParent(canvas.transform, false);

            _staminaText = go.AddComponent<TextMeshProUGUI>();
            _staminaText.fontSize = 20;
            _staminaText.color = Color.black;
            _staminaText.alignment = TextAlignmentOptions.MidlineLeft;
            _staminaText.rectTransform.anchoredPosition = new Vector2(-215f, -145f);
            _staminaText.rectTransform.sizeDelta = new Vector2(60f, 25f);

            var font = Resources.Load<TMP_FontAsset>("Fonts/COMIC_24_Pro SDF");
            if (font != null)
                _staminaText.font = font;

            UpdateText();
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText.Value) return;
            if (_staminaText == null) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer < 0.2f) return;
            _updateTimer = 0f;

            UpdateText();
        }

        private static void UpdateText()
        {
            if (_staminaText == null) return;

            var player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
            if (player == null) return;

            float stamina = player.plm.stamina;
            float maxStamina = player.plm.StaminaMax;
            int percent = Mathf.RoundToInt(stamina / maxStamina * 100f);

            _staminaText.text = percent + "%";
        }
    }
}