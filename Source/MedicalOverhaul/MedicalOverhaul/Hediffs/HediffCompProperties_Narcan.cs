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
    public class HediffCompProperties_Narcan : HediffCompProperties
    {
        public HediffCompProperties_Narcan()
        {
            base.compClass = typeof(HediffComp_Narcan);
        }
    }
}