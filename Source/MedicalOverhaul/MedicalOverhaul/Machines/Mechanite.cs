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
    public class Building_Mechanite : Building
    {
        public static HediffDef IV_Mechanite = HediffDef.Named("IV_Mechanite");
        public static HediffDef IV_Mechanite2 = HediffDef.Named("IV_Mechanite2");
        private CompRefuelable refuelComp = null;
        private CompFlickable flickableComp = null;

        public Building_Mechanite()
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
                        List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
                        List<Hediff> Mechanites = new List<Hediff>();

                        // Make two lists containing all the Pawn's hediffs and only the ones which contain mechanites

                        foreach (Hediff hediff in Hediffs)
                        {
                            if (hediff.ToString().Contains("LuciferiumAddiction") || hediff.ToString().Contains("FibrousMechanites") || hediff.ToString().Contains("SensoryMechanites"))
                            {
                                // Add the mechanite hediff to the list
                                Mechanites.Add(hediff);
                            }
                        }

                        // Make sure the Mechanites list isn't empty
                        if (Mechanites.Count() >= 1)
                        {
                            RemoveMechanites(Mechanites, pawn);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }

        public void RemoveMechanites(List<Hediff> Mechanites, Pawn pawn)
        {
            pawn.health.AddHediff(IV_Mechanite);
            pawn.health.AddHediff(IV_Mechanite2);

            foreach (Hediff hediff in Mechanites)
            {
                pawn.health.RemoveHediff(hediff);
                Log.Message("Removed mechanite hediff: " + hediff);
                refuelComp.ConsumeFuel(0.1f);
            }
        }
    }
}
