using HarmonyLib;
using PatchedAndLatched;
using System.Collections;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Baldi_Chase))]
    internal static class BaldiKillsNPCsPatch
    {
        private static SoundObject _loseBuzzSound;
        private static bool _soundFound = false;
        private static float _lastSoundTime = -1f;
        private const float SOUND_COOLDOWN = 0.5f; 

        private static SoundObject GetLoseBuzzSound()
        {
            if (_soundFound) return _loseBuzzSound;
            SoundObject[] allSounds = Resources.FindObjectsOfTypeAll<SoundObject>();
            foreach (var s in allSounds)
            {
                if (s != null && s.name == "Lose_Buzz")
                {
                    _loseBuzzSound = s;
                    break;
                }
            }
            _soundFound = true;
            return _loseBuzzSound;
        }

        [HarmonyPatch("OnStateTriggerStay")]
        [HarmonyPrefix]
        private static bool Prefix(Baldi_Chase __instance, Entity otherEntity, Collider other, bool validCollision)
        {
            if (!PatchedAndLatchedPlugin.BaldiKillsNPCs.Value) return true;
            if (!validCollision) return true;
            if (other.CompareTag("Player")) return true;

            NPC npc = other.GetComponent<NPC>();
            if (npc != null)
            {
                Baldi baldi = Traverse.Create(__instance).Field("baldi").GetValue<Baldi>();
                if (baldi == null) return true;

                baldi.StartCoroutine(KillNPCWithGlitch(npc, baldi));
                return false;
            }

            return true;
        }

        private static IEnumerator KillNPCWithGlitch(NPC npc, Baldi baldi)
        {
            if (Time.time - _lastSoundTime > SOUND_COOLDOWN)
            {
                SoundObject buzz = GetLoseBuzzSound();
                if (buzz != null && Singleton<CoreGameManager>.Instance?.audMan != null)
                {
                    if (!Singleton<CoreGameManager>.Instance.audMan.audioSourceManager.isPlaying)
                    {
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(buzz);
                        _lastSoundTime = Time.time;
                    }
                }
            }

            float glitchDuration = 0.5f;
            float glitchIntensity = 3f;
            float elapsed = 0f;

            Shader.SetGlobalFloat("_VertexGlitchIntensity", glitchIntensity);
            Shader.SetGlobalFloat("_TileVertexGlitchIntensity", glitchIntensity);
            Shader.SetGlobalInt("_ColorGlitching", 1);
            Shader.SetGlobalInt("_SpriteColorGlitching", 1);
            Shader.SetGlobalFloat("_VertexGlitchSeed", Random.Range(0f, 1000f));
            Shader.SetGlobalFloat("_TileVertexGlitchSeed", Random.Range(0f, 1000f));

            while (elapsed < glitchDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / glitchDuration;
                float currentIntensity = Mathf.Lerp(glitchIntensity, 0f, progress);
                Shader.SetGlobalFloat("_VertexGlitchIntensity", currentIntensity);
                Shader.SetGlobalFloat("_TileVertexGlitchIntensity", currentIntensity);
                yield return null;
            }

            Shader.SetGlobalFloat("_VertexGlitchIntensity", 0f);
            Shader.SetGlobalFloat("_TileVertexGlitchIntensity", 0f);
            Shader.SetGlobalInt("_ColorGlitching", 0);
            Shader.SetGlobalInt("_SpriteColorGlitching", 0);

            // 3. Удаляем NPC
            if (npc != null)
            {
                if (npc.ec != null)
                    npc.ec.Npcs.Remove(npc);
                Object.Destroy(npc.gameObject);
            }
        }
    }
}