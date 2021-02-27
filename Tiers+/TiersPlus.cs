using GadgetCore.API;
using GadgetCore.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using RecipeMenuCore;
using RecipeMenuCore.API;

namespace TiersPlus
{
    [Gadget("Tiers+", LoadPriority: 500)]
    public class TiersPlus : Gadget<TiersPlus>
    {
        private static readonly FieldInfo canAttack = typeof(PlayerScript).GetField("canAttack", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo attacking = typeof(PlayerScript).GetField("attacking", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo hyper = typeof(PlayerScript).GetField("hyper", BindingFlags.NonPublic | BindingFlags.Instance);
        public static ItemInfo NebulaCannon;
        ItemInfo PlasmaCannon;
        ItemInfo MykonogreToken;
        ItemInfo FellbugToken;
        ItemInfo GladriaToken;
        PlasmaLanceItemInfo PlasmaLance;
        ItemInfo PlasmaArmor;
        ItemInfo PlasmaHelmet;
        ItemInfo PlasmaShield;
        public const string MOD_VERSION = "1.0.6";
        public const string CONFIG_VERSION = "1.0.5";

        protected override void Initialize()
        {
            Logger.Log("Tiers+ v" + Info.Mod.Version);
            Logger.Log("Plasmathrower v" + Info.Mod.Version);
            PlasmaCannon = new ItemInfo(ItemType.WEAPON, "Plasmathrower", "", GadgetCoreAPI.LoadTexture2D("items/PlasmaCannonItem"), Stats: new EquipStats(5, 0, 10, 10, 0, 0), HeldTex: GadgetCoreAPI.LoadTexture2D("items/PlasmaCannonHeld"));
            PlasmaCannon.SetWeaponInfo(new float[] { 0, 0, 0.5f, 0.25f, 0, 0 }, GadgetCoreAPI.GetAttackSound(497));
            PlasmaCannon.Register("PlasmaCannon");
            GameObject PlasmaCannonProj = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetWeaponProjectileResource(471));
            PlasmaCannonProj.GetComponentInChildren<ParticleSystemRenderer>().material = new Material(Shader.Find("Particles/Additive"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("CustomParticleTextureFileName"),
            };
            PlasmaCannonProj.GetComponentInChildren<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("items/PlasmaCannonProj"),
            };
            Vector3 particleSystemPos = PlasmaCannonProj.transform.Find("Particle System").localPosition;
            UnityEngine.Object.DestroyImmediate(PlasmaCannonProj.transform.Find("Particle System").gameObject);
            GameObject customParticleSystem = UnityEngine.Object.Instantiate(PlasmaCannonProj.GetComponent<Projectile>().particleAtalanta);
            customParticleSystem.name = "Particle System";
            customParticleSystem.transform.SetParent(PlasmaCannonProj.transform);
            customParticleSystem.transform.localPosition = particleSystemPos;
            customParticleSystem.SetActive(true);
            customParticleSystem.GetComponent<ParticleSystemRenderer>().material = new Material(Shader.Find("Particles/Additive"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("items/particleplasma"),
            };
            GadgetCoreAPI.AddCustomResource("proj/shot" + PlasmaCannon.GetID(), PlasmaCannonProj);


            PlasmaCannon.OnAttack += TripleShot;
            //plasmacannon
            ItemInfo
            particleAccelerator = new ItemInfo(ItemType.WEAPON, "Particle Accelerator", "", GadgetCoreAPI.LoadTexture2D("items/ParticleAccItem"), Stats: new EquipStats(3, 0, 7, 0, 7, 3), HeldTex: GadgetCoreAPI.LoadTexture2D("items/ParticleAccHeld"));
            particleAccelerator.SetWeaponInfo(new float[] { 0, 0, 1.5f, 0, 1, 0 }, GadgetCoreAPI.GetAttackSound(426));
            particleAccelerator.Register("ParticleAcc");
            GameObject particleAccProj = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetWeaponProjectileResource(427));
            GadgetCoreAPI.AddCustomResource("proj/shot" + particleAccelerator.GetID(), particleAccProj);
            particleAccProj.GetComponent<Projectile>().speed = 45;
            particleAccelerator.OnAttack += particleAccelerator.ShootGun;

            NebulaCannon = new ItemInfo(ItemType.WEAPON, "Nebular Grenadier", "", GadgetCoreAPI.LoadTexture2D("items/NebulaCannonItem"), Stats: new EquipStats(0, 0, 10, 10, 0, 0), HeldTex: GadgetCoreAPI.LoadTexture2D("items/NebulaCannonHeld"));
            NebulaCannon.SetWeaponInfo(new float[] { 0, 0, 0.5f, 2, 0, 0 }, GadgetCoreAPI.GetAttackSound(473));
            NebulaCannon.Register("NebulaCannon");
            NebulaCannon.OnAttack += CustomPlasma;

            ItemInfo plantain = new ItemInfo(ItemType.WEAPON, "Plantain", "", GadgetCoreAPI.LoadTexture2D("items/PlantainItem"), Stats: new EquipStats(0, 0, 0, 0, 10, 7), HeldTex: GadgetCoreAPI.LoadTexture2D("items/PlantainHeld"));
            plantain.SetWeaponInfo(new float[] { 0, 0, 0, 0, 1.5f, 0.5f }, GadgetCoreAPI.GetAttackSound(529));
            plantain.Register("Plantain");
            GameObject plantainProj = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetWeaponProjectileResource(596));
            GadgetCoreAPI.AddCustomResource("proj/shot" + plantain.GetID(), plantainProj);
            plantainProj.GetComponent<Projectile>().speed = 45;
            plantainProj.GetComponentInChildren<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("items/PlantainItem"),
            };
            plantain.OnAttack += plantain.CastStaff;

