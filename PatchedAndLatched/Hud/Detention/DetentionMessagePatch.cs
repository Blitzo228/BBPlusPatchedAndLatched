using HarmonyLib;
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
            {
                textObj.SetActive(true);
            }
        }

        private static void EnsureTextCreated()
        {
            if (textObj != null) return;

            HudManager? hud = Singleton<CoreGameManager>.Instance?.GetHud(0);
            if (hud == null || hud.Canvas() == null) return;

            textObj = new GameObject("DetentionMessageText", typeof(TextMeshProUGUI), typeof(DetentionTextUpdater));
            textObj.transform.SetParent(hud.Canvas().transform, false);

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(600f, 150f);

            detentionText = textObj.GetComponent<TMP_Text>();
            detentionText.alignment = TextAlignmentOptions.Center;
            detentionText.color = Color.red;
            detentionText.fontSize = 28f;

            DetentionTextUpdater updater = textObj.GetComponent<DetentionTextUpdater>();
            updater.textComponent = detentionText;
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
                    textComponent!.text = $"You get detention!\n{seconds} seconds remain.";
                    if (!textComponent.gameObject.activeSelf)
                        textComponent.gameObject.SetActive(true);
                    return;
                }
            }

            if (textComponent != null && textComponent.gameObject.activeSelf)
            {
                textComponent.gameObject.SetActive(false);
            }
        }
    }
}
