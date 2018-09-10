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
    //public class Alert_Chemotherapy : Alert
    //{
    //    public Alert_Chemotherapy()
    //    { 
    //        Log.Message("Length of ChemoTreatment = " + ChemotherapyTreatment.Count());
    //        base.defaultLabel = ChemotherapyTreatment.Count().ToStringCached() + " colonist's receiving chemotherapy";
    //        base.defaultExplanation = ChemotherapyTreatment.Count().ToStringCached() + " colonist's are currently unconscious and receiving chemotherapy";
    //        base.defaultPriority = AlertPriority.High;
    //    }

    //    public override AlertReport GetReport()
    //    {
    //        return AlertReport.CulpritsAre(this.ChemotherapyTreatment);
    //    }

    //    private IEnumerable<Pawn> ChemotherapyTreatment
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return from p in PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer).Concat(PawnsFinder.AllMaps_PrisonersOfColonySpawned)
    //                       where HealthAIUtility.ShouldHaveSurgeryDoneNow(p) && p.InBed()
    //                       select p;
    //            }
    //            catch
    //            {
    //                Log.Message("Error in ChemotherapyTreatment");
    //                return null;
    //            }
    //        }
    //    }
    //}
}
