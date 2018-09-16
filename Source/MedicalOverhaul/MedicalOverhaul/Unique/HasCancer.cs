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
    public class HasCancer
    {
        public static bool CheckForChemo(Pawn pawn)
        {
            if (pawn != null)
            {
                List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
                foreach (Hediff hediff in Hediffs)
                {
                    var StrHediff = hediff.ToString();
                    if (StrHediff.Contains("IV_Chemo"))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            { 
                Log.Error("Pawn is NULL!");
                return false;
            }
           
        }

        public static bool CheckForCancer(Pawn pawn)
        {
            if (pawn != null)
            {
                List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
                foreach (Hediff hediff in Hediffs)
                {
                    var StrHediff = hediff.ToString();
                    if (StrHediff.Contains("IV_Cancer"))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Log.Error("Pawn is NULL!");
                return false;
            }
        }
    }
}
