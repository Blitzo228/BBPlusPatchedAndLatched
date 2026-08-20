using HarmonyLib;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(TapePlayer))]
    internal static class TapePlayerPatch
    {
        private static Sprite? _tapePlayerOpenSprite;
        private static FieldInfo? _spriteToChangeField;
        private static FieldInfo? _activeField;

        static TapePlayerPatch()
        {
            _tapePlayerOpenSprite = Resources.FindObjectsOfTypeAll<Sprite>()
                .FirstOrDefault(s => s.name == "TapePlayerOpen");

            _spriteToChangeField = typeof(TapePlayer).GetField("spriteToChange", BindingFlags.NonPublic | BindingFlags.Instance);
            _activeField = typeof(TapePlayer).GetField("active", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPatch("InsertItem")]
        [HarmonyPostfix]
        private static void InsertItemPostfix(TapePlayer __instance)
        {
            if (_tapePlayerOpenSprite == null) return;
            if (_spriteToChangeField == null || _activeField == null) return;

            __instance.StartCoroutine(WaitForCooldownAndSetSprite(__instance));
        }

        private static IEnumerator WaitForCooldownAndSetSprite(TapePlayer player)
        {
            while (true)
            {
                if (_activeField == null) yield break;
                bool active = (bool)_activeField.GetValue(player);
                if (!active) break;
                yield return null;
            }

            if (_spriteToChangeField != null && _tapePlayerOpenSprite != null)
            {
                var sr = _spriteToChangeField.GetValue(player) as SpriteRenderer;
                if (sr != null)
                {
                    sr.sprite = _tapePlayerOpenSprite;
                }
            }
        }
    }
}
