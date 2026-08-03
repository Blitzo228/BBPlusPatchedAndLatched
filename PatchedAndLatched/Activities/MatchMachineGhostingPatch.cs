using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    internal static class MatchMachineGhostingPatch
    {
        private static FieldInfo? _activityField;
        private static FieldInfo? _balloonArrayField;
        private static FieldInfo? _spriteRendererField;
        private static FieldInfo? _revealedSpriteField;
        private static FieldInfo? _revealingField;
        private static FieldInfo? _completedField;
        private static FieldInfo? _balloonPopDelayField;
        private static FieldInfo? _balloonPopRateField;

        private static Dictionary<MatchActivityBalloon, Sprite> _originalSprites = new Dictionary<MatchActivityBalloon, Sprite>();

        static MatchMachineGhostingPatch()
        {
            _activityField = AccessTools.Field(typeof(MatchActivityBalloon), "activity");
            _balloonArrayField = AccessTools.Field(typeof(MatchActivity), "balloon");
            _spriteRendererField = AccessTools.Field(typeof(MatchActivityBalloon), "spriteRenderer");
            _revealedSpriteField = AccessTools.Field(typeof(MatchActivityBalloon), "revealedSprite");
            _revealingField = AccessTools.Field(typeof(MatchActivityBalloon), "revealing");
            _completedField = AccessTools.Field(typeof(MatchActivityBalloon), "completed");
            _balloonPopDelayField = AccessTools.Field(typeof(MatchActivity), "balloonPopDelay");
            _balloonPopRateField = AccessTools.Field(typeof(MatchActivity), "balloonPopRate");
        }

        private static void ShowGhostedBalloons(MatchActivityBalloon current)
        {
            if (_activityField == null || _balloonArrayField == null || _spriteRendererField == null || _revealedSpriteField == null || _revealingField == null || _completedField == null) return;

            var activity = _activityField.GetValue(current) as MatchActivity;
            if (activity == null) return;

            var balloons = _balloonArrayField.GetValue(activity) as MatchActivityBalloon[];
            if (balloons == null) return;

            var toRemove = new List<MatchActivityBalloon>();
            foreach (var kv in _originalSprites)
            {
                if (kv.Key == null || (bool)_completedField.GetValue(kv.Key))
                    toRemove.Add(kv.Key!);
            }
            foreach (var b in toRemove)
                _originalSprites.Remove(b);

            foreach (var b in balloons)
            {
                if (b == null || b == current) continue;
                bool completed = (bool)_completedField.GetValue(b);
                bool revealing = (bool)_revealingField.GetValue(b);
                if (completed || revealing) continue;

                var sr = _spriteRendererField.GetValue(b) as SpriteRenderer;
                if (sr == null) continue;

                if (sr.sprite == _revealedSpriteField.GetValue(b) as Sprite)
                    continue;

                if (!_originalSprites.ContainsKey(b))
                {
                    _originalSprites[b] = sr.sprite;
                }

                sr.sprite = _revealedSpriteField.GetValue(b) as Sprite;
                Color c = sr.color;
                c.a = 0.3f;
                sr.color = c;
            }
        }

        private static void RestoreBalloons()
        {
            if (_spriteRendererField == null || _completedField == null) return;

            foreach (var kv in _originalSprites)
            {
                var b = kv.Key;
                if (b == null) continue;
                if ((bool)_completedField.GetValue(b)) continue;
                var sr = _spriteRendererField.GetValue(b) as SpriteRenderer;
                if (sr == null) continue;
                sr.sprite = kv.Value;
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
            _originalSprites.Clear();
        }

        private static void SetBalloonAlpha(MatchActivityBalloon b, float alpha)
        {
            if (b == null) return;
            var sr = _spriteRendererField?.GetValue(b) as SpriteRenderer;
            if (sr == null) return;
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        [HarmonyPatch(typeof(MatchActivityBalloon), "RevealConfirm")]
        [HarmonyPostfix]
        private static void RevealConfirmPostfix(MatchActivityBalloon __instance)
        {
            ShowGhostedBalloons(__instance);
            SetBalloonAlpha(__instance, 1f);
        }

        [HarmonyPatch(typeof(MatchActivityBalloon), "Unreveal")]
        [HarmonyPostfix]
        private static void UnrevealPostfix(MatchActivityBalloon __instance)
        {
            RestoreBalloons();
        }

        [HarmonyPatch(typeof(MatchActivityBalloon), "Matched")]
        [HarmonyPostfix]
        private static void MatchedPostfix(MatchActivityBalloon __instance)
        {
            RestoreBalloons();
        }

        [HarmonyPatch(typeof(MatchActivity), "ReInit")]
        [HarmonyPostfix]
        private static void ReInitPostfix(MatchActivity __instance)
        {
            RestoreBalloons();
            _originalSprites.Clear();
            var balloons = _balloonArrayField?.GetValue(__instance) as MatchActivityBalloon[];
            if (balloons != null)
            {
                foreach (var b in balloons)
                {
                    if (b != null)
                        SetBalloonAlpha(b, 1f);
                }
            }
            if (_balloonPopDelayField != null)
                _balloonPopDelayField.SetValue(__instance, 0f);
            if (_balloonPopRateField != null)
                _balloonPopRateField.SetValue(__instance, 0f);
        }

        [HarmonyPatch(typeof(MatchActivity), "Completed", typeof(int), typeof(bool))]
        [HarmonyPostfix]
        private static void CompletedPostfix(MatchActivity __instance)
        {
            RestoreBalloons();
            _originalSprites.Clear();

            var balloons = _balloonArrayField?.GetValue(__instance) as MatchActivityBalloon[];
            if (balloons != null)
            {
                foreach (var b in balloons)
                {
                    if (b != null)
                        SetBalloonAlpha(b, 1f);
                }
            }
            if (_balloonPopDelayField != null)
                _balloonPopDelayField.SetValue(__instance, 0f);
            if (_balloonPopRateField != null)
                _balloonPopRateField.SetValue(__instance, 0f);
        }
    }
}
