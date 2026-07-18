using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public class LethalTouch : MonoBehaviour
    {
        private NPC? _npc;

        public void Initialize(NPC npc)
        {
            _npc = npc;
            transform.SetParent(npc.transform);
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (!other.CompareTag("Player")) return;
            if (_npc == null) return;

            Baldi? baldi = _npc.ec?.GetBaldi();
            if (baldi == null && _npc is Baldi b)
                baldi = b;

            if (baldi == null) return;

            var core = Singleton<CoreGameManager>.Instance;
            if (core != null)
                core.EndGame(other.transform, baldi);
        }
    }

    [HarmonyPatch(typeof(NPC), "Initialize")]
    internal static class LethalTouchPatch
    {
        [HarmonyPostfix]
        private static void AddLethalTouch(NPC __instance)
        {
            if (!PatchedAndLatchedPlugin.LethalTouchEnabled.Value) return;
            if (__instance.GetComponent<LethalTouch>() == null)
            {
                LethalTouch lethal = __instance.gameObject.AddComponent<LethalTouch>();
                lethal.Initialize(__instance);
            }
        }
    }
}