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
    public class Building_Chemo : Building
    {
        public static HediffDef IV_Chemo    = HediffDef.Named("IV_Chemo");
        public static HediffDef IV_ChemoBad = HediffDef.Named("IV_ChemoBad");
        private CompRefuelable refuelComp = null;
        private CompFlickable flickableComp = null;

        public Building_Chemo()
            : base()
        {
            Log.Message("[Medical Overhaul] Building Initialised");
        }

        // SpawnSetup and checking power sources
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            refuelComp = base.GetComp<CompRefuelable>();
            flickableComp = base.GetComp<CompFlickable>();
        }

        // Begin ticking 
        public override void Tick()
        {
            if ((Find.TickManager.TicksGame + thingIDNumber.HashOffset()) % 60 == 0)
            {
                base.Tick();

                if (refuelComp.HasFuel && flickableComp.SwitchIsOn)
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
                        Building curBed = pawn.CurrentBed();

                        ApplyHediff(curBed, pawn);
                    }
                }
            }
        }

        // Check the pawn(s) health stats and then, if applicable, apply the hediff
        public void ApplyHediff(Building bed, Pawn pawn)
        {
            Hediff Cancer = null;
            Hediff Hediff_ChemoBad = null;

            // Return the cancer hediff
            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            foreach (Hediff hediff in Hediffs)
            {
                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("IV_Cancer"))
                {
                    Cancer = hediff;
                }
                if (StrHediff.Contains("IV_ChemoBad"))
                {
                    Hediff_ChemoBad = hediff;
                }
            }

            if (bed != null && Cancer != null)
            {
                pawn.health.AddHediff(IV_Chemo);
                Cancer.Severity -= 0.000025f;

                if (Hediff_ChemoBad != null)
                {
                    Hediff_ChemoBad.Severity += 0.0035f;
                }
                else
                {
                    pawn.health.AddHediff(IV_ChemoBad);
                }

            }
            else
            {
                return;
            }
        }
    }
}
