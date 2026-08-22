using HarmonyLib;
using PatchedAndLatched;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Activity), "Completed", new System.Type[] { typeof(int), typeof(bool) })]
    internal static class IncorrectActivityPraisePatch
    {
        private static FieldInfo? _correctSoundsField;

        static IncorrectActivityPraisePatch()
        {
            _correctSoundsField = AccessTools.Field(typeof(Baldi), "correctSounds");
        }

        [HarmonyPostfix]
        private static void Postfix(Activity __instance, int player, bool correct)
        {
            if (correct) return;

            var baldi = __instance.room?.ec?.GetBaldi();
            if (baldi == null) return;

            if (_correctSoundsField != null)
            {
                var sounds = _correctSoundsField.GetValue(baldi) as WeightedSoundObject[];
                if (sounds != null && sounds.Length > 0)
                {
                    var sound = WeightedSelection<SoundObject>.RandomSelection(sounds);
                    if (sound != null)
                    {
                        baldi.AudMan.FlushQueue(endCurrent: true);
                        baldi.AudMan.PlaySingle(sound);
                    }
                }
            }

            baldi.PraiseAnimation();
            baldi.StartCoroutine(CutPraise(baldi));
        }

        private static IEnumerator CutPraise(Baldi baldi)
        {
            yield return new WaitForSeconds(1.5f);
            if (baldi != null && baldi.gameObject != null)
            {
                baldi.AudMan.FlushQueue(endCurrent: true);
                baldi.ResetSprite();
            }
        }
    }
}