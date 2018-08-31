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
    internal class Recipe_Stomach : RecipeWorker
    {
        public BodyPartRecord stomach = null;

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            // BodyPartRecord part should ALWAYS be stomach 
            string partString = part.def.ToString();

            if (partString.Contains("Stomach"))
            {
                BodyPartRecord stomach = part;
            }

            // Make sure that the pawn has "Intestinal Failure"
            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            foreach (Hediff hediff in Hediffs)
            {
                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("IntestinalFailure"))
                {
                    if (stomach != null)
                    {
                        pawn.health.RestorePart(stomach);
                    }
                    Log.Warning("BodyPartRecord Stomach = NULL!");
                }
            }
        }
    }
}
