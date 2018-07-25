using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using Harmony;
using Harmony.ILCopying;
using IV;
using System.Collections;
using System.Reflection;

[StaticConstructorOnStartup]
class Main
{
    static Main()
    {
        var harmony = HarmonyInstance.Create("com.medical.iv");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}

[HarmonyPatch(typeof(HediffSet), "CalculateBleedRate")]
static class BleedPatch
{
    static void Postfix(ref float __result, HediffSet __instance)
    {
        __result *= __instance.pawn.GetStatValue(StatDef.Named("BleedRate"));
    }
}

[HarmonyPatch(typeof(Pawn))]
[HarmonyPatch("CurrentlyUsableForBills")]
static class SurgeryBedPatch
{
    static void Postfix(ref bool __result)
    {
        __result = true;
    }
}

namespace IV
{
    public class Building_Bypass : Building
    {
        public static HediffDef IV_Bypass = HediffDef.Named("IV_Bypass");
        private CompPowerTrader powerComp = null;

        public Building_Bypass()
            : base()
        {
            Log.Message("[Medical Overhaul] Building Initialised");
        }

        // SpawnSetup and checking power sources
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = base.GetComp<CompPowerTrader>();
        }

        // Begin ticking 
        public override void Tick()
        {
            if ((Find.TickManager.TicksGame + thingIDNumber.HashOffset()) % 60 == 0)
            {
                base.Tick();

                if (powerComp.PowerOn)
                {
                    CheckPawnHealth();
                }
            }
        }

        // Get the Pawn(s) in the adjacent cells and retrieve their health stats
        public void CheckPawnHealth()
        {
            var adjacent = GenAdj.CardinalDirectionsAround;
            var position = this.Position;
            for (int i = 0; i < adjacent.Length; i++)
            {
                var things = this.Map.thingGrid.ThingsListAt(adjacent[i] + position);
                foreach (Thing thing in things)
                {
                    if (thing is Pawn)
                    {
                        Pawn pawn = thing as Pawn;
                        var BloodPumpingStat = pawn.health.capacities.GetLevel(PawnCapacityDefOf.BloodPumping);
                        var BreathingStat = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Breathing);

                        ApplyHediff(BloodPumpingStat, BreathingStat, pawn);
                    }
                }
            }
        }

        // Check the pawn(s) health stats and then, if applicable, apply the hediff
        public void ApplyHediff(float BloodPumpingStat, float BreathingStat, Pawn pawn)
        {
            if (BloodPumpingStat <= 0f || BreathingStat <= 0f)
            {
                pawn.health.AddHediff(IV_Bypass);

                // Stop the "Oxygen Deprivation" or "No Heartbeat" hediff from increasing
                List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

                foreach (Hediff hediff in Hediffs)
                {

                    var StrHediff = hediff.ToString();
                    if (StrHediff.Contains("ClinicalDeathNoHeartbeat"))
                    {
                        pawn.health.RemoveHediff(hediff);
                    }
                    if (StrHediff.Contains("ClinicalDeathAsphyxiation"))
                    {
                        pawn.health.RemoveHediff(hediff);
                    }
                }

            }
            else
            {
                return;
            }
        }
    }
}
