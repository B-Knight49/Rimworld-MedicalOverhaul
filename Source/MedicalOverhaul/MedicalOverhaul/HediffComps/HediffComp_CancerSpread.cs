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
using UnityEngine;

namespace IV
{
    public class HediffComp_CancerSpread : HediffComp
    {

        public Hediff Cancer = null;

        protected const int SeverityUpdateInterval = 200;

        public HediffCompProperties_CancerSpread Props
        {
            get
            {
                return (HediffCompProperties_CancerSpread)base.props;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            bool hasCancer = false;

            List<Hediff> Hediffs = base.Pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
            foreach (Hediff hediff in Hediffs)
            {
                if (hediff.ToString().Contains("IV_Cancer"))
                {
                    hasCancer = true;
                }
            }

            if (hasCancer == false)
            {
                TrySendLetterPostMake();
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                return "Severity" + ": " + Cancer.Severity;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (base.Pawn.IsHashIntervalTick(200))
            {

                List<Hediff> Hediffs = base.Pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
                foreach (Hediff hediff in Hediffs)
                {
                    if (hediff.ToString().Contains("IV_Cancer"))
                    {
                        Cancer = hediff;
                    }
                }

                float num = this.SeverityChangePerDay();
                num *= 0.00333333341f;
                severityAdjustment += num;

                if (Cancer.Severity >= 0.3f && Cancer.Severity <= 0.31f)
                {
                    SpreadCancer(Cancer.Part);
                }
            }
        }

        protected virtual float SeverityChangePerDay()
        {
            return this.Props.severityPerDay;
        }

        public override string CompDebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.CompDebugString());
            if (!base.Pawn.Dead)
            {
                stringBuilder.AppendLine("severity/day: " + this.SeverityChangePerDay().ToString("F3"));
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public void SpreadCancer(BodyPartRecord bodyPart)
        {
            HediffDef CancerHediff = HediffDef.Named("IV_Cancer");
            List<string> vitals = new List<string> {"LeftLung","RightLung","Liver","LeftKidney","RightKidney","Brain","Stomach"};

            BodyPartRecord LeftLung = null;
            BodyPartRecord RightLung = null;
            BodyPartRecord Liver = null;
            BodyPartRecord LeftKidney = null;
            BodyPartRecord RightKidney = null;
            BodyPartRecord Brain = null;
            BodyPartRecord Stomach = null;

            List<BodyPartRecord> BodyParts = base.Pawn.RaceProps.body.AllParts;

            int kidneyCount = 0;
            int lungCount = 0;

            foreach (BodyPartRecord BodyPart in BodyParts)
            {
                string partString = BodyPart.def.ToString();

                //Log.Message("bodyPart = " + partString);
                if (partString.Contains("Stomach"))
                {
                    Stomach = BodyPart;
                }
                else if (partString.Contains("Lung") && lungCount == 0)
                {
                    LeftLung = BodyPart;
                    lungCount += 1;
                }
                else if (partString.Contains("Lung") && lungCount == 1)
                {
                    RightLung = BodyPart;
                }
                else if (partString.Contains("Liver"))
                {
                    Liver = BodyPart;
                }
                else if (partString.Contains("Kidney") && kidneyCount == 0)
                {
                    LeftKidney = BodyPart;
                    kidneyCount += 1;
                }
                else if (partString.Contains("Kidney") && kidneyCount == 1)
                {
                    RightKidney = BodyPart;
                }
                else if (partString.Contains("Brain"))
                {
                    Brain = BodyPart;
                }
            }

            //Log.Message("Stomach = " + Stomach);
            //Log.Message("LeftLung = " + LeftLung);
            //Log.Message("RightLung = " + RightLung);
            //Log.Message("Liver = " + Liver);
            //Log.Message("LeftKidney = " + LeftKidney);
            //Log.Message("RightKidney = " + RightKidney);
            //Log.Message("Brain = " + Brain);
            //Log.Message("Cancer is on part: " + Cancer.Part.customLabel);

            if (Cancer.Part.customLabel == "right lung")
            {
                vitals.Remove("RightLung");
            }
            else if (Cancer.Part.customLabel == "left lung")
            {
                vitals.Remove("LeftLung");
            }
            else
            {
                vitals.Remove(Cancer.Part.def.ToString());
            }

            var nextCancer = vitals.RandomElement();
            BodyPartDef nextCancerDef = null;
            Log.Message("nextCancer = " + nextCancer);

            if (nextCancer == "LeftLung")
            {
                base.Pawn.health.AddHediff(CancerHediff, LeftLung);
                nextCancerDef = LeftLung.def;
            }
            else if (nextCancer == "RightLung")
            {
                base.Pawn.health.AddHediff(CancerHediff, RightLung);
                nextCancerDef = RightLung.def;
            }
            else if (nextCancer == "Liver")
            {
                base.Pawn.health.AddHediff(CancerHediff, Liver);
                nextCancerDef = Liver.def;
            }
            else if (nextCancer == "LeftKidney")
            {
                base.Pawn.health.AddHediff(CancerHediff, LeftKidney);
                nextCancerDef = LeftKidney.def;
            }
            else if (nextCancer == "RightKidney")
            {
                base.Pawn.health.AddHediff(CancerHediff, RightKidney);
                nextCancerDef = RightKidney.def;
            }
            else if (nextCancer == "Brain")
            {
                base.Pawn.health.AddHediff(CancerHediff, Brain);
                nextCancerDef = Brain.def;
            }
            else if (nextCancer == "Stomach")
            {
                base.Pawn.health.AddHediff(CancerHediff, Stomach);
                nextCancerDef = Stomach.def;
            }

            TrySendLetter(nextCancerDef);
            Cancer.Severity += 0.01f;

        }

        protected bool TrySendLetter(BodyPartDef part)
        {
            if (!PawnUtility.ShouldSendNotificationAbout(base.Pawn))
            {
                return false;
            }
            string label = "Cancer Spread";
            string text = base.Pawn.Label.CapitalizeFirst();
            text = text + "'s cancer has spread to their " + part;
            text = text.AdjustedFor(base.Pawn, "PAWN");
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NegativeEvent, base.Pawn, null, null);
            return true;
        }

        protected bool TrySendLetterPostMake()
        {
            if (!PawnUtility.ShouldSendNotificationAbout(base.Pawn))
            {
                return false;
            }
            string label = "Cancer";
            string text = base.Pawn.Label.CapitalizeFirst();
            text = text + " has cancer...";
            text = text.AdjustedFor(base.Pawn, "PAWN");
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NegativeEvent, base.Pawn, null, null);
            return true;
        }
    }
}
