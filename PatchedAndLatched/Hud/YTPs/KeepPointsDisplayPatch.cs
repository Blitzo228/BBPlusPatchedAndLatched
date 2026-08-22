using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public class PointsDisplayKeeper : MonoBehaviour
    {
        private RectTransform? _rect;
        private Vector2 _startPos;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            if (_rect != null)
                _startPos = _rect.anchoredPosition;
        }

        private void LateUpdate()
        {
            if (_rect == null) return;
            if (_rect.anchoredPosition != _startPos)
                _rect.anchoredPosition = _startPos;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            var animator = GetComponent<Animator>();
            if (animator != null && !animator.GetBool("Adding"))
                animator.SetBool("Adding", true);
        }
    }

    [HarmonyPatch(typeof(PointsAnimation))]
    internal static class KeepPointsDisplayPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(PointsAnimation __instance)
        {
            if (__instance.GetComponent<PointsDisplayKeeper>() == null)
                __instance.gameObject.AddComponent<PointsDisplayKeeper>();
        }

        [HarmonyPatch("ShowDisplay")]
        [HarmonyPrefix]
        private static bool Prefix(bool val)
        {
            if (!val)
                return false;
            return true;
        }
    }
}