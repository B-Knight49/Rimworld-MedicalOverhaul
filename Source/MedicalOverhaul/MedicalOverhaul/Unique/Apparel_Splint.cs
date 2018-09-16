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
    public class Apparel_Splint : Apparel
    {
        public HediffDef diffLegs = HediffDef.Named("IV_Splint1");
        public HediffDef diffArms = HediffDef.Named("IV_Splint2");

        public Apparel_Splint()
        {
            List<Hediff> Hediffs = base.Wearer.health.hediffSet.GetHediffs<Hediff>().ToList();
            List<BodyPartRecord> brokenBones = new List<BodyPartRecord>();

            var movingParts = new List<string> { "LeftFemur", "RightFemur", "LeftTibia", "RightTibia" };
            var manipulationParts = new List<string> { "LeftClavicle", "RightClavicle", "LeftHumerus", "RightHumerus", "LeftRadius", "RightRadius" };

            foreach (Hediff hediff in Hediffs)
            {
                var StrHediff = hediff.ToString();
                if (StrHediff.Contains("IV_BrokenBone"))
                {
                    brokenBones.Add(hediff.Part);
                }
            }

            Log.Message("Length of brokenBones = " + brokenBones.Count());

            if (brokenBones.Count < 1)
            {
                base.Wearer.apparel.Remove(this);
            }

            foreach (BodyPartRecord brokenBone in brokenBones)
            {
                BodyPartDef bodyPart = brokenBone.def;
                string partString = bodyPart.ToString();

                Log.Message("bone " + partString + " checking...");

                if (movingParts.Contains(partString))
                {
                    // Apply the splint to the legs
                    Log.Message("bone " + partString + " movingPart");
                    base.Wearer.health.AddHediff(diffLegs);
                    return;
                }

                else if (manipulationParts.Contains(partString))
                {
                    // Apply the splint to the arms
                    Log.Message("bone " + partString + " manipulationPart");
                    base.Wearer.health.AddHediff(diffArms);
                    return;
                }

                else
                {
                    if (partString == "Pelvis" || partString == "Sternum")
                    {
                        Log.Message("bone " + partString + " is pelvis or sternum");
                        base.Wearer.apparel.Remove(this);
                    }
                    else
                    {
                        Log.Warning("Splint can not be applied to pawn " + base.Wearer + " even though bone " + partString + " is broken.");
                    }
                }
            }
        }
    }
}
