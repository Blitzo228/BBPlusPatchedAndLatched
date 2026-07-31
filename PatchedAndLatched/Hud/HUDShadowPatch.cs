using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PatchedAndLatched.Patches
{
    public class ShadowFollower : MonoBehaviour
    {
        public TMP_Text? targetText;
        private RectTransform? _myRT;
        private TMP_Text? _myText;

        private Color _lastColor;
        private bool? _lastEnabled = null;

        private void Awake()
        {
            _myRT = GetComponent<RectTransform>();
            _myText = GetComponent<TMP_Text>();

            if (_myText != null)
            {
                _myText.text = string.Empty;
                _myText.color = Color.clear;
                _myText.enabled = false;
                CanvasRenderer cr = GetComponent<CanvasRenderer>();
                if (cr != null)
                {
                    cr.Clear();
                }
            }
        }
        private void OnEnable()
        {
            Canvas.willRenderCanvases += SyncShadow;
        }

        private void OnDisable()
        {
            Canvas.willRenderCanvases -= SyncShadow;
        }

        private void Update()
        {
            if (targetText == null)
            {
                Destroy(gameObject);
            }
        }

        private void SyncShadow()
        {
            if (targetText == null || _myText == null || _myRT == null) return;

            bool shouldBeVisible = targetText.gameObject.activeInHierarchy && targetText.enabled;

            if (_lastEnabled != shouldBeVisible)
            {
                _myText.enabled = shouldBeVisible;
                _lastEnabled = shouldBeVisible;
            }

            if (!shouldBeVisible) return;

            RectTransform targetRT = targetText.rectTransform;
            _myRT.localPosition = targetRT.localPosition + new Vector3(-1f, -1f, 0f);
            _myRT.localScale = targetRT.localScale;
            _myRT.rotation = targetRT.rotation;
            _myRT.anchorMin = targetRT.anchorMin;
            _myRT.anchorMax = targetRT.anchorMax;
            _myRT.pivot = targetRT.pivot;
            _myRT.sizeDelta = targetRT.sizeDelta;

            if (_myText.text != targetText.text) _myText.text = targetText.text;
            if (_myText.fontSize != targetText.fontSize) _myText.fontSize = targetText.fontSize;
            if (_myText.font != targetText.font) _myText.font = targetText.font;
            if (_myText.alignment != targetText.alignment) _myText.alignment = targetText.alignment;
            if (_myText.enableWordWrapping != targetText.enableWordWrapping) _myText.enableWordWrapping = targetText.enableWordWrapping;
            if (_myText.richText != targetText.richText) _myText.richText = targetText.richText;
            if (_myText.fontSharedMaterial != targetText.fontSharedMaterial) _myText.fontSharedMaterial = targetText.fontSharedMaterial;

            Color c = targetText.color;
            if (_lastColor != c)
            {
                _lastColor = c;
                _myText.color = new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f, c.a * 0.4f);
            }
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    internal static class HUDShadowPatch
    {
        private static readonly List<TMP_Text> TextCache = new List<TMP_Text>();
        private static readonly HashSet<TMP_Text> ProcessedTexts = new HashSet<TMP_Text>();

        private static float _scanTimer = 0f;
        private const float ScanInterval = 0.05f;

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void AwakePostfix(HudManager __instance)
        {
            ProcessedTexts.Clear();
            ProcessHUDTexts(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch("ReInit")]
        private static void ReInitPostfix(HudManager __instance)
        {
            ProcessHUDTexts(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        private static void UpdatePostfix(HudManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableHUDShadows!.Value || __instance == null) return;

            _scanTimer += Time.deltaTime;
            if (_scanTimer < ScanInterval) return;
            _scanTimer = 0f;

            ProcessHUDTexts(__instance);
        }

        private static void ProcessHUDTexts(HudManager hud)
        {
            if (!PatchedAndLatchedPlugin.EnableHUDShadows!.Value || hud == null) return;

            TextCache.Clear();
            hud.GetComponentsInChildren(true, TextCache);

            for (int i = 0; i < TextCache.Count; i++)
            {
                TMP_Text original = TextCache[i];

                if (original == null || ProcessedTexts.Contains(original) || original.gameObject.name.EndsWith("_Shadow"))
                    continue;

                GetOrCreateShadow(original);
                ProcessedTexts.Add(original);
            }
        }

        private static TMP_Text GetOrCreateShadow(TMP_Text original)
        {
            string shadowName = original.gameObject.name + "_Shadow";
            Transform parent = original.transform.parent;
            Transform existing = parent.Find(shadowName);

            if (existing != null)
            {
                return existing.GetComponent<TMP_Text>();
            }

            GameObject shadowGo = new GameObject(shadowName);
            shadowGo.transform.SetParent(parent, false);
            TMP_Text shadow = (TMP_Text)shadowGo.AddComponent(original.GetType());
            shadowGo.transform.SetSiblingIndex(original.transform.GetSiblingIndex());
            ((Graphic)shadow).raycastTarget = false;

            ShadowFollower follower = shadowGo.AddComponent<ShadowFollower>();
            follower.targetText = original;

            return shadow;
        }
    }
}
