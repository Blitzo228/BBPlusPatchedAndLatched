using HarmonyLib;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PatchedAndLatched.Patches
{
    public static class NametagHudAnimationState
    {
        public static Coroutine? AnimationCoroutine;
        public static Image? NametagHudImage;
        public static Sprite[]? NametagSprites;

        public static void Cleanup()
        {
            if (NametagHudImage != null)
            {
                if (NametagHudImage.gameObject != null)
                {
                    Object.Destroy(NametagHudImage.gameObject);
                }
                NametagHudImage = null;
            }
            AnimationCoroutine = null;
        }
    }

    [HarmonyPatch(typeof(ITM_Nametag), nameof(ITM_Nametag.Use))]
    public static class NametagUsePatch
    {
        static void Postfix(ITM_Nametag __instance, PlayerManager pm, bool __result)
        {
            if (!__result) return;

            HudManager hud = Singleton<CoreGameManager>.Instance.GetHud(pm.playerNumber);
            if (hud == null) return;

            if (NametagHudAnimationState.NametagSprites == null)
            {
                NametagHudAnimationState.NametagSprites = new Sprite[8];
                for (int i = 0; i < 8; i++)
                {
                    string spriteName = $"tag000{i}";
                    NametagHudAnimationState.NametagSprites[i] = Resources.FindObjectsOfTypeAll<Sprite>()
                        .FirstOrDefault(s => s.name == spriteName);
                }
            }

            if (NametagHudAnimationState.NametagHudImage == null)
            {
                GameObject obj = new GameObject("NametagHudSprite", typeof(RectTransform), typeof(Image));
                obj.transform.SetParent(hud.Canvas().transform, false);

                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(0f, -35f);
                rect.sizeDelta = new Vector2(160f, 160f);

                NametagHudAnimationState.NametagHudImage = obj.GetComponent<Image>();
            }

            NametagHudAnimationState.NametagHudImage.gameObject.SetActive(true);

            if (NametagHudAnimationState.AnimationCoroutine != null)
            {
                hud.StopCoroutine(NametagHudAnimationState.AnimationCoroutine);
            }
            NametagHudAnimationState.AnimationCoroutine = hud.StartCoroutine(CycleNametagFrames(pm));
        }

        private static IEnumerator CycleNametagFrames(PlayerManager pm)
        {
            int frame = 0;

            while (pm.Tagged && NametagHudAnimationState.NametagHudImage != null)
            {
                if (NametagHudAnimationState.NametagSprites != null && NametagHudAnimationState.NametagSprites.Length > 0)
                {
                    if (NametagHudAnimationState.NametagSprites[frame] != null)
                    {
                        NametagHudAnimationState.NametagHudImage.sprite = NametagHudAnimationState.NametagSprites[frame];
                    }
                }

                frame = (frame + 1) % 8;
                yield return new WaitForSeconds(0.125f);
            }

            if (NametagHudAnimationState.NametagHudImage != null)
            {
                NametagHudAnimationState.NametagHudImage.gameObject.SetActive(false);
            }
        }
    }

    [HarmonyPatch]
    public static class NametagHudFloorSwitchPatch
    {
        [HarmonyPatch(typeof(BaseGameManager), "PrepareToLoad")]
        [HarmonyPrefix]
        public static void BaseGameManager_PrepareToLoad_Prefix()
        {
            NametagHudAnimationState.Cleanup();
        }

        [HarmonyPatch(typeof(CoreGameManager), "PrepareForReload")]
        [HarmonyPrefix]
        public static void CoreGameManager_PrepareForReload_Prefix()
        {
            NametagHudAnimationState.Cleanup();
        }
    }
}
