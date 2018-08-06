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
    public class IngestionOutcomeDoer_SuperAntibiotics : IngestionOutcomeDoer
    {
        public HediffDef hediffDef;

        public float severity = -1f;

        public ChemicalDef toleranceChemical;

        #pragma warning disable 0649
        private bool divideByBodySize;
        #pragma warning restore 0649

        public static HediffDef HiddenAntibiotics;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            HiddenAntibiotics = HediffDef.Named("SuperAntiHidden");

            Hediff hediff = HediffMaker.MakeHediff(this.hediffDef, pawn, null);
            float num = (!(this.severity > 0f)) ? this.hediffDef.initialSeverity : this.severity;
            if (this.divideByBodySize)
            {
                num /= pawn.BodySize;
            }
            AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize(pawn, this.toleranceChemical, ref num);
            hediff.Severity = num;
            pawn.health.AddHediff(hediff, null, null);

            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
            
            // Temporary variables used to determine if code runs or not
            int timesUsed = 0;
            Hediff infection = null;

            foreach (Hediff hediffList in Hediffs)
            {
                var StrHediff = hediffList.ToString();
                if (StrHediff.Contains("SuperAntiHidden"))
                {
                    timesUsed += 1;
                    return;
                }
                if (StrHediff.Contains("Infection"))
                {
                    infection = hediffList;
                }
            }

            if (timesUsed != 1 && infection != null)
            {
                pawn.health.AddHediff(HiddenAntibiotics);
                pawn.health.RemoveHediff(infection);
            }
                
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (parentDef.IsDrug && base.chance >= 1f)
            {
                using (IEnumerator<StatDrawEntry> enumerator = this.hediffDef.SpecialDisplayStats().GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        StatDrawEntry s = enumerator.Current;
                        yield return s;
                        /*Error: Unable to find new state assignment for yield return*/
                        ;
                    }
                }
            }
            yield break;
            // IL_00e3:
            /*Error near IL_00e4: Unexpected return in MoveNext()*/
            ;
        }
    }
}

