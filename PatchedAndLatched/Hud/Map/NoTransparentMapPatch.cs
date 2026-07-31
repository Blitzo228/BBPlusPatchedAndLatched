using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Map))]
    public static class NoTransparentMapPatch
    {
        private static Material? opaqueMat = null;

        private static void CreateOpaqueMaterial()
        {
            if (opaqueMat == null)
            {
                Shader shader = Shader.Find("Sprites/Default");

                opaqueMat = new Material(shader);

                opaqueMat.SetOverrideTag("RenderType", "Opaque");
                opaqueMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                opaqueMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                opaqueMat.SetInt("_ZWrite", 1);
                opaqueMat.DisableKeyword("_ALPHATEST_ON");
                opaqueMat.DisableKeyword("_ALPHABLEND_ON");
                opaqueMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                opaqueMat.renderQueue = -1;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Initialize")]
        public static void Initialize_Postfix(Map __instance)
        {
            CreateOpaqueMaterial();

            for (int i = 0; i < __instance.size.x; i++)
            {
                for (int j = 0; j < __instance.size.z; j++)
                {
                    MapTile tile = __instance.tiles[i, j];
                    if (tile != null)
                    {
                        var renderer = tile.SpriteRenderer;
                        if (renderer != null)
                        {
                            Color color = renderer.color;
                            color.a = 1f;

                            renderer.sharedMaterial = opaqueMat;
                            renderer.color = color;

                            if (__instance.foundTiles[i, j])
                            {
                                renderer.enabled = true;
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void Update_Postfix(Map __instance)
        {
            for (int i = 0; i < __instance.size.x; i++)
            {
                for (int j = 0; j < __instance.size.z; j++)
                {
                    MapTile tile = __instance.tiles[i, j];
                    if (tile != null)
                    {
                        var renderer = tile.SpriteRenderer;
                        if (renderer != null)
                        {
                            Color color = renderer.color;
                            if (color.a < 1f)
                            {
                                color.a = 1f;
                                renderer.color = color;
                            }

                            if (renderer.sharedMaterial != opaqueMat)
                            {
                                Color currentColor = renderer.color;
                                renderer.sharedMaterial = opaqueMat;
                                renderer.color = currentColor;
                            }

                            if (__instance.foundTiles[i, j])
                            {
                                renderer.enabled = true;
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Find")]
        public static void Find_Postfix(Map __instance, int posX, int posZ, int bin, RoomController room)
        {
            MapTile tile = __instance.tiles[posX, posZ];
            if (tile != null)
            {
                var renderer = tile.SpriteRenderer;
                if (renderer != null)
                {
                    Color color = renderer.color;
                    color.a = 1f;
                    renderer.color = color;
                    renderer.sharedMaterial = opaqueMat;
                    renderer.enabled = true;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MapTile), "Reveal")]
        public static void MapTile_Reveal_Postfix(MapTile __instance)
        {
            var renderer = __instance.SpriteRenderer;
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = 1f;
                renderer.color = color;

                if (opaqueMat != null)
                {
                    renderer.sharedMaterial = opaqueMat;
                }
                renderer.enabled = true;
            }
        }
    }
}
