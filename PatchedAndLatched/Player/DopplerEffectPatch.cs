using HarmonyLib;
using PatchedAndLatched;
using System.Linq;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public class VelocityTracker : MonoBehaviour
    {
        public Vector3 LastPosition;
        public Vector3 Velocity;

        private void FixedUpdate()
        {
            Velocity = (transform.position - LastPosition) / Time.fixedDeltaTime;
            LastPosition = transform.position;
        }
    }

    public class DopplerSourceData : MonoBehaviour
    {
        public float basePitch = 1f;
        public bool initialized;
    }

    [HarmonyPatch(typeof(AudioManager), "Update")]
    internal static class DopplerEffectPatch
    {
        private const float SPEED_OF_SOUND = 343f;
        private static Vector3 _lastListenerPos;
        private static Vector3 _listenerVelocity;

        private static void UpdateListenerVelocity()
        {
            var listener = Object.FindObjectOfType<AudioListener>();
            if (listener != null)
            {
                _listenerVelocity = (listener.transform.position - _lastListenerPos) / Time.fixedDeltaTime;
                _lastListenerPos = listener.transform.position;
            }
        }

        [HarmonyPostfix]
        private static void ApplyDoppler(AudioManager __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableDopplerEffect!.Value) return;
            if (!__instance.positional || __instance.audioSourceManager == null || !__instance.AnyAudioIsPlaying) return;

            var listener = Object.FindObjectOfType<AudioListener>();
            if (listener == null) return;

            UpdateListenerVelocity();

            var tracker = __instance.GetComponent<VelocityTracker>();
            if (tracker == null)
            {
                tracker = __instance.gameObject.AddComponent<VelocityTracker>();
                tracker.LastPosition = __instance.transform.position;
            }

            Vector3 dirToListener = listener.transform.position - __instance.transform.position;
            float dist = dirToListener.magnitude;
            if (dist < 0.1f) return;

            Vector3 dir = dirToListener / dist;
            float sourceVelocity = Vector3.Dot(tracker.Velocity, dir);
            float listenerVelocity = Vector3.Dot(_listenerVelocity, dir);

            float rawDoppler = (SPEED_OF_SOUND - listenerVelocity) / (SPEED_OF_SOUND - sourceVelocity);
            rawDoppler = Mathf.Clamp(rawDoppler, 0.5f, 2f);

            var source = __instance.audioSourceManager;
            var data = source.GetComponent<DopplerSourceData>();
            if (data == null)
            {
                data = source.gameObject.AddComponent<DopplerSourceData>();
                data.basePitch = source.pitch;
                data.initialized = true;
            }
            else if (!data.initialized)
            {
                data.basePitch = source.pitch;
                data.initialized = true;
            }

            source.pitch = data.basePitch * rawDoppler;
        }
    }
}