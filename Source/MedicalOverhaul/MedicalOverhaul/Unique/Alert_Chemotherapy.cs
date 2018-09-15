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
    public class Alert_Chemotherapy : Alert
    {
        private IEnumerable<Pawn> ChemotherapyTreatment
        {
            get
            {
                return from p in PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer).Concat(PawnsFinder.AllMaps_PrisonersOfColonySpawned)
                       where HasCancer.CheckForChemo(p) && p.InBed()
                       select p;
            }
        }

        public override string GetLabel()
        {
            string Label = this.ChemotherapyTreatment.Count().ToStringCached() + " colonists receiving chemotherapy";
            return Label;
        }

        public override string GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Pawn item in this.ChemotherapyTreatment)
            {
                stringBuilder.AppendLine("    " + item.LabelShort.CapitalizeFirst());
            }
            string LabelDesc = "These colonists are unconscious and receiving chemotherapy:\n\n" + stringBuilder.ToString();
            return LabelDesc; 
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(this.ChemotherapyTreatment);
        }

        
    }
}