            GameObject proj = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load("proj/wyvern"));
            proj.GetComponent<HazardScript>().damage = 40;
            GadgetCoreAPI.AddCustomResource("proj/wyvernCustom", proj);

            GameObject proj2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load("proj/spongeProj"));
            proj2.GetComponent<HazardScript>().damage = 45;
            GadgetCoreAPI.AddCustomResource("proj/spongeProjCustom", proj2);

            //projectiles
            PlasmaLance = new PlasmaLanceItemInfo(ItemType.WEAPON, "Plasma Lance", "", GadgetCoreAPI.LoadTexture2D("items/Plasmaspearitem"), Stats: new EquipStats(5, 20, 0, 0, 0, 0), HeldTex: GadgetCoreAPI.LoadTexture2D("items/PlasmaSpear"));
            PlasmaLance.SetWeaponInfo(new float[] { 2f, 4f, 0, 0, 0, 0 }, GadgetCoreAPI.GetAttackSound(367));
            PlasmaLance.Register("PlasmaLance");
            PlasmaLance.OnAttack += PlasmaLance.ThrustLance;
            //plasmalance
            PlasmaArmor = new ItemInfo(ItemType.ARMOR, "Plasmatic Armor", "", GadgetCoreAPI.LoadTexture2D("items/PlasmaArmor"), Stats: new EquipStats(7, 5, 8, 5, 80, 5), BodyTex: GadgetCoreAPI.LoadTexture2D("Items/PlasmaArmorEquip"), ArmTex: GadgetCoreAPI.LoadTexture2D("items/PlasmaHand"));
            PlasmaArmor.Register("PlasmaArmor");
            PlasmaHelmet = new ItemInfo(ItemType.HELMET, "Plasmatic Helmet", "", GadgetCoreAPI.LoadTexture2D("items/PlasmaHelm"), Stats: new EquipStats(8, 5, 5, 8, 5, 8), HeadTex: GadgetCoreAPI.LoadTexture2D("items/PlasmaHelmEquip"));
            PlasmaHelmet.Register("PlasmaHelmet");
            ItemInfo NexusArmor = new ItemInfo(ItemType.ARMOR, "Nexus Armor", "", GadgetCoreAPI.LoadTexture2D("items/NexusArmor"), Stats: new EquipStats(6, 5, 5, 5, 7, 7), BodyTex: GadgetCoreAPI.LoadTexture2D("Items/NexusArmor"), ArmTex: GadgetCoreAPI.LoadTexture2D("items/NexusHand"));
            NexusArmor.Register("NexusArmor");
            ItemInfo NexusHelmet = new ItemInfo(ItemType.HELMET, "Nexus Helmet", "", GadgetCoreAPI.LoadTexture2D("items/NexusHelmItem"), Stats: new EquipStats(4, 4, 4, 4, 10, 10), HeadTex: GadgetCoreAPI.LoadTexture2D("items/NexusHelm"));
            NexusHelmet.Register("NexusHelmet");
            PlasmaShield = new ItemInfo(ItemType.OFFHAND, "Nexus Shield", "", GadgetCoreAPI.LoadTexture2D("items/NexusShield"), Stats: new EquipStats(5, 3, 3, 3, 3, 8), HeldTex: GadgetCoreAPI.LoadTexture2D("items/NexusShield"));
            PlasmaShield.Register("NexusShield");
            ItemInfo NexusShield = new ItemInfo(ItemType.OFFHAND, "Plasmatic Shield", "", GadgetCoreAPI.LoadTexture2D("items/PlasmaShield"), Stats: new EquipStats(7, 5, 5, 5, 5, 5), HeldTex: GadgetCoreAPI.LoadTexture2D("items/PlasmaShield"));
            NexusShield.Register("Plasmahield");
            //equipment
            ItemInfo SliverCard = new ItemInfo(ItemType.GENERIC, "Sliver Card", "A rare card \nCan be placed \nIn your ship", GadgetCoreAPI.LoadTexture2D("Cards/SliverCard")).Register("Slivercard");
            GameObject SliverCardPlaced = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetPropResource(2518));
            TileInfo SliverCardTile = new TileInfo(TileType.NONSOLID, GadgetCoreAPI.LoadTexture2D("Cards/SliverCardPlaced"), SliverCardPlaced, SliverCard).Register("SliverCardPlaced");
            SliverCardPlaced.GetComponentInChildren<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("Cards/SliverCardPlaced"),
            };
            ItemInfo GhostCard = new ItemInfo(ItemType.GENERIC, "Ghostshroom Card", "A rare card \nCan be placed \nIn your ship", GadgetCoreAPI.LoadTexture2D("Cards/GhostShroomCard")).Register("Ghostcard");
            GameObject GhostCardPlaced = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetPropResource(2518));
            TileInfo GhostCardTile = new TileInfo(TileType.NONSOLID, GadgetCoreAPI.LoadTexture2D("Cards/GhostShroomCardPlaced"), GhostCardPlaced, GhostCard).Register("GhostCardPlaced");
            GhostCardPlaced.GetComponentInChildren<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("Cards/GhostShroomCardPlaced"),
            };
            ItemInfo GuardianCard = new ItemInfo(ItemType.GENERIC, "Guardian Card", "A rare card \nCan be placed \nIn your ship", GadgetCoreAPI.LoadTexture2D("Cards/GuardianCard")).Register("Guardcard");
            GameObject GuardianCardPlaced = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetPropResource(2518));
            TileInfo GuardianCardTile = new TileInfo(TileType.NONSOLID, GadgetCoreAPI.LoadTexture2D("Cards/GuardianCardPlaced"), GuardianCardPlaced, GuardianCard).Register("GuardianCardPlaced");
            GuardianCardPlaced.GetComponentInChildren<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("Cards/GuardianCardPlaced"),
            };
            ItemInfo BlankCard = new ItemInfo(ItemType.GENERIC, "Blank Card", "A rare card \nCard Mimic.", GadgetCoreAPI.LoadTexture2D("Cards/BlankCard")).Register("Blankcard");
            GameObject BlankCardPlaced = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetPropResource(2518));
            TileInfo BlankCardTile = new TileInfo(TileType.NONSOLID, GadgetCoreAPI.LoadTexture2D("Cards/BlankCardPlaced"), BlankCardPlaced, BlankCard).Register("BlankCardPlaced");
            BlankCardPlaced.GetComponentInChildren<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("Cards/BlankCardPlaced"),
            };
            //cards
            MykonogreToken = new ItemInfo(ItemType.EMBLEM, "Mykonogre Token", "A token dropped from \n Mykonogre \n used to craft items at the universal crafter.", GadgetCoreAPI.LoadTexture2D("MykonogreToken"));
            MykonogreToken.Register("MykonogreToken");

            FellbugToken = new ItemInfo(ItemType.EMBLEM, "Fellbug Token", "A token dropped from \n Fellbug. \n Used to craft items at the universal crafter.", GadgetCoreAPI.LoadTexture2D("FellbugToken"));
            FellbugToken.Register("FellbugToken");

            GladriaToken = new ItemInfo(ItemType.EMBLEM, "Gladria Token", "A token dropped from \n Glaedria. \n used to craft items at the universal crafter.", GadgetCoreAPI.LoadTexture2D("GladriaToken"));
            GladriaToken.Register("GladriaToken");
            //tokens
            //FellbugToken.AddToLootTable("entity:fellbug", 1.0f, 1);
            //GladriaToken.AddToLootTable("entity:glaedria", 1.0f, 1);
            //loot pools
            //plasmathrower
            ItemInfo energiteItem = new ItemInfo(ItemType.LOOT | ItemType.ORE | ItemType.TIER7, "Energite", "LOOT - ORE\nTIER: 7",
                GadgetCoreAPI.LoadTexture2D("Items/Energite")).Register(7);
            ItemInfo plasmaFernItem = new ItemInfo(ItemType.LOOT | ItemType.PLANT | ItemType.TIER7, "Plasma Fern", "LOOT - PLANT\nTIER: 7",
                GadgetCoreAPI.LoadTexture2D("Items/PlasmaFern")).Register(17);
            ItemInfo powerCrystalItem = new ItemInfo(ItemType.LOOT | ItemType.MONSTER | ItemType.TIER7, "Power Crystal", "LOOT - MONSTER PART\nTIER: 7",
                GadgetCoreAPI.LoadTexture2D("Items/PowerCrystal")).Register(27);
            ItemInfo lightingBugItem = new ItemInfo(ItemType.LOOT | ItemType.BUG | ItemType.TIER7, "Lightning Bug", "LOOT - BUG\nTIER: 7",
                GadgetCoreAPI.LoadTexture2D("Items/LightningBug")).Register(37);

            //Resin
            //Needle
            //Worm Tooth
            //Bookworm Larva


            ItemInfo energiteEmblemItem = new ItemInfo(ItemType.EMBLEM | ItemType.ORE | ItemType.TIER7, "Energite Emblem", "Tier 7.\nA shiny Token. Used\nto forge items.",
                GadgetCoreAPI.LoadTexture2D("Items/EnergiteEmblem")).Register(107);
            ItemInfo fernEmblemItem = new ItemInfo(ItemType.EMBLEM | ItemType.PLANT | ItemType.TIER7, "Fern Emblem", "Tier 7.\nA shiny Token. Used\nto forge items.",
                GadgetCoreAPI.LoadTexture2D("Items/FernEmblem")).Register(117);
            ItemInfo powerEmblemItem = new ItemInfo(ItemType.EMBLEM | ItemType.MONSTER | ItemType.TIER7, "Power Emblem", "Tier 7.\nA shiny Token. Used\nto forge items.",
                GadgetCoreAPI.LoadTexture2D("Items/PowerEmblem")).Register(127);
            ItemInfo lightingEmblemItem = new ItemInfo(ItemType.EMBLEM | ItemType.BUG | ItemType.TIER7, "Lightning Emblem", "Tier 7.\nA shiny Token. Used\nto forge items.",
                GadgetCoreAPI.LoadTexture2D("Items/LightningEmblem")).Register(137);

            ItemInfo plasmaTracerItem = new ItemInfo(ItemType.CONSUMABLE, "Plasma Tracer", "Grants 3 portal uses to\nThe Plasma Zone.",
                GadgetCoreAPI.LoadTexture2D("Items/PlasmaTracer"), 32).Register();

            ItemInfo healthPack4Item = new ItemInfo(ItemType.CONSUMABLE, "Health Pack IV", "Restores 18 Health.",
                GadgetCoreAPI.LoadTexture2D("Items/HealthPack4")).Register();

            ItemInfo TydusRing = new ItemInfo(ItemType.RING, "Tydus Ring", "", GadgetCoreAPI.GetItemMaterial(909), Stats: new EquipStats(0,0,3,3,0,0)).Register(909);
            ItemInfo OwainPearl = new ItemInfo(ItemType.RING, "Owain's Pearl", "", GadgetCoreAPI.LoadTexture2D("Items/OwainPearl"), Stats: new EquipStats(2, 1, 1, 1, 1, 1)).Register("OwainRing", 908);
            ItemInfo VaatiBadge = new ItemInfo(ItemType.RING, "Vaati Badge", "", GadgetCoreAPI.GetItemMaterial(910), Stats: new EquipStats(2, 0, 2, 0, 2, 1)).Register(910);
            
            healthPack4Item.OnUse += (slot) =>
            {
                InstanceTracker.GameScript.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/drink"), Menuu.soundLevel / 10f);
                InstanceTracker.GameScript.RecoverHP(18);
                return true;
            };
            ItemInfo manaPack4Item = new ItemInfo(ItemType.CONSUMABLE, "Mana Pack IV", "Restores 50 Mana.",
                GadgetCoreAPI.LoadTexture2D("Items/ManaPack4")).Register();
            manaPack4Item.OnUse += (slot) =>
            {
                InstanceTracker.GameScript.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/drink"), Menuu.soundLevel / 10f);
                InstanceTracker.GameScript.RecoverMana(50);
                return true;
            };
            ItemInfo energyPack4Item = new ItemInfo(ItemType.CONSUMABLE, "Energy Pack IV", "Restores 100 Stamina.\nGrants a temporary\nspeed boost.",
                GadgetCoreAPI.LoadTexture2D("Items/EnergyPack4")).Register();
            energyPack4Item.OnUse += (slot) =>
            {
                InstanceTracker.GameScript.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/drink"), Menuu.soundLevel / 10f);
                InstanceTracker.GameScript.RecoverStamina(100);
                InstanceTracker.GameScript.StartCoroutine(EnergyPackSpeedBoost(15, 10));
                return true;
            };

            GadgetCoreAPI.AddAlchemyStationRecipe(Tuple.Create(plasmaFernItem.GetID(), powerCrystalItem.GetID(), lightingBugItem.GetID()), new Item(healthPack4Item.GetID(), 1, 0, 0, 0, new int[3], new int[3]), 2);
            GadgetCoreAPI.AddAlchemyStationRecipe(Tuple.Create(plasmaFernItem.GetID(), lightingBugItem.GetID(), powerCrystalItem.GetID()), new Item(manaPack4Item.GetID(), 1, 0, 0, 0, new int[3], new int[3]), 2);
            GadgetCoreAPI.AddAlchemyStationRecipe(Tuple.Create(lightingBugItem.GetID(), plasmaFernItem.GetID(), powerCrystalItem.GetID()), new Item(energyPack4Item.GetID(), 1, 0, 0, 0, new int[3], new int[3]), 2);
            GadgetCoreAPI.AddAlchemyStationRecipe(Tuple.Create(lightingBugItem.GetID(), powerCrystalItem.GetID(), plasmaFernItem.GetID()), new Item(63, 4, 0, 0, 0, new int[3], new int[3]), 3);

            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(
                Tuple.Create(new int[] { 136, 74, 136 }, new Item(plasmaTracerItem.GetID(), 1, 0, 0, 0, new int[3], new int[3]), 0)
            ));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { fernEmblemItem.GetID(), powerEmblemItem.GetID(), energiteEmblemItem.GetID() }, new Item(PlasmaCannon.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { energiteEmblemItem.GetID(), fernEmblemItem.GetID(), lightingEmblemItem.GetID() }, new Item(PlasmaShield.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { energiteEmblemItem.GetID(), powerEmblemItem.GetID(), fernEmblemItem.GetID() }, new Item(PlasmaLance.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { energiteEmblemItem.GetID(), lightingEmblemItem.GetID(), fernEmblemItem.GetID() }, new Item(PlasmaArmor.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { fernEmblemItem.GetID(), lightingEmblemItem.GetID(), energiteEmblemItem.GetID() }, new Item(PlasmaHelmet.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { powerEmblemItem.GetID(), lightingEmblemItem.GetID(), energiteEmblemItem.GetID() }, new Item(NebulaCannon.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            ((CraftMenuInfo)MenuRegistry.Singleton["Gadget Core:Crafter Menu"]).AddCraftPerformer(CraftMenuInfo.CreateSimpleCraftPerformer(Tuple.Create(new int[] { fernEmblemItem.GetID(), energiteEmblemItem.GetID(), powerEmblemItem.GetID() }, new Item(particleAccelerator.GetID(), 1, 0, 3, 0, new int[3], new int[3]), 0)));
            GadgetCoreAPI.AddCreationMachineRecipe(powerEmblemItem.GetID(), new Item(1030, 1, 0, 0, 0, new int[3], new int[3]));
            GadgetCoreAPI.AddCreationMachineRecipe(energiteEmblemItem.GetID(), new Item(909, 1, 0, 0, 0, new int[3], new int[3]));
            GadgetCoreAPI.AddCreationMachineRecipe(fernEmblemItem.GetID(), new Item(908, 1, 0, 0, 0, new int[3], new int[3]));
            GadgetCoreAPI.AddCreationMachineRecipe(lightingEmblemItem.GetID(), new Item(910, 1, 0, 0, 0, new int[3], new int[3]));
            GadgetCoreAPI.AddAlchemyStationRecipe(Tuple.Create(21, 31, 11), new Item(64, 1, 0, 0, 0, new int[3], new int[3]), 3);
            GadgetCoreAPI.AddAlchemyStationRecipe(Tuple.Create(32, 22, 12), new Item(68, 1, 0, 0, 0, new int[3], new int[3]), 3);
            GadgetCoreAPI.AddAlchemyStationRecipe(Tuple.Create(33, 23, 13), new Item(69, 1, 0, 0, 0, new int[3], new int[3]), 3);
            GadgetCoreAPI.AddAlchemyStationRecipe(Tuple.Create(34, 24, 14), new Item(63, 1, 0, 0, 0, new int[3], new int[3]), 3);
            //crafting recipes

            ObjectInfo EnergiteOre = new ObjectInfo(ObjectType.ORE, new Item(energiteItem.GetID(), 1, 0, 0, 0, new int[3], new int[3]), 1, GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Energite")).Register("TestObject");
            ObjectInfo PlasmaFern = new ObjectInfo(ObjectType.PLANT, new Item(plasmaFernItem.GetID(), 1, 0, 0, 0, new int[3], new int[3]), 1, GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Fern")).Register("PlasmaFern");
            ObjectInfo LightningBugTree = new ObjectInfo(ObjectType.BUGSPOT, new Item(lightingBugItem.GetID(), 1, 0, 0, 0, new int[3], new int[3]), 1, GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/LightningBugNode"), GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/LightningBugBody"), GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/LightningBugWing")).Register("LightningBugTree");
            GameObject ParticleDragon = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetEntityResource("wyvern"));
            ParticleDragon.name = "particleWyvern";
            UnityEngine.Object.Destroy(ParticleDragon.GetComponent<WyvernScript>());
            ParticleDragon.AddComponent<ParticleWyvernScript>();
            ParticleDragon.transform.Find("e").Find("wyvern").Find("Plane").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/particlewyvernhead"),
            };
            ParticleDragon.transform.Find("e").Find("wyvern").Find("Plane_001").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/particlewyvernbody"),
            };
            ParticleDragon.transform.Find("e").Find("wyvern").Find("Plane_002").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/particlewyverntail"),
            };
            ParticleDragon.transform.Find("e").Find("wyvern").Find("Plane_003").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/particlewyvernwing"),
            };
            ParticleDragon.transform.Find("e").Find("wyvern").Find("Plane_004").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/particlewyvernwing"),
            };
            EntityInfo ParticleWyvern = new EntityInfo(EntityType.COMMON, ParticleDragon).Register("ParticleWyvern");
            powerCrystalItem.AddToLootTable("entity:particleWyvern", 1.0f, 0, 4);

            GameObject MykBug = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetEntityResource("spider"));
            MykBug.name = "Mykdunebug";
            SpiderScript spiderScript = MykBug.GetComponent<SpiderScript>();
            GameObject mykBugHead = spiderScript.head;
            GameObject mykBugBody = spiderScript.body;
            GameObject mykBugLeg = spiderScript.leg;
            GameObject mykBugB = spiderScript.b;
            UnityEngine.Object.Destroy(spiderScript);
            MykBugScript mykBugScript = MykBug.AddComponent<MykBugScript>();
            mykBugScript.head = mykBugHead;
            mykBugScript.body = mykBugBody;
            mykBugScript.leg = mykBugLeg;
            mykBugScript.b = mykBugB;
            MykBug.transform.Find("e").Find("dunebug").Find("Plane").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/MykDunebug/Head"),
            };
            MykBug.transform.Find("e").Find("dunebug").Find("Plane_001").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/MykDunebug/Body"),
            };
            MykBug.transform.Find("e").Find("dunebug").Find("Plane_002").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/MykDunebug/Back"),
            };
            MykBug.transform.Find("e").Find("dunebug").Find("Plane_003").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/MykDunebug/Legs"),
            };
            MykBug.transform.Find("e").Find("dunebug").Find("Plane_004").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/MykDunebug/Wings"),
            };
            MykBug.transform.Find("e").Find("dunebug").Find("Plane_005").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Transparent Cutout"))
            {
                mainTexture = GadgetCoreAPI.LoadTexture2D("enemies/MykDunebug/Wings"),
            };
            EntityInfo MykDunebug = new EntityInfo(EntityType.COMMON, MykBug).Register("Mykdunebug");

            GameObject testProjEnemy = UnityEngine.Object.Instantiate(GadgetCoreAPI.GetEntityResource("sponge"));
            testProjEnemy.name = "testProjEnemy";
            testProjEnemy.SetActive(false);
            testProjEnemy.ReplaceComponent<SpongeScript, TestProjScript>();
            EntityInfo TestProjEnemy = new EntityInfo(EntityType.COMMON, testProjEnemy).Register("testProjEnemy");
            manaPack4Item.AddToLootTable("entity:testProjEnemy", 0.5f, 0, 2);
            powerCrystalItem.AddToLootTable("entity:testProjEnemy", 0.5f, 0, 4);


            GameObject lavadragonPrefab = GadgetCoreAPI.GetEntityResource("lavadragon");
            lavadragonPrefab.SetActive(false);
            GameObject bigPlasmaDragon = UnityEngine.Object.Instantiate(lavadragonPrefab);
            bigPlasmaDragon.name = "bigplasmadragon";
            foreach (Millipede m in bigPlasmaDragon.GetComponentsInChildren<Millipede>())
            {
            //Transform part = m.transform;

            m.gameObject.ReplaceComponent<Millipede, PlasmaDragonScript>();
            //part.gameObject.AddComponent<PlasmaDragonScript>().isMainHead = part.name == "0";
            }
            EntityInfo bigPlasmaDerg = new EntityInfo(EntityType.BOSS, bigPlasmaDragon).Register("bigplasmadragon");


            PlanetInfo plasmaZonePlanet = new PlanetInfo(PlanetType.NORMAL, "Plasmatic Rift", new Tuple<int, int>[] { Tuple.Create(-1, 1), Tuple.Create(13, 1) }, GadgetCoreAPI.LoadAudioClip("Planets/Plasma Zone/Music"));
            plasmaZonePlanet.SetTerrainInfo(GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Entrance"), GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Zone"),
                GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/MidChunkFull"), GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/MidChunkOpen"),
                GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/SideH"), GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/SideV"));
            plasmaZonePlanet.SetBackgroundInfo(GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Parallax"),
                GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Background0"), GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Background1"),
                GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Background2"), GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Background3"));
            plasmaZonePlanet.SetPortalInfo(GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Sign"), GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Button"),
                GadgetCoreAPI.LoadTexture2D("Planets/Plasma Zone/Icon"));
            plasmaZonePlanet.Register("Plasma Zone");
            plasmaZonePlanet.AddWeightedWorldSpawn(EnergiteOre, 15);
            plasmaZonePlanet.AddWeightedWorldSpawn(ParticleWyvern, 25);
            plasmaZonePlanet.AddWeightedWorldSpawn(LightningBugTree, 15);
            plasmaZonePlanet.AddWeightedWorldSpawn(PlasmaFern, 15);
            plasmaZonePlanet.AddWeightedWorldSpawn(TestProjEnemy, 15);
            plasmaZonePlanet.AddWeightedWorldSpawn(bigPlasmaDerg, 10);
            plasmaZonePlanet.AddWeightedWorldSpawn("obj/chest", 20);
            plasmaZonePlanet.AddWeightedWorldSpawn("obj/chestGold", 5);
            plasmaZonePlanet.AddWeightedTownSpawn("obj/chest", 25);
            plasmaZonePlanet.AddWeightedTownSpawn("obj/itemStand", 25);
            plasmaZonePlanet.AddWeightedTownSpawn("obj/chipStand", 25);
            plasmaZonePlanet.AddWeightedWorldSpawn((pos) => (GameObject)null, 2);
            plasmaZonePlanet.AddWeightedTownSpawn((pos) => (GameObject)null, 25);
            plasmaZonePlanet.AddWeightedWorldSpawn("obj/relic", 5);
            //plasmaZonePlanet.AddWeightedWorldSpawn();
            PlanetRegistry.SetVanillaExitPortalWeight(13, plasmaZonePlanet.GetID(), 75);

            PlanetInfo MykPlanet = new PlanetInfo(PlanetType.NORMAL, "Mykonogre's Zone", new Tuple<int, int>[] { Tuple.Create(-1, 1) }, GadgetCoreAPI.LoadAudioClip("Planets/Plasma Zone/Music"));
            MykPlanet.SetTerrainInfo(GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/Entrance"), GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/Zone"),
                GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/MidChunkFull"), GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/MidChunkOpen"),
                GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/SideH"), GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/SideV"));
            MykPlanet.SetBackgroundInfo(GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/Parallax"),
                GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/bg0"), GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/bg1"),
                GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/bg2"), GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/bg3"));
            MykPlanet.SetPortalInfo(GadgetCoreAPI.LoadTexture2D("Planets/Mykworld/MykSign"), GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/Button"),
                GadgetCoreAPI.LoadTexture2D("Planets/MykWorld/Planet"));
            MykPlanet.Register("Mykonogre Zone");
            MykPlanet.AddWeightedWorldSpawn(MykDunebug, 1);

            plasmaTracerItem.OnUse += (slot) =>
            {
                plasmaZonePlanet.PortalUses += 3;
                InstanceTracker.GameScript.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/glitter"), Menuu.soundLevel / 10f);
                return true;
            };
            BlankCard.AddToLootTable("entity:all", 0.00013f, 1);
            SliverCard.AddToLootTable("entity:sliver", 0.013f, 1);
            GuardianCard.AddToLootTable("entity:guardian", 0.013f, 1);
            GhostCard.AddToLootTable("entity:shroomy1", 0.013f, 1);
            //MykonogreToken.AddToLootTable("entity:mykonogre", 1.0f, 1, CustomDropBehavior: (item, pos) => {
            //MykPlanet.PortalUses += 3;
            //return true;
            //});
            var recAlchemy = new RecipePage(RecipePageType.AlchemyStation, "Stamina/Misc. Recipes", GadgetCoreAPI.LoadTexture2D("recipesAlch.png")).RegisterAsVanilla(1);
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(21, 11, 31, 62, 1, 2));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(22, 12, 32, 62, 4, 3));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(33, 13, 23, 67, 1, 2));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(34, 14, 24, 67, 4, 3));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(35, 15, 25, 72, 1, 2));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(26, 36, 16, 72, 4, 3));

            recAlchemy.AddRecipePageEntry(new RecipePageEntry(21, 31, 11, 64, 1, 2));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(32, 32, 12, 68, 1, 2));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(33, 23, 13, 69, 1, 2));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(34, 24, 14, 63, 1, 2)); // Nuldmg
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(35, 25, 15, 73, 1, 2));
            recAlchemy.AddRecipePageEntry(new RecipePageEntry(36, 16, 26, 74, 1, 2));
        }

        public static IEnumerator CustomPlasma(PlayerScript script)
        {
            canAttack.SetValue(script, false);
            attacking.SetValue(script, true);
            script.StartCoroutine(script.ATKSOUND());
            script.StartCoroutine(script.GunEffects(NebulaCannon.GetID()));
            script.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/shoot"), Menuu.soundLevel / 10f);
            script.Animate(4);
            yield return new WaitForSeconds(0.3f);
            int dmg = NebulaCannon.GetDamage(script);
            if ((bool)hyper.GetValue(script))
            {
                hyper.SetValue(script, false);
                script.HyperBeam();
            }
            if (NebulaCannon.TryCrit(script))
            {
                dmg = NebulaCannon.MultiplyCrit(script, dmg);
                script.GetComponent<AudioSource>().PlayOneShot(script.critSound, Menuu.soundLevel / 10f);
                UnityEngine.Object.Instantiate(script.crit, script.transform.position, Quaternion.identity);
            }

            script.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/plasma1"), Menuu.soundLevel / 10f);
            int num = InstanceTracker.GameScript.GetFinalStat(3) * 2 + InstanceTracker.GameScript.GetFinalStat(2) / 2;
            Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PackPlasma value = new PackPlasma(num, vector);
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("proj/plasma"), script.shot.transform.position, Quaternion.identity);
            gameObject.SendMessage("Plasma2", value);
            script.GetComponent<NetworkView>().RPC("ShootSpecial", RPCMode.Others, new object[]
            {
        1001,
        vector,
        num
            });

            yield return new WaitForSeconds(0.2f);
            attacking.SetValue(script, false);
            yield return new WaitForSeconds(0.1f);
            canAttack.SetValue(script, true);
            yield break;

        }

        private IEnumerator EnergyPackSpeedBoost(int power, float duration)
        {
            GameScript.MODS[16] += power;
            GameScript.MODS[17] += power;
            GameScript.MODS[18] += power;
            int boostedMoveModCount = GameScript.MODS[16];
            int boostedDashModCount = GameScript.MODS[17];
            int boostedJumpModCount = GameScript.MODS[18];
            yield return new WaitForSeconds(duration);
            if (GameScript.MODS[16] == boostedMoveModCount) GameScript.MODS[16] -= power;
            if (GameScript.MODS[17] == boostedDashModCount) GameScript.MODS[17] -= power;
            if (GameScript.MODS[18] == boostedJumpModCount) GameScript.MODS[18] -= power;
            yield break;
        }
        IEnumerator TripleShot(PlayerScript script)
        {
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            yield return new WaitForSeconds(0.2f);
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            yield return new WaitForSeconds(0.2f);
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            script.StartCoroutine(PlasmaCannon.ShootGun(script));
            yield return new WaitForSeconds(0.2f);
        }
    }
}