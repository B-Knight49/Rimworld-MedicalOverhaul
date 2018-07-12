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

            foreach (Hediff hediff in Hediffs)
            {

                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("HeartArteryBlockage"))
                {
                    if (hediff.Severity <= 0.05)
                    {
                        Log.Message("[Medical Overhaul] " + hediff.Severity);
                        hediff.Severity -= 0.05f;
                    }
                    else
                    {
                        base.Pawn.health.RemoveHediff(hediff);
                    }
                }
            }
        }
    }
}
