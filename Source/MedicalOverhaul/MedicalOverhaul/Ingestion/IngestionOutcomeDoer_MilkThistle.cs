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

    public class IngestionOutcomeDoer_MilkThistle : IngestionOutcomeDoer
    {
        public HediffDef hediffDef;
        public float severity = -1f;
        public ChemicalDef toleranceChemical;
        #pragma warning disable 0649
        private readonly bool divideByBodySize;
#pragma warning restore 0649
        public HediffDef MilkThistleBenefit;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            MilkThistleBenefit = HediffDef.Named("MilkThistleBenefit");

            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            foreach (Hediff hediffList in Hediffs)
            {
                if (hediffList.ToString().Contains("Cirrhosis"))
                {
                    Hediff hediff = HediffMaker.MakeHediff(this.hediffDef, pawn, null);
                    float num = (!(this.severity > 0f)) ? this.hediffDef.initialSeverity : this.severity;
                    if (this.divideByBodySize)
                    {
                        num /= pawn.BodySize;
                    }
                    AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize(pawn, this.toleranceChemical, ref num);
                    hediff.Severity = num;
                    pawn.health.AddHediff(hediff, null, null);
                }
            }
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (parentDef.IsDrug && base.chance >= 1f)
            {
                using (IEnumerator<StatDrawEntry> enumerator = this.hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()).GetEnumerator())
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
            /*Error near IL_00e4: Unexpected return in MoveNext()*/
            ;
        }
    }
}
