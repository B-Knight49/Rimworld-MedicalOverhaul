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
    public class Building_VirusMachine : Building
    {

        public static HediffDef IV_Antibiotics = HediffDef.Named("IV_Antibiotics");
        private CompRefuelable refuelComp = null;
        private CompFlickable flickableComp = null;

        public Building_VirusMachine()
            : base()
        {
            Log.Message("[Medical IV's] Initialised Antibiotics Machine");
        }

        // Make sure it has power before doing stuff
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            refuelComp = base.GetComp<CompRefuelable>();
            flickableComp = base.GetComp<CompFlickable>();
        }

        // Repeat code every Tick (might change it to TickRare at some point)
        public override void Tick()
        {

            if ((Find.TickManager.TicksGame + thingIDNumber.HashOffset()) % 60 == 0)
            {
                base.Tick();

                if (refuelComp.HasFuel && flickableComp.SwitchIsOn)
                {
                    ApplyIV();
                }
            }
        }

        // Apply Hediff to every Pawn in adjacent cells once Tick() has been called
        public void ApplyIV()
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
                        pawn.health.AddHediff(IV_Antibiotics);

                        refuelComp.ConsumeFuel(0.02f);
                    }

                }
            }
        }

    }
}

