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
    public class HediffComp_Methadone : HediffComp
    {

        public HediffCompProperties_Methadone Props
        {
            get
            {
                return (HediffCompProperties_Methadone)base.props;
            }
        }

        public override void CompPostMake()
        {
            List<Hediff> Hediffs = base.Pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            foreach (Hediff hediff in Hediffs)
            {
                // Opiate's below:
                // - Flake 
                // - Yayo (THIS IS A STIMULANT, NOT AN OPIATE. MIGHT ADD AT FUTURE DATE)
                // - GoJuice
                // - WakeUp

                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("FlakeHigh") || StrHediff.Contains("GoJuiceHigh") || StrHediff.Contains("WakeUpHigh"))
                {
                    base.Pawn.health.RemoveHediff(hediff);
                }
            }
        }
    }
}
