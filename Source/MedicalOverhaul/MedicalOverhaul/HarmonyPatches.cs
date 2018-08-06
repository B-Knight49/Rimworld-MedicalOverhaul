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

[StaticConstructorOnStartup]
class Main
{
    #pragma warning disable 0649
    public static HarmonyInstance harmony;
    #pragma warning restore 0649

    static Main()
    {
        harmony = HarmonyInstance.Create("com.medical.iv");

        // Patch the BleedPatch

        MethodInfo bleed_targetmethod = AccessTools.Method(typeof(HediffSet), "CalculateBleedRate");
        HarmonyMethod bleed_postfix = new HarmonyMethod(typeof(BleedPatch).GetMethod("Patch_Postfix"));
        harmony.Patch(bleed_targetmethod, null, bleed_postfix);

        // Patch SurgeryBedPatch

        MethodInfo bed_targetmethod = AccessTools.Method(typeof(Pawn), "CurrentlyUsableForBills");
        HarmonyMethod bed_postfix = new HarmonyMethod(typeof(SurgeryBedPatch).GetMethod("Patch_Postfix"));
        harmony.Patch(bed_targetmethod, null, bed_postfix);

        // Patch GiverAndDoer

        MethodInfo work_targetmethod = AccessTools.Method(typeof(WorkGiver_DoBill), "TryStartNewDoBillJob");
        HarmonyMethod work_prefix = new HarmonyMethod(typeof(GiverAndDoer).GetMethod("Patch_Prefix"));
        harmony.Patch(work_targetmethod, work_prefix, null);

        // Patch ToxicImmunityPatch

        MethodInfo toxic_targetmethod = AccessTools.Method(typeof(HealthUtility), "AdjustSeverity");
        HarmonyMethod toxic_prefix = new HarmonyMethod(typeof(ToxicImmunityPatch).GetMethod("Patch_Prefix"));
        harmony.Patch(toxic_targetmethod, toxic_prefix, null);
    }
}

static class BleedPatch
{
    public static void Patch_Postfix(ref float __result, HediffSet __instance)
    {
        __result *= __instance.pawn.GetStatValue(StatDef.Named("BleedRate"));
    }
}

static class SurgeryBedPatch
{
    public static void Patch_Postfix(ref bool __result, ref Pawn __instance)
    {
        __result = true; 
    }
}

static class GiverAndDoer
{
    public static bool Patch_Prefix(ref Pawn pawn, ref Bill bill, ref IBillGiver giver)
    {
        if (!bill.ToString().Contains("Bill_Administer_SerumOne") && !bill.ToString().Contains("Bill_Administer_MorphineSerum") && !bill.ToString().Contains("Bill_Administer_EpiSerum"))                              // If bill is NOT Injectable Coagulant OR Morphine OR Epinephrine then:
        {
            //Log.Message("[Case 1]" + "Patient: " + giver + "Surgeon: " + pawn);
            //Log.Message("Bill Name: " + bill);
            if (pawn.ToString() != giver.ToString())                                            // If patient is NOT the same as surgeon then:
            {
                Pawn giverPawn = giver as Pawn;
                if (giverPawn.CurrentBed() != null)                                             // If patient IS in bed then:
                {
                    return true;                                                                // Do Bill
                }
                return false;
            }
            return false;
        }

        else                                                                                    // If bill IS Injectable Coagulant OR Morphine OR Epinephrine then:
        {
            //Log.Message("[Case 2]" + "Patient: " + giver + "Surgeon: " + pawn);
            if (pawn.ToString() != giver.ToString())                                            // If patient is NOT the same as surgeon then:
            {
                return true;                                                                    // Do bill
            }
            return false;
        }
    }
}

static class ToxicImmunityPatch
{
    public static bool Patch_Prefix(ref Pawn pawn, ref float sevOffset, ref HediffDef hdDef)
    {
        bool ConsumedTablets = false;

        if (hdDef.ToString().Contains("ToxicBuildup"))
        {
            List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();

            foreach (Hediff hediff in Hediffs)
            {
                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("NoToxic"))
                {
                    ConsumedTablets = true;
                }
            }

            if (ConsumedTablets == true)
            {
                sevOffset = sevOffset * 0.25f;
            }
            return true;
        }
        return true;
    }
}