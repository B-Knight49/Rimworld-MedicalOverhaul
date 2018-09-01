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
        public HediffDef stomachHalf = HediffDef.Named("IV_StomachHalved");
        public HediffDef infection = HediffDef.Named("WoundInfection");

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            // BodyPartRecord 'part' should ALWAYS be stomach 
            List<BodyPartRecord> BodyParts = pawn.RaceProps.body.corePart.parts.ToList();

            foreach (BodyPartRecord BodyPart in BodyParts)
            {
                if (BodyPart.def.ToString().Contains("Stomach"))
                {
                    stomach = BodyPart;
                    break;
                }
            }

            // Make sure that the pawn has "Intestinal Failure"
            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            // Make sure they've not had the operation before
            bool stomachHediff = false;
            bool intestinalFailure = false;

            foreach (Hediff hediff in Hediffs)
            {
                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("IntestinalFailure"))
                {
                    intestinalFailure = true;
                }
                if (StrHediff.Contains("StomachHalved"))
                {
                    stomachHediff = true;
                }
            }

            // Apply the operation
            if (intestinalFailure == true && stomachHediff == false)
            {
                if (stomach != null)
                {
                    pawn.health.RestorePart(stomach);
                    pawn.health.AddHediff(stomachHalf, stomach);
                    pawn.health.AddHediff(infection, stomach);
                }
                Log.Warning("BodyPartRecord Stomach = NULL!");
            }

            else if (stomachHediff == true)
            {
                string text = "Pawn " + pawn + " failed to recieve stomach surgery because they have had it before!";
                Messages.Message(text, pawn, MessageTypeDefOf.NegativeHealthEvent, true);
            }
            else if (intestinalFailure == false)
            {
                string text = "Pawn " + pawn + " failed to recieve stomach surgery because they are healthy!";
                Messages.Message(text, pawn, MessageTypeDefOf.NegativeHealthEvent, true);
            }
        }
    }
}
