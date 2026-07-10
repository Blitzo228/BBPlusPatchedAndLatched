using HarmonyLib;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(BalloonBuster), "ReInit")]
    public static class BalloonBusterColorsPatch
    {
        private static void Postfix(BalloonBuster __instance)
        {
            Traverse val = Traverse.Create(__instance);
            val.Field("countUpRate").SetValue(0f);

            BalloonBusterBalloon[] balloons = val.Field("balloon").GetValue<BalloonBusterBalloon[]>();
            int solution = val.Field("solution").GetValue<int>();
            int startingTotal = val.Field("startingTotal").GetValue<int>();

            int num = 0;
            for (int i = 0; i < balloons.Length; i++)
            {
                if (i >= startingTotal || balloons[i] == null)
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
                    if (num < solution)
                    {
                        spriteRenderer.color = Color.green;
                        num++;
                    }
                    else
                    {
                        spriteRenderer.color = Color.red;
                    }
                }
            }
        }
    }
}