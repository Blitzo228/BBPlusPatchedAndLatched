using HarmonyLib;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(MatchActivity), "ReInit")]
    public static class MatchActivityColorsPatch
    {
        private static Color[] PairColors = new Color[5]
        {
            Color.red,
            Color.cyan,
            Color.yellow,
            Color.magenta,
            new Color(1f, 0.5f, 0f)
        };

        private static void Postfix(MatchActivity __instance)
        {
            Traverse val = Traverse.Create(__instance);
            val.Field("balloonPopDelay").SetValue(0f);
            val.Field("balloonPopRate").SetValue(0f);
            val.Field("baldiShortPause").SetValue(0f);

            MatchActivityBalloon[] balloons = val.Field("balloon").GetValue<MatchActivityBalloon[]>();
            int num = 0;

            for (int i = 0; i < balloons.Length; i += 2)
            {
                Color color = (num < PairColors.Length) ? PairColors[num] : Color.white;

                if (balloons[i] != null)
                    TintBalloon(balloons[i], color);

                if (i + 1 < balloons.Length && balloons[i + 1] != null)
                    TintBalloon(balloons[i + 1], color);

                num++;
            }
        }

        private static void TintBalloon(MatchActivityBalloon balloon, Color color)
        {
            Traverse val = Traverse.Create(balloon);
            val.Field("balloonAnimationSpeedMult").SetValue(200f);

            SpriteRenderer spriteRenderer = val.Field("spriteRenderer").GetValue<SpriteRenderer>();

            if (spriteRenderer == null)
            {
                Transform spriteTransform = val.Field("spriteTransform").GetValue<Transform>();
                if (spriteTransform != null)
                    spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer != null)
                spriteRenderer.color = color;
        }
    }
}