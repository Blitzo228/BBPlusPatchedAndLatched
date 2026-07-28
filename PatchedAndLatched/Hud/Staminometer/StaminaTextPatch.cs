using HarmonyLib;
using TMPro;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    internal static class StaminaTextPatch
    {
        private static TMP_Text? _staminaText;
        private static TMP_Text? _restText;
        private static float _updateTimer = 0f;

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

            var staminaNeedle = Traverse.Create(hud).Field<RectTransform>("staminaNeedle").Value;

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
            _restText.text = "YOU NEED REST!";

            RectTransform restRect = _restText.rectTransform;
            restRect.anchorMin = new Vector2(0.5f, 0.5f);
            restRect.anchorMax = new Vector2(0.5f, 0.5f);
            restRect.pivot = new Vector2(0.5f, 0.5f);
            restRect.localScale = Vector3.one;

            float yOffset = 35f;
            if (bgTransform is RectTransform bgRect)
            {
                yOffset = (bgRect.rect.height / 2f) + 50f;
            }

            restRect.anchoredPosition = new Vector2(0f, yOffset);
            restRect.sizeDelta = new Vector2(200f, 30f);

            var font = Resources.Load<TMP_FontAsset>("Fonts/COMIC_24_Pro SDF");
            if (font != null)
            {
                _staminaText.font = font;
                _restText.font = font;
            }

            _restText.gameObject.SetActive(false);

            UpdateText();
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText!.Value) return;
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

            float ratio = maxStamina > 0f ? Mathf.Clamp01(stamina / maxStamina) : 0f;
            int percent = Mathf.RoundToInt(stamina / maxStamina * 100f);

            _staminaText.text = percent + "%";

            _staminaText.color = Color.Lerp(Color.red, Color.green, ratio);

            if (_restText != null)
            {
                bool restTextEnabled = PatchedAndLatchedPlugin.EnableStaminaRestText!.Value;
                _restText.gameObject.SetActive(restTextEnabled && stamina <= 0f);
            }
        }
    }
}
