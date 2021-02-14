using UnityEngine;
using GadgetCore.API;
using System.Collections;
using GadgetCore.Util;

namespace Plasmathrower
{
    [Gadget("Plasmathrower")]
    public class Plasmathrower : Gadget<Plasmathrower>
    {
        ItemInfo PlasmaCannon;
        ItemInfo MykonogreToken;
        ItemInfo FellbugToken;
        ItemInfo GladriaToken;
        PlasmaLanceItemInfo PlasmaLance;
        ItemInfo PlasmaArmor;
        ItemInfo PlasmaHelmet;
        ItemInfo PlasmaShield;
        public const string MOD_VERSION = "0.1"; // Set this to the version of your mod.
        public const string CONFIG_VERSION = "1.0"; // Increment this whenever you change your mod's config file.
		
		protected override void LoadConfig()
		{
			Config.Load();
			
			string fileVersion = Config.ReadString("ConfigVersion", CONFIG_VERSION, comments: "The Config Version (not to be confused with mod version)");

            if (fileVersion != CONFIG_VERSION)
            {
                Config.Reset();
                Config.WriteString("ConfigVersion", CONFIG_VERSION, comments: "The Config Version (not to be confused with mod version)");
            }
			
			// Do stuff with `Config`
			
			Config.Save();
		}
		
        public override string GetModDescription()
        {
            return "MMMMMMMMMMMMMMM Thro w Plasma"; // TODO: Change this
        }


        protected override void Initialize()
        {
            Logger.Log("Plasmathrower v" + Info.Mod.Version);
            PlasmaCannon = new ItemInfo(ItemType.WEAPON, "Plasmathrower", "", GadgetCoreAPI.LoadTexture2D("PlasmaCannonItem"), Stats: new EquipStats(20, 0, 25, 25, 0, 0), HeldTex: GadgetCoreAPI.LoadTexture2D("PlasmaCannonHeld"));
            PlasmaCannon.SetWeaponInfo(new float[] { 0.25f, 0, 0.5f, 0.5f, 0, 0 }, GadgetCoreAPI.GetAttackSound(497));
            PlasmaCannon.Register("PlasmaCannon");
            GameObject PlasmaCannonProj = Object.Instantiate(GadgetCoreAPI.GetWeaponProjectileResource(471));
            PlasmaCannonProj.GetComponentInChildren<ParticleSystemRenderer>().material = new Material(Shader.Find("Particles/Additive"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("CustomParticleTextureFileName"),
            };
            PlasmaCannonProj.GetComponentInChildren<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("PlasmaCannonProj"),
            };
            Vector3 particleSystemPos = PlasmaCannonProj.transform.Find("Particle System").localPosition;
            Object.DestroyImmediate(PlasmaCannonProj.transform.Find("Particle System").gameObject);
            GameObject customParticleSystem = Object.Instantiate(PlasmaCannonProj.GetComponent<Projectile>().particleAtalanta);
            customParticleSystem.name = "Particle System";
            customParticleSystem.transform.SetParent(PlasmaCannonProj.transform);
            customParticleSystem.transform.localPosition = particleSystemPos;
            customParticleSystem.SetActive(true);
            customParticleSystem.GetComponent<ParticleSystemRenderer>().material = new Material(Shader.Find("Particles/Additive"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("particleplasma"),
            };
            GadgetCoreAPI.AddCustomResource("proj/shot" + PlasmaCannon.GetID(), PlasmaCannonProj);
            

            PlasmaCannon.OnAttack += TripleShot;
            //plasmacannon
            PlasmaLance = new PlasmaLanceItemInfo(ItemType.WEAPON, "Plasma Lance", "", GadgetCoreAPI.LoadTexture2D("Plasmaspearitem"), Stats: new EquipStats(20, 40, 0, 0, 0, 0), HeldTex: GadgetCoreAPI.LoadTexture2D("PlasmaSpear"));
            PlasmaLance.SetWeaponInfo(new float[] { 2f, 4f, 0, 0, 0, 0 }, GadgetCoreAPI.GetAttackSound(367));
            PlasmaLance.Register("PlasmaLance");
            PlasmaLance.OnAttack += PlasmaLance.ThrustLance;
            //plasmalance
            PlasmaArmor = new ItemInfo(ItemType.ARMOR, "Plasmatic Armor", "", GadgetCoreAPI.LoadTexture2D("PlasmaArmor"), Stats: new EquipStats(15, 10, 20, 15, 20, 15), BodyTex: GadgetCoreAPI.LoadTexture2D("PlasmaArmorEquip"), ArmTex: GadgetCoreAPI.LoadTexture2D("PlasmaHand"));
            PlasmaArmor.Register("PlasmaArmor");
            PlasmaHelmet = new ItemInfo(ItemType.HELMET, "Plasmatic Helmet", "", GadgetCoreAPI.LoadTexture2D("PlasmaHelm"), Stats: new EquipStats(20, 15, 15, 20, 15, 20), HeadTex: GadgetCoreAPI.LoadTexture2D("PlasmaHelmEquip"));
            PlasmaHelmet.Register("PlasmaHelmet");
            PlasmaShield = new ItemInfo(ItemType.OFFHAND, "Plasmatic Shield", "", GadgetCoreAPI.LoadTexture2D("PlasmaShield"), Stats: new EquipStats(15, 15, 15, 15, 15, 15), HeldTex: GadgetCoreAPI.LoadTexture2D("PlasmaShield"));
            PlasmaShield.Register("PlasmaShield");
            //equipment
            MykonogreToken = new ItemInfo(ItemType.EMBLEM, "Mykonogre Token", "A token dropped from \n Mykonogre \n used to craft items at the universal crafter.", GadgetCoreAPI.LoadTexture2D("MykonogreToken"));
            MykonogreToken.Register("MykonogreToken");

            FellbugToken = new ItemInfo(ItemType.EMBLEM, "Fellbug Token", "A token dropped from \n Fellbug. \n Used to craft items at the universal crafter.", GadgetCoreAPI.LoadTexture2D("FellbugToken"));
            FellbugToken.Register("FellbugToken");

            GladriaToken = new ItemInfo(ItemType.EMBLEM, "Gladria Token", "A token dropped from \n Glaedria. \n used to craft items at the universal crafter.", GadgetCoreAPI.LoadTexture2D("GladriaToken"));
            GladriaToken.Register("GladriaToken");
            //tokens
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { MykonogreToken.GetID(), GladriaToken.GetID(), FellbugToken.GetID() }, new Item(PlasmaCannon.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { MykonogreToken.GetID(), FellbugToken.GetID(), GladriaToken.GetID() }, new Item(PlasmaLance.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { FellbugToken.GetID(),   GladriaToken.GetID(), MykonogreToken.GetID() }, new Item(PlasmaShield.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { GladriaToken.GetID(), MykonogreToken.GetID(), FellbugToken.GetID() }, new Item(PlasmaArmor.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { FellbugToken.GetID(), MykonogreToken.GetID(), GladriaToken.GetID() }, new Item(PlasmaHelmet.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            //crafting recipes
            MykonogreToken.AddToLootTable("entity:mykonogre", 1.0f, 1);
            FellbugToken.AddToLootTable("entity:fellbug", 1.0f, 1);
            GladriaToken.AddToLootTable("entity:glaedria", 1.0f, 1);
            //loot pools
            // TODO: Do stuff like registering items
        }
        IEnumerator TripleShot(PlayerScript script)
        {
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            yield return new WaitForSeconds(0.2f);
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            yield return new WaitForSeconds(0.2f);
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            yield return new WaitForSeconds(0.2f);
        }
    }
}
