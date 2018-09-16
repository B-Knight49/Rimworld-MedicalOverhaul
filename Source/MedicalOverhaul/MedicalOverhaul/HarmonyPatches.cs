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

        // Patch BlockOpiateNeed

        MethodInfo need_targetmethod = AccessTools.Method(typeof(IngestionOutcomeDoer_OffsetNeed), "DoIngestionOutcomeSpecial");
        HarmonyMethod need_prefix = new HarmonyMethod(typeof(BlockOpiateNeed).GetMethod("Patch_Prefix"));
        harmony.Patch(need_targetmethod, need_prefix, null);

        // Patch BlockOpiateHediff

        MethodInfo hediff_targetmethod = AccessTools.Method(typeof(IngestionOutcomeDoer_GiveHediff), "DoIngestionOutcomeSpecial");
        HarmonyMethod hediff_prefix = new HarmonyMethod(typeof(BlockOpiateHediff).GetMethod("Patch_Prefix"));
        harmony.Patch(hediff_targetmethod, hediff_prefix, null);

        // Patch DiabetesTrait

        MethodInfo trait_targetmethod = AccessTools.Method(typeof(PawnGenerator), "GenerateInitialHediffs");
        HarmonyMethod trait_postfix = new HarmonyMethod(typeof(DiabetesTrait).GetMethod("Patch_Postfix"));
        harmony.Patch(trait_targetmethod, null, trait_postfix);

        // Patch LimbEfficiency

        MethodInfo limb_targetmethod = AccessTools.Method(typeof(PawnCapacityUtility), "CalculatePartEfficiency");
        HarmonyMethod limb_postfix = new HarmonyMethod(typeof(LimbEfficiency).GetMethod("Patch_Postfix"));
        harmony.Patch(limb_targetmethod, null, limb_postfix);

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
                try
                {
                    if (giverPawn.CurrentBed() != null)                                         // If patient IS in bed then:
                    {
                        return true;                                                            // Do Bill
                    }
                    return false;
                }
                catch(NullReferenceException)                                                   // Error therefore bill is not medical so continue
                {
                    return true;
                }
            }
            return false;
        }

        else                                                                                    // If bill IS Injectable Coagulant OR Morphine OR Epinephrine then:
        {
            //Log.Message("[Case 2]" + "Patient: " + giver + "Surgeon: " + pawn);
            Pawn giverPawn = giver as Pawn;
            if (pawn.ToString() != giver.ToString())                                            // If patient is NOT the same as surgeon then:
            {
                if (giverPawn.Downed || giverPawn.Drafted || giverPawn.InBed() || giverPawn.IsPrisoner)
                {
                    return true;                                                                // Do bill
                }
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

static class BlockOpiateNeed
{
    public static bool Patch_Prefix(ref Pawn pawn, ref Thing ingested)
    {
        List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
        bool pawnNarcan = false;
        string ingestedString = ingested.ToString();

        foreach (Hediff hediff in Hediffs)
        {
            if (hediff.ToString().Contains("NarcanHediff"))
            {
                pawnNarcan = true;
            }
        }

        if (pawnNarcan == true)
        {
            if (ingestedString.Contains("GoJuice") || ingestedString.Contains("WakeUp") || ingestedString.Contains("Flake"))
            {
                // Narcan blocks the consumption of the opiate
                return false;
            }
        }
        return true;
    }
}

static class BlockOpiateHediff
{
    public static bool Patch_Prefix(ref Pawn pawn, ref Thing ingested)
    {
        List<Hediff> Hediffs = pawn.health.hediffSet.GetHediffs<Hediff>().ToList();
        bool pawnNarcan = false;
        string ingestedString = ingested.ToString();

        foreach (Hediff hediff in Hediffs)
        {
            if (hediff.ToString().Contains("NarcanHediff"))
            {
                pawnNarcan = true;
            }
        }

        if (pawnNarcan == true)
        {
            if (ingestedString.Contains("GoJuice") || ingestedString.Contains("WakeUp") || ingestedString.Contains("Flake"))
            {
                // Narcan blocks the consumption of the opiate
                return false;
            }
        }
        return true;
    }
}

static class DiabetesTrait
{
    public static TraitDef Diabetes = TraitDef.Named("Diabetic");
    public static HediffDef DiabetesHediff = HediffDef.Named("IV_DiabetesHediff");

    public static void Patch_Postfix(ref Pawn pawn)
    {
        if (pawn != null)
        {
            if (pawn.RaceProps.Humanlike)
            {
                if (pawn.story.traits.HasTrait(Diabetes))
                {
                    pawn.health.AddHediff(DiabetesHediff);
                }
            }
        }
    }
}

static class LimbEfficiency
{
    public static HediffDef BrokenBone1 = HediffDef.Named("IV_BrokenBone1");
    public static HediffDef BrokenBone2 = HediffDef.Named("IV_BrokenBone2");
    public static HediffDef BrokenBone3 = HediffDef.Named("IV_BrokenBone3");

    public static void Patch_Postfix(BodyPartRecord part, float __result, HediffSet diffSet)
    {
        Pawn partOwner = diffSet.pawn;
        List<Hediff> Hediffs             = partOwner.health.hediffSet.GetHediffs<Hediff>().ToList();
        var partList                     = new List<string> { "leftclavicle", "rightclavicle", "sternum", "lefthumerus", "righthumerus", "leftradius", "rightradius", "pelvis", "leftfemur", "rightfemur", "lefttibia", "righttibia" };
        var movingParts                  = new List<string> { "sternum", "pelvis", "leftfemur", "rightfemur","lefttibia","righttibia"};
        var manipulationParts            = new List<string> { "leftclavicle", "rightclavicle", "lefthumerus", "righthumerus", "leftradius", "rightradius","ribcage"};
        List<BodyPartRecord> brokenParts = new List<BodyPartRecord>();
        string partString                = part.Label.ToString();
        partString                       = partString.Replace(" ", "");

        foreach (Hediff hediff in Hediffs)
        {
            if (hediff.ToString().Contains("IV_BrokenBone"))
            {
                brokenParts.Add(hediff.Part);
            }
        }

        if (partList.Contains(partString))
        {
            if (!brokenParts.Contains(part))
            {
                if (__result < 0.40f)
                {
                    if (movingParts.Contains(partString))
                    {
                        if (partString == "sternum")
                        {
                            partOwner.health.AddHediff(BrokenBone3, part);
                        }
                        partOwner.health.AddHediff(BrokenBone1, part);
                        return;
                    }
                    if (manipulationParts.Contains(partString))
                    {
                        if (partString == "ribcage")
                        {
                            partOwner.health.AddHediff(BrokenBone3, part);
                        }
                        partOwner.health.AddHediff(BrokenBone2, part);
                        return;
                    }
                    else
                    {
                        Log.Warning("Bone is neither moving part or manipulation part. Bone name = " + partString);
                    }
                }
            }
        }
    }
}
