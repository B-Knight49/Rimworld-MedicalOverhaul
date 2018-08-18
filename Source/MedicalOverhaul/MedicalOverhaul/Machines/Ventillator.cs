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

namespace IV
{
    public class Building_Ventil : Building
    {
        public static HediffDef IV_Vent = HediffDef.Named("IV_Vent");
        public static HediffDef DR_Oxygen = HediffDef.Named("ClinicalDeathAsphyxiation");
        private CompPowerTrader powerComp = null;

        public Building_Ventil()
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
            for (int i = 0; i <adjacent.Length; i++)
            {
                var things = this.Map.thingGrid.ThingsListAt(adjacent[i] + position);
                foreach (Thing thing in things)
                {
                    if (thing is Pawn)
                    {
                        Pawn pawn = thing as Pawn;
                        var BreathingStat = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Breathing);

                        ApplyHediff(BreathingStat, pawn);
                    }
                }
            }
        }

        // Check the pawn(s) health stats and then, if applicable, apply the hediff
        public void ApplyHediff(float BreathingStat, Pawn pawn)
        {
            if (BreathingStat <= 0f)
            {
                pawn.health.AddHediff(IV_Vent);

                // Stop the "Oxygen Deprivation" hediff from increasing
                List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

                foreach (Hediff hediff in Hediffs)
                {
                    var StrHediff = hediff.ToString();
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
