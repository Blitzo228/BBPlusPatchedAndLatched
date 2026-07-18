using HarmonyLib;
using PatchedAndLatched;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PatchedAndLatched.Patches
{
    public class ShadowFollower : MonoBehaviour
    {
        public RectTransform? target;
        private RectTransform _myRT;

        private void Awake()
        {
            _myRT = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            if (gameObject.activeSelf != target.gameObject.activeSelf)
                gameObject.SetActive(target.gameObject.activeSelf);

            _myRT.localPosition = target.localPosition + new Vector3(-1f, -1f, 0f);
            _myRT.localScale = target.localScale;
            _myRT.rotation = target.rotation;
        }
    }

    [HarmonyPatch(typeof(TMP_Text), "set_text")]
    internal static class HUDShadowPatch
    {
        [HarmonyPostfix]
        private static void OnTextSet(TMP_Text __instance, string value)
        {
            if (!PatchedAndLatchedPlugin.EnableHUDShadows.Value) return;
            if (__instance == null) return;
            if (__instance.gameObject.name.EndsWith("_Shadow")) return;

            Transform parent = __instance.transform;
            bool isInHUD = false;
            while (parent != null)
            {
                if (parent.GetComponent<HudManager>() != null)
                {
                    isInHUD = true;
                    break;
                }
                parent = parent.parent;
            }

            if (!isInHUD) return;

            TMP_Text shadow = GetOrCreateShadow(__instance);
            if (shadow != null)
            {
                shadow.text = value;
                SyncShadow(__instance, shadow);
            }
        }

        private static TMP_Text GetOrCreateShadow(TMP_Text original)
        {
            string shadowName = original.gameObject.name + "_Shadow";
            Transform parent = original.transform.parent;
            Transform existing = parent.Find(shadowName);

            if (existing != null)
                return existing.GetComponent<TMP_Text>();

            GameObject shadowGo = new GameObject(shadowName);
            shadowGo.transform.SetParent(parent, false);

            TMP_Text shadow = (TMP_Text)shadowGo.AddComponent(original.GetType());
            shadowGo.transform.SetSiblingIndex(original.transform.GetSiblingIndex());
            ((Graphic)shadow).raycastTarget = false;

            ShadowFollower follower = shadowGo.AddComponent<ShadowFollower>();
            follower.target = original.rectTransform;

            return shadow;
        }

        private static void SyncShadow(TMP_Text original, TMP_Text shadow)
        {
            shadow.font = original.font;
            shadow.fontSharedMaterial = original.fontSharedMaterial;
            shadow.fontSize = original.fontSize;
            shadow.alignment = original.alignment;
            shadow.enableWordWrapping = original.enableWordWrapping;
            shadow.richText = original.richText;

            Color color = original.color;
            shadow.color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 0.4f);

            RectTransform rtShadow = shadow.rectTransform;
            RectTransform rtOriginal = original.rectTransform;

            rtShadow.anchorMin = rtOriginal.anchorMin;
            rtShadow.anchorMax = rtOriginal.anchorMax;
            rtShadow.pivot = rtOriginal.pivot;
            rtShadow.sizeDelta = rtOriginal.sizeDelta;
        }
    }
}