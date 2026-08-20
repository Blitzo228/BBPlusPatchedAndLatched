using HarmonyLib;
using System.Collections.Generic;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    public static class DoubleStorePricesPatch
    {
        [HarmonyPatch(typeof(StoreRoomFunction), nameof(StoreRoomFunction.Initialize))]
        [HarmonyPostfix]
        private static void Initialize_Postfix(StoreRoomFunction __instance)
        {
            var traverse = Traverse.Create(__instance);

            int currentRestock = traverse.Field("restockPrice").GetValue<int>();
            traverse.Field("restockPrice").SetValue(currentRestock * 2);

            var mapPickup = traverse.Field("mapPickup").GetValue<Pickup>();
            var mapTag = traverse.Field("mapTag").GetValue<PriceTag>();
            if (mapPickup != null && mapPickup.gameObject.activeSelf && mapPickup.price > 0)
            {
                mapPickup.price *= 2;
                mapTag.SetText(mapPickup.price.ToString());
            }

            var glueStickPickup = traverse.Field("glueStickPickup").GetValue<Pickup>();
            var glueStickTag = traverse.Field("glueStickTag").GetValue<PriceTag>();
            if (glueStickPickup != null && glueStickPickup.gameObject.activeSelf && glueStickPickup.price > 0)
            {
                glueStickPickup.price *= 2;
                glueStickTag.SetText(glueStickPickup.price.ToString());
            }
        }

        [HarmonyPatch(typeof(StoreRoomFunction), "Restock")]
        [HarmonyPostfix]
        private static void Restock_Postfix(StoreRoomFunction __instance)
        {
            var traverse = Traverse.Create(__instance);
            var pickups = traverse.Field("pickups").GetValue<List<Pickup>>();
            var tags = traverse.Field("tag").GetValue<PriceTag[]>();
            var stickerPickups = traverse.Field("stickerPickup").GetValue<Pickup[]>();
            var stickerTags = traverse.Field("stickerTag").GetValue<PriceTag[]>();

            if (pickups != null && tags != null)
            {
                for (int i = 0; i < pickups.Count; i++)
                {
                    if (pickups[i] != null && pickups[i].gameObject.activeSelf)
                    {
                        pickups[i].price *= 2;

                        if (pickups[i].item != null)
                        {
                            if (pickups[i].price != pickups[i].item.price * 2)
                            {
                                tags[i].SetSale(pickups[i].item.price * 2, pickups[i].price);
                            }
                            else
                            {
                                tags[i].SetText(pickups[i].price.ToString());
                            }
                        }
                    }
                }
            }

            if (stickerPickups != null && stickerTags != null)
            {
                for (int j = 0; j < stickerPickups.Length; j++)
                {
                    if (stickerPickups[j] != null && stickerPickups[j].gameObject.activeSelf)
                    {
                        stickerPickups[j].price *= 2;
                        stickerTags[j].SetText(stickerPickups[j].price.ToString());
                    }
                }
            }
        }

        [HarmonyPatch(typeof(StoreRoomFunction), "ItemPurchased")]
        [HarmonyPostfix]
        private static void ItemPurchased_Postfix(StoreRoomFunction __instance)
        {
            var traverse = Traverse.Create(__instance);
            bool tutorialMode = traverse.Field("tutorialMode").GetValue<bool>();
            bool tutorialStickersSpawned = traverse.Field("tutorialStickersSpawned").GetValue<bool>();

            if (tutorialMode && tutorialStickersSpawned)
            {
                var stickerPickups = traverse.Field("stickerPickup").GetValue<Pickup[]>();
                var stickerTags = traverse.Field("stickerTag").GetValue<PriceTag[]>();

                if (stickerPickups != null && stickerTags != null)
                {
                    for (int i = 0; i < stickerPickups.Length; i++)
                    {
                        if (stickerPickups[i] != null && stickerPickups[i].gameObject.activeSelf && stickerPickups[i].price == 250)
                        {
                            stickerPickups[i].price = 500;
                            stickerTags[i].SetText("500");
                        }
                    }
                }
            }
        }
    }
}
