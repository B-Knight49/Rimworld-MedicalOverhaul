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

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            pawn.health.AddHediff(BloodLoss);

            // Return the BloodLoss hediffdef as a hediff so we can then set the severity
            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            foreach (Hediff hediff in Hediffs)
            {
                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("BloodLoss"))
                {
                    hediff.Severity = 0.4f;
                }
            }
        }
    }
}
