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

            Baldi? baldi = null;
            if (_npc is Baldi)
                baldi = _npc as Baldi;
            else
            {
                Baldi[] baldies = Object.FindObjectsOfType<Baldi>();
                if (baldies.Length > 0)
                    baldi = baldies[0];
            }
            CoreGameManager.Instance.EndGame(other.transform, baldi);
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