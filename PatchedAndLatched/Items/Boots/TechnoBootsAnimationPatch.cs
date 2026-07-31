using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(ITM_Boots))]
    public static class TechnoBootsAnimationPatch
    {
        private static FieldInfo? _setTimeField;
        private static readonly List<GameObject> activeBootsObjects = new List<GameObject>();

        public static void Cleanup()
        {
            for (int i = activeBootsObjects.Count - 1; i >= 0; i--)
            {
                if (activeBootsObjects[i] != null)
                {
                    Object.Destroy(activeBootsObjects[i]);
                }
            }
            activeBootsObjects.Clear();
        }

        [HarmonyPostfix]
        [HarmonyPatch("Use")]
        public static void Use_Postfix(ITM_Boots __instance, PlayerManager pm)
        {
            if (_setTimeField == null)
                _setTimeField = typeof(ITM_Boots).GetField("setTime", BindingFlags.NonPublic | BindingFlags.Instance);

            float duration = 15f;
            if (_setTimeField != null && _setTimeField.GetValue(__instance) is float setVal)
            {
                duration = setVal;
            }

            Sprite? bootsSprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "TechnoBootsSprite");

            HudManager? hud = Singleton<CoreGameManager>.Instance?.GetHud(pm.playerNumber);
            Canvas? canvas = hud?.Canvas();
            if (canvas == null) return;

            GameObject bootsObj = new GameObject("TechnoBootsUIAnimation");
            bootsObj.transform.SetParent(canvas.transform, false);
            activeBootsObjects.Add(bootsObj);

            RectTransform rectTransform = bootsObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            Image img = bootsObj.AddComponent<Image>();
            if (bootsSprite != null)
            {
                img.sprite = bootsSprite;
            }

            hud!.StartCoroutine(AnimateTechnoBoots(rectTransform, canvas, duration));
        }

        private static IEnumerator AnimateTechnoBoots(RectTransform rect, Canvas canvas, float effectDuration)
        {
            if (rect == null || canvas == null) yield break;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            float canvasHeight = canvasRect.rect.height;

            float topY = 80f;
            float bottomY = -canvasHeight - 100f;

            float elapsed = 0f;
            float animDuration = 1.0f;

            while (elapsed < animDuration)
            {
                if (rect == null) yield break;
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / animDuration);
                float y = Mathf.Lerp(topY, bottomY, t);
                rect.anchoredPosition = new Vector2(0f, y);
                yield return null;
            }

            yield return new WaitForSeconds(effectDuration);

            elapsed = 0f;
            while (elapsed < animDuration)
            {
                if (rect == null) yield break;
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / animDuration);
                float y = Mathf.Lerp(bottomY, topY, t);
                rect.anchoredPosition = new Vector2(0f, y);
                yield return null;
            }

            if (rect != null && rect.gameObject != null)
            {
                activeBootsObjects.Remove(rect.gameObject);
                Object.Destroy(rect.gameObject);
            }
        }
    }

    [HarmonyPatch]
    public static class TechnoBootsFloorSwitchPatch
    {
        [HarmonyPatch(typeof(BaseGameManager), "PrepareToLoad")]
        [HarmonyPrefix]
        public static void BaseGameManager_PrepareToLoad_Prefix()
        {
            TechnoBootsAnimationPatch.Cleanup();
        }

        [HarmonyPatch(typeof(CoreGameManager), "PrepareForReload")]
        [HarmonyPrefix]
        public static void CoreGameManager_PrepareForReload_Prefix()
        {
            TechnoBootsAnimationPatch.Cleanup();
        }
    }
}
