using HarmonyLib;
using PatchedAndLatched;
using System.Collections;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    // Компонент для управления анимацией растворения
    public class DissolveController : MonoBehaviour
    {
        private Pickup? _pickup;
        private Collider? _collider;
        private bool _isRunning;

        public void StartDissolve(Pickup pickup)
        {
            if (_isRunning) return;
            _pickup = pickup;
            _collider = pickup.GetComponent<Collider>();
            StartCoroutine(DissolveCoroutine());
        }

        private IEnumerator DissolveCoroutine()
        {
            _isRunning = true;

            if (_pickup != null)
            {
                if (_collider != null) _collider.enabled = false;
            }

            var sr = _pickup?.itemSprite;
            if (sr != null)
            {
                Color orig = sr.color;
                float duration = PatchedAndLatchedPlugin.PickupDissolveDuration.Value;
                float timer = 0f;
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, timer / duration);
                    sr.color = new Color(orig.r, orig.g, orig.b, alpha);
                    yield return null;
                }
                sr.color = new Color(orig.r, orig.g, orig.b, 0f);
            }

            if (_pickup != null)
            {
                _pickup.gameObject.SetActive(false);
                if (_pickup.icon != null)
                    _pickup.icon.spriteRenderer.enabled = false;
            }

            _isRunning = false;
            Destroy(this);
        }
    }

    [HarmonyPatch(typeof(Pickup))]
    internal static class PickupDissolvePatch
    {
        [HarmonyPatch("Hide")]
        [HarmonyPrefix]
        private static bool PrefixHide(Pickup __instance, bool hidden)
        {
            if (!PatchedAndLatchedPlugin.EnablePickupDissolve.Value) return true;
            if (!hidden) return true;

            if (__instance.GetComponent<DissolveController>() != null)
                return false;

            var controller = __instance.gameObject.AddComponent<DissolveController>();
            controller.StartDissolve(__instance);
            return false; 
        }
    }
}