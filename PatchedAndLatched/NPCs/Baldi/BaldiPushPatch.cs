using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace PatchedAndLatched.Patches
{
    internal static class BaldiPushPatch
    {
        private static float _lastCatchTime = 0f;
        private static int _pushCount = 0;

        private const float JUMP_DURATION = 0.5f;
        private const float JUMP_HEIGHT = 3f;

        [HarmonyPatch(typeof(Baldi), "CaughtPlayer")]
        [HarmonyPrefix]
        private static bool OnCaughtPlayer(Baldi __instance, PlayerManager player)
        {
            if (!PatchedAndLatchedPlugin.EnableBaldiPushBack.Value)
                return true;

            if (_pushCount >= PatchedAndLatchedPlugin.BaldiMaxPushes.Value)
            {
                _pushCount = 0;
                return true;
            }

            if (Time.time < _lastCatchTime + PatchedAndLatchedPlugin.BaldiPushCooldown.Value)
                return false;

            _lastCatchTime = Time.time;
            _pushCount++;


            Vector3 dir = (__instance.transform.position - player.transform.position).normalized;
            dir.y = 0f;
            __instance.Entity.AddForce(new Force(dir, PatchedAndLatchedPlugin.BaldiPushForce.Value, -25f));

            __instance.StartCoroutine(BaldiJump(__instance.spriteRenderer[0].transform));

            PlaySound("Bang", __instance.transform.position);
            PlaySound("BAL_Ohh", __instance.transform.position, __instance);

            return false;
        }

        private static IEnumerator BaldiJump(Transform baldiTransform)
        {
            float timer = 0f;
            Vector3 startPos = baldiTransform.localPosition;
            while (timer < JUMP_DURATION)
            {
                timer += Time.deltaTime;
                float progress = timer / JUMP_DURATION;
                float yOffset = Mathf.Sin(progress * Mathf.PI) * JUMP_HEIGHT;
                baldiTransform.localPosition = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
                yield return null;
            }
            baldiTransform.localPosition = startPos;
        }

        private static void PlaySound(string name, Vector3 position, Baldi? baldi = null)
        {
            SoundObject sound = Resources.Load<SoundObject>("Sounds/" + name);
            if (sound == null) sound = Resources.Load<SoundObject>(name);
            if (sound == null) sound = Resources.FindObjectsOfTypeAll<SoundObject>().FirstOrDefault(s => s.name == name);

            if (sound == null || sound.soundClip == null)
            {
                var clip = Resources.FindObjectsOfTypeAll<AudioClip>().FirstOrDefault(c => c.name == name);
                if (clip != null)
                {
                    var go = new GameObject("TempAudio");
                    go.transform.position = position;
                    var src = go.AddComponent<AudioSource>();
                    src.spatialBlend = 1f;
                    src.rolloffMode = AudioRolloffMode.Logarithmic;
                    src.maxDistance = 30f;
                    src.PlayOneShot(clip);
                    Object.Destroy(go, clip.length + 0.1f);
                }
                return;
            }


            if (name == "BAL_Ohh" && baldi != null)
            {
                baldi.AudMan.PlaySingle(sound);
            }
            else
            {

                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(sound);
            }
        }
    }
}
