using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(BalloonBuster))]
    public static class BalloonBusterColorsPatch
    {
        private static Sprite? greenSprite;
        private static Sprite? redSprite;
        private static bool spritesLoaded = false;

        private static void LoadSprites()
        {
            if (spritesLoaded) return;

            Sprite[] allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
            foreach (Sprite s in allSprites)
            {
                if (s.name == "BalloonBuster_Balloons_Sheet_2")
                    greenSprite = s;
                else if (s.name == "BalloonBuster_Balloons_Sheet_6")
                    redSprite = s;

                if (greenSprite != null && redSprite != null)
                    break;
            }

            spritesLoaded = true;
        }

        [HarmonyPatch("ReInit")]
        [HarmonyPostfix]
        private static void Postfix_ReInit(BalloonBuster __instance)
        {
            Traverse.Create(__instance).Field("countUpRate").SetValue(0f);

            UpdateBalloonColors(__instance);
        }

        [HarmonyPatch("CheckCount")]
        [HarmonyPostfix]
        private static void Postfix_CheckCount(BalloonBuster __instance)
        {
            UpdateBalloonColors(__instance);
        }

        private static void UpdateBalloonColors(BalloonBuster __instance)
        {
            LoadSprites();

            Traverse val = Traverse.Create(__instance);
            BalloonBusterBalloon[] balloons = val.Field("balloon").GetValue<BalloonBusterBalloon[]>();
            int solution = val.Field("solution").GetValue<int>();
            int startingTotal = val.Field("startingTotal").GetValue<int>();

            int unpoppedCount = 0;
            for (int i = 0; i < startingTotal; i++)
            {
                if (balloons[i] != null && !balloons[i].popped)
                {
                    unpoppedCount++;
                }
            }

            bool cannotPopMore = unpoppedCount <= solution;

            for (int i = 0; i < startingTotal; i++)
            {
                if (balloons[i] == null || balloons[i].popped)
                    continue;

                Traverse balloonTraverse = Traverse.Create(balloons[i]);
                SpriteRenderer spriteRenderer = balloonTraverse.Field("spriteRenderer").GetValue<SpriteRenderer>();

                if (spriteRenderer == null)
                {
                    Transform spriteTransform = balloonTraverse.Field("spriteTransform").GetValue<Transform>();
                    if (spriteTransform != null)
                        spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();

                    if (spriteRenderer == null)
                        spriteRenderer = balloons[i].GetComponentInChildren<SpriteRenderer>();
                }

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;

                    if (cannotPopMore)
                    {
                        if (redSprite != null)
                            spriteRenderer.sprite = redSprite;
                    }
                    else
                    {
                        if (greenSprite != null)
                            spriteRenderer.sprite = greenSprite;
                    }
                }
            }
        }
    }
}
