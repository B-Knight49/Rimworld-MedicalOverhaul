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
    internal class Recipe_Scars : RecipeWorker
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            List<Hediff> allHediffs = pawn.health.hediffSet.hediffs;
            int i = 0;
            while (true)
            {
                if (i < allHediffs.Count)
                {
                    if (allHediffs[i].Part != null && allHediffs[i].def == recipe.removesHediff && allHediffs[i].Visible)
                    {
                        break;
                    }
                    i++;
                    continue;
                }
                yield break;
            }
            yield return allHediffs[i].Part;
            /*Error: Unable to find new state assignment for yield return*/
            ;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            // Return the BloodLoss hediffdef as a hediff so we can then set the severity
            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
            Hediff Scar = null;

            // Split the bill string into an array so we can check if the name is the same as the scar we're editing
            string[] billSplit = bill.ToString().Split('_');
            string InjuryName = billSplit[1];

            foreach (Hediff hediff in Hediffs)
            {
                // Split the name of the hediff into an array so we can check if the scar is the same as the bill name
                string[] hediffSplit = hediff.ToString().Split(' ');
                string hediffName = hediffSplit[0];

                // Hediffs are in brackets. Remove open bracket to allow for comparison without issue
                hediffName = hediffName.Replace("(","");

                if (hediff.Part == part && hediff.IsOld())
                {
                    if (InjuryName.Contains(hediffName))
                    {
                        Scar = hediff;
                    }
                }
            }
            if (Scar != null)
            {
                Scar.Severity -= 1f;
            }
        }
    }
}
