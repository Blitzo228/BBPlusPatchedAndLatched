using HarmonyLib;
using PatchedAndLatched;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(GottaSweep))]
    public static class GottaSweepAccelerationPatch
    {
        private const float ACCEL_DURATION = 15f;
        private const float START_MULTIPLIER = 0.2f;

        private static Coroutine? _currentCoroutine = null;
        private static FieldInfo _navigatorField;
        private static FieldInfo _speedField;

        static GottaSweepAccelerationPatch()
        {
            _navigatorField = typeof(NPC).GetField("navigator", BindingFlags.NonPublic | BindingFlags.Instance);
            _speedField = typeof(GottaSweep).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch("StartSweeping")]
        public static void StartSweeping_Postfix(GottaSweep __instance)
        {
            if (!PatchedAndLatchedPlugin.GottaSweepAcceleration.Value) return;

            if (_currentCoroutine != null)
            {
                __instance.StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }

            _currentCoroutine = __instance.StartCoroutine(AccelerateCoroutine(__instance));
        }

        private static IEnumerator AccelerateCoroutine(GottaSweep sweep)
        {
            Navigator? navigator = _navigatorField?.GetValue(sweep) as Navigator;
            if (navigator == null) yield break;

            float speed = (float)_speedField.GetValue(sweep);

            float elapsed = 0f;
            float startSpeed = speed * START_MULTIPLIER;
            float targetSpeed = speed;

            navigator.SetSpeed(startSpeed);

            while (elapsed < ACCEL_DURATION)
            {
                elapsed += Time.deltaTime * sweep.TimeScale;
                float t = elapsed / ACCEL_DURATION;
                float currentSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
                navigator.SetSpeed(currentSpeed);
                yield return null;
            }

            navigator.SetSpeed(targetSpeed);
            _currentCoroutine = null;
        }
    }
}
