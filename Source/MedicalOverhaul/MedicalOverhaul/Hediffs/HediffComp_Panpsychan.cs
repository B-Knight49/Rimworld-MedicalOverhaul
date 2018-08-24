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
    public class HediffComp_Panpsychan : HediffComp
    {

        public HediffCompProperties_Panpsychan Props
        {
            get
            {
                return (HediffCompProperties_Panpsychan)base.props;
            }
        }

        public override void CompPostMake()
        {

            // Check to see if they've unlocked the MK2 version of this drug
            ResearchProjectDef Psychan2 = ResearchProjectDef.Named("PanPsychanMK2");
            bool completedResearch = Psychan2.IsFinished;

            List<Hediff> Hediffs = base.Pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
            bool overdosed = false;
            bool illegalDrug = false;
            Hediff overdoseHediff = null;

            foreach (Hediff hediff in Hediffs)
            {
                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("High"))
                {
                    if (completedResearch == true)
                    {
                        if (!StrHediff.Contains("GoJuice") && !StrHediff.Contains("Luciferium"))
                        {
                            base.Pawn.health.RemoveHediff(hediff);
                        }
                        else
                        {
                            // Don't treat overdoses for Luciferium or Go-Juice
                            illegalDrug = true;
                        }
                    }
                    else
                    {
                        if (!StrHediff.Contains("GoJuice") && !StrHediff.Contains("Luciferium") && !StrHediff.Contains("Flake") && !StrHediff.Contains("Yayo"))
                        {
                            base.Pawn.health.RemoveHediff(hediff);
                        }
                        else
                        {
                            // Don't treat overdoses for Luciferium or Go-Juice
                            illegalDrug = true;
                        }
                    }
                }

                if (StrHediff.Contains("Overdose"))
                {
                    overdosed = true;
                    overdoseHediff = hediff;
                }

                if (overdosed == true && illegalDrug == false)
                {
                    if (overdoseHediff != null)
                    {
                        overdoseHediff.Severity -= 0.75f;
                    }
                }
            }
        }
    }
}
