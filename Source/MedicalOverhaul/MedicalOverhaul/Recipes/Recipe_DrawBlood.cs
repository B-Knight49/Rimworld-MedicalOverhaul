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
    internal class Recipe_DrawBlood : RecipeWorker
    {
        public HediffDef BloodLoss = HediffDef.Named("BloodLoss");
        public ThingDef BloodBag = ThingDef.Named("IV_BloodBag");
        public bool containsBloodLoss = false;

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            // Get the pawn's location so we can spawn blood bags on them
            IntVec3 loc = billDoer.Position;
            Map map = billDoer.Map;

            // Return the BloodLoss hediffdef as a hediff so we can then set the severity
            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            foreach (Hediff hediff in Hediffs)
            {
                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("BloodLoss"))
                {
                    containsBloodLoss = true;
                    hediff.Severity += 0.15f;
                    GenSpawn.Spawn(BloodBag, loc, map);
                }
            }

            if (containsBloodLoss != true)
            {
                pawn.health.AddHediff(BloodLoss);
                List<Hediff> Hediffs2 = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

                foreach (Hediff hediff in Hediffs2)
                {
                    var StrHediff = hediff.ToString();
                    if (StrHediff.Contains("BloodLoss"))
                    {
                        hediff.Severity = 0.15f;
                        GenSpawn.Spawn(BloodBag, loc, map);
                    }
                }
            }
        }
    }
}
