using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches;

[HarmonyPatch(typeof(MapTile))]
public class MapTileFadeInPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("Reveal")]
    public static void RevealPostfix(MapTile __instance, ref SpriteRenderer ___spriteRenderer)
    {
        __instance.StartCoroutine(FadeIn(___spriteRenderer));
        MapIcon[] componentsInChildren = __instance.GetComponentsInChildren<MapIcon>(includeInactive: false);
        foreach (MapIcon mapIcon in componentsInChildren)
        {
            __instance.StartCoroutine(IconFadeIn(mapIcon.spriteRenderer));
        }
    }

    public static IEnumerator FadeIn(SpriteRenderer spriteRen)
    {
        if (spriteRen == null) yield break;

        float alpha = 0f;

        Color currentColor = spriteRen.color;
        spriteRen.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

        while (alpha < 1f)
        {
            alpha += 0.015f;

            currentColor = spriteRen.color;
            spriteRen.color = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Min(alpha, 1f));

            yield return null;
        }

        currentColor = spriteRen.color;
        spriteRen.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
    }

    public static IEnumerator IconFadeIn(SpriteRenderer spriteRen)
    {
        if (spriteRen == null) yield break;

        float alpha = 0f;

        Color currentColor = spriteRen.color;
        spriteRen.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

        while (alpha < 1f)
        {
            alpha += 0.015f;

            currentColor = spriteRen.color;
            spriteRen.color = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Min(alpha, 1f));

            yield return null;
        }

        currentColor = spriteRen.color;
        spriteRen.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
    }
}
