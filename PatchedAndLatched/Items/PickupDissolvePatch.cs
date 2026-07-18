using HarmonyLib;
using PatchedAndLatched;
using System.Collections;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Pickup))]
    internal static class PickupDissolvePatch
    {
        [HarmonyPatch("Hide")]
        [HarmonyPrefix]
        private static bool PrefixHide(Pickup __instance, bool hidden)
        {
            if (!PatchedAndLatchedPlugin.EnablePickupDissolve.Value) return true;
            if (!hidden) return true; 

            __instance.StartCoroutine(DissolveCoroutine(__instance));
            return false; 
        }

        private static IEnumerator DissolveCoroutine(Pickup pickup)
        {
            SpriteRenderer sr = pickup.itemSprite;
            if (sr == null) yield break;

            Color origColor = sr.color;
            float duration = PatchedAndLatchedPlugin.PickupDissolveDuration.Value;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timer / duration);
                sr.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
                yield return null;
            }

            sr.color = new Color(origColor.r, origColor.g, origColor.b, 0f);
            pickup.gameObject.SetActive(false);

            if (pickup.icon != null)
                pickup.icon.spriteRenderer.enabled = false;
        }
    }
}