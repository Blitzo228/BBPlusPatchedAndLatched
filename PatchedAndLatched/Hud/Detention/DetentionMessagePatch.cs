using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(DetentionRoomFunction))]
    public class DetentionMessagePatch
    {
        private static GameObject? textObj;
        private static TMP_Text? detentionText;
        public static DetentionRoomFunction? currentDetentionRoom;

        [HarmonyPatch("Activate")]
        [HarmonyPostfix]
        static void ActivatePostfix(float time, DetentionRoomFunction __instance)
        {
            currentDetentionRoom = __instance;
            EnsureTextCreated();
            if (textObj != null)
                textObj.SetActive(true);
        }

        private static void EnsureTextCreated()
        {
            if (textObj != null) return;

            HudManager? hud = Singleton<CoreGameManager>.Instance?.GetHud(0);
            if (hud == null || hud.Canvas() == null) return;
            TMP_FontAsset? baseFont = Resources.Load<TMP_FontAsset>("Fonts/COMIC_24_Pro SDF");
            if (baseFont == null)
                baseFont = TMP_Settings.defaultFontAsset;

            textObj = new GameObject("DetentionMessageText", typeof(TextMeshProUGUI), typeof(DetentionTextUpdater));
            textObj.transform.SetParent(hud.Canvas().transform, false);

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(600f, 150f);

            detentionText = textObj.GetComponent<TMP_Text>();
            detentionText.font = baseFont;
            detentionText.alignment = TextAlignmentOptions.Center;
            detentionText.color = Color.red;
            detentionText.fontSize = 28f;
            detentionText.enableWordWrapping = true;

            DetentionTextUpdater updater = textObj.GetComponent<DetentionTextUpdater>();
            updater.textComponent = detentionText;
            TryApplyCyrillic(detentionText);
        }

        private static void TryApplyCyrillic(TMP_Text text)
        {
            if (!PatchedAndLatchedPlugin.IsCyrillicPlusInstalled) return;
            var type = System.Type.GetType("CyrillicPlus.BasePlugin, CyrillicPlus");
            if (type == null) return;
            var method = type.GetMethod("ProccessComponent", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
                method.Invoke(null, new object[] { text });
        }

        [HarmonyPatch(typeof(HudManager), "ReInit")]
        [HarmonyPostfix]
        static void ReInitPostfix()
        {
            if (textObj != null)
            {
                Object.Destroy(textObj);
                textObj = null;
                detentionText = null;
            }
            currentDetentionRoom = null;
        }
    }

    public class DetentionTextUpdater : MonoBehaviour
    {
        public TMP_Text? textComponent;

        void Update()
        {
            if (DetentionMessagePatch.currentDetentionRoom != null)
            {
                float time = Traverse.Create(DetentionMessagePatch.currentDetentionRoom).Field("time").GetValue<float>();
                bool active = Traverse.Create(DetentionMessagePatch.currentDetentionRoom).Field("active").GetValue<bool>();

                if (active && time > 0f)
                {
                    int seconds = Mathf.CeilToInt(time);
                    textComponent!.text = GetLocalizedDetentionText(seconds);
                    if (!textComponent.gameObject.activeSelf)
                        textComponent.gameObject.SetActive(true);
                    return;
                }
            }

            if (textComponent != null && textComponent.gameObject.activeSelf)
                textComponent.gameObject.SetActive(false);
        }

        private static string GetLocalizedDetentionText(int seconds)
        {
            if (PatchedAndLatchedPlugin.IsCyrillicPlusInstalled)
                return $"Ты получили наказание!\nОсталось {seconds} секунд.";
            return $"You get detention!\n{seconds} seconds remain.";
        }
    }
}