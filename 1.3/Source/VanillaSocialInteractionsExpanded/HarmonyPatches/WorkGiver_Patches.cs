using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VanillaSocialInteractionsExpanded
{
    [StaticConstructorOnStartup]
    public static class WorkGiver_Patches
    {
        static WorkGiver_Patches()
        {
            HarmonyInit.harmony.Patch(AccessTools.Method(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell)),
                transpiler: new HarmonyMethod(typeof(WorkGiver_Patches),
                nameof(JobOnCellTranspiler)));
        }
        
        public static IEnumerable<CodeInstruction> JobOnCellTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
        
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];
        
                if (instruction.opcode == OpCodes.Ldc_I4_0 && instructionList[i - 1].LoadsField(AccessTools.Field(typeof(PlantProperties), nameof(PlantProperties.sowMinSkill))))
                {
                    Label label = ilg.DefineLabel();
                    Label brLabel = ilg.DefineLabel();
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(WorkGiver_Patches), nameof(WorkGiver_Patches.MatchingInspirationPlanting)));
                    instruction = instructionList[++i];
                }
        
                yield return instruction;
            }
        }
        
        public static int MatchingInspirationPlanting(Pawn pawn)
        {
            if (pawn.InspirationDef != VSIE_DefOf.VSIE_Inspired_Planting)
            {
                return 0;
            }
            return int.MaxValue;
        }
    }
}
