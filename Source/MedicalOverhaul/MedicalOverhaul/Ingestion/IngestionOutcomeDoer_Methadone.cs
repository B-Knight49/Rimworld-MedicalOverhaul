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

    public class IngestionOutcomeDoer_Methadone : IngestionOutcomeDoer
    {
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            foreach (Hediff hediffList in Hediffs)
            {
                var strHediff = hediffList.ToString();

                if (strHediff.Contains("GoJuiceTolerance") || strHediff.Contains("WakeUpTolerance") || strHediff.Contains("PsychiteTolerance"))
                {
                    hediffList.Severity -= 0.01f;
                }
     
                if (strHediff.Contains("GoJuiceAddiction") || strHediff.Contains("WakeUpAddiction") || strHediff.Contains("PsychiteAddiction"))
                {
                    hediffList.Severity -= 0.01f;
                }
            }
        }
    }
}
