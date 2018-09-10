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
    public class HediffComp_HediffChanger : HediffComp
    {

        public HediffCompProperties_HediffChanger Props
        {
            get
            {
                return (HediffCompProperties_HediffChanger)base.props;
            }
        }

        public override void CompPostMake()
        {
            List<Hediff> Hediffs = base.Pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            string reducedHediff = Props.hediffToReduce;

            foreach (Hediff hediff in Hediffs)
            {
                var listHediff = hediff.ToString();

                if (reducedHediff.Contains("IV_DiabetesHediff"))
                {
                    if (listHediff.Contains("IV_DiabetesHediff"))
                    {
                        if (hediff.Severity > 0.15f)
                        {
                            hediff.Severity -= 0.15f;
                        }
                        else if (hediff.Severity <= 0.15f)
                        {
                            hediff.Severity = 0.01f;
                            return;
                        }
                    }
                }

                if (reducedHediff.Contains("HeartArteryBlockage"))
                {
                    if (listHediff.Contains("HeartArteryBlockage"))
                    {
                        if (hediff.Severity > 0.0007f)
                        {
                            hediff.Severity -= 0.00075f;
                        }
                        else if (hediff.Severity <= 0.0007f)
                        {
                            //base.Pawn.health.RemoveHediff(hediff);
                            return;
                        }
                    }
                }

                else
                {
                    return;
                }
            }
        }
    }
}
