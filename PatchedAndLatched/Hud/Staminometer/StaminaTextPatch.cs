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
        private static float _updateTimer = 0f;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText.Value) return;
            CreateTexts(__instance);
        }

        [HarmonyPatch("ReInit")]
        [HarmonyPostfix]
        private static void ReInitPostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText.Value) return;
            if (_staminaText != null) return;
            CreateTexts(__instance);
        }

        private static void CreateTexts(HudManager hud)
        {
            if (_staminaText != null) return;

            var canvas = hud.Canvas();
            if (canvas == null) return;

            var goPercent = new GameObject("StaminaPercentText");
            goPercent.transform.SetParent(canvas.transform, false);

            _staminaText = goPercent.AddComponent<TextMeshProUGUI>();
            _staminaText.fontSize = 20;
            _staminaText.color = Color.black;
            _staminaText.alignment = TextAlignmentOptions.MidlineLeft;
            _staminaText.rectTransform.anchoredPosition = new Vector2(-215f, -145f);
            _staminaText.rectTransform.sizeDelta = new Vector2(60f, 25f);

            var goRest = new GameObject("StaminaRestText");
            goRest.transform.SetParent(canvas.transform, false);

            _restText = goRest.AddComponent<TextMeshProUGUI>();
            _restText.fontSize = 16; 
            _restText.color = Color.red;
            _restText.alignment = TextAlignmentOptions.MidlineLeft;
            _restText.rectTransform.anchoredPosition = new Vector2(-215f, -145f);
            _restText.rectTransform.sizeDelta = new Vector2(150f, 25f); 
            _restText.text = "YOU NEED REST!";
            _restText.gameObject.SetActive(false);


            var font = Resources.Load<TMP_FontAsset>("Fonts/COMIC_24_Pro SDF");
            if (font != null)
            {
                _staminaText.font = font;
                _restText.font = font;
            }

            UpdateTexts();
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableStaminaText.Value) return;
            if (_staminaText == null || _restText == null) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer < 0.2f) return;
            _updateTimer = 0f;

            UpdateTexts();
        }

        private static void UpdateTexts()
        {
            if (_staminaText == null || _restText == null) return;

            var player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
            if (player == null) return;

            float stamina = player.plm.stamina;
            float maxStamina = player.plm.StaminaMax;
            int percent = Mathf.RoundToInt(stamina / maxStamina * 100f);

            if (stamina <= 0.01f)
            {
                _staminaText.gameObject.SetActive(false);
                _restText.gameObject.SetActive(true);
            }
            else
            {
                _staminaText.text = percent + "%";
                _staminaText.gameObject.SetActive(true);
                _restText.gameObject.SetActive(false);
            }
        }
    }
}
