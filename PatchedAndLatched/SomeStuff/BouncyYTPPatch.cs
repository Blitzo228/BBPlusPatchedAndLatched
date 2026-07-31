using HarmonyLib;
using TMPro;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public class BouncyYTPPatch
    {
        [HarmonyPatch(typeof(PointsAnimation), "AddScore")]
        [HarmonyPrefix]
        public static void AddScorePrefix()
        {
            var hud = Singleton<CoreGameManager>.Instance?.GetHud(0);
            if (hud?.PointsAnimator?.additionTmp != null)
            {
                TMP_Text additionTmp = hud.PointsAnimator.additionTmp;
                additionTmp.rectTransform.localScale = new Vector3(1.8f, 0.3f, additionTmp.rectTransform.localScale.z);
            }
        }
    }

    [HarmonyPatch(typeof(CoreGameManager), "Update")]
    public class BouncyYTPUpdatePatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            var hud = Singleton<CoreGameManager>.Instance?.GetHud(0);
            if (hud?.PointsAnimator?.gameObject != null && hud.PointsAnimator?.additionTmp != null)
            {
                TMP_Text additionTmp = hud.PointsAnimator.additionTmp;
                Vector3 scale = additionTmp.rectTransform.localScale;

                if (scale.x > 1f || scale.y < 1f)
                {
                    float x = scale.x - 0.1f * Time.timeScale;
                    float y = scale.y + 0.1f * Time.timeScale;

                    if (x <= 1f && y >= 1f)
                    {
                        additionTmp.rectTransform.localScale = new Vector3(1f, 1f, scale.z);
                    }
                    else
                    {
                        additionTmp.rectTransform.localScale = new Vector3(x, y, scale.z);
                    }
                }
            }
        }
    }
}
