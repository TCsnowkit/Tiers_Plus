using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GadgetCore.API;
using System.Collections;
using GadgetCore.Util;

namespace TiersPlus
{
    public class PlasmaLanceItemInfo : ItemInfo
    {
        public PlasmaLanceItemInfo(ItemType Type, string Name, string Desc, Texture Tex, int Value = -1, EquipStats Stats = default, Texture HeldTex = null) :
            base(Type, Name, Desc, Tex, Value, Stats, HeldTex)
        { }

        /// <summary>
        /// Gets the amount of damage that this item will do on a crit. Returns 0 if WeaponScaling is null. Preserves the ID-specific behavior of the base game, so if the ItemInfo's ID matches the ID of a vanilla item, it will behave in the exact same way that the vanilla item of the same ID would.
        /// </summary>
        public override int MultiplyCrit(PlayerScript script, int dmg)
        {
            if (GameScript.debugMode) return 99999;
            float num = CritPowerBonus;
            return (int)(dmg * (1.5f + (PlayerGearModsTracker.GetGearMods(script)[12] * 0.10f) + num));
        }

        /// <summary>
        /// Mathematically attempts to trigger a critical attack. Returns true if a crit should occur.
        /// </summary>
        public override bool TryCrit(PlayerScript script)
        {
            float num = CritChanceBonus;
            if (Menuu.curUniform == 1)
            {
                num += 5;
            }
            return UnityEngine.Random.Range(0, 100) + (PlayerGearModsTracker.GetGearMods(script)[11] * 0.9f) + num >= 95f;
        }
    }
}
