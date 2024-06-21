// -----------------------------------------------------------------------
// <copyright file="ChangingAhp.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using PlayerStatsSystem;

    /// <summary>
    /// Patches <see cref="AhpStat.ServerUpdateProcesses" /> for <see cref="Handlers.Player.ChangingAhp" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingAhp))]
    [HarmonyPatch(typeof(AhpStat), nameof(AhpStat.ServerUpdateProcesses))]
    internal static class ChangingAhp
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAhpEventArgs));

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_0);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // player = Player.Get(this.Hub);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(HealthStat), nameof(HealthStat.Hub))),
                new(OpCodes.Call, AccessTools.Method(typeof(Player), nameof(Player.Get), new Type[] { typeof(ReferenceHub) })),

                // this.ahpProcess;
                new(OpCodes.Ldloc_3),

                // ChangingAhpEventArgs ev = new(Player, ahpProcess)
                new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(ChangingAhpEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // OnChangingAhp(ev)
                new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingAhp))),

                // if (!ev.IsAllowed)
                //   goto continueLabel;
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ChangingAhpEventArgs), nameof(ChangingAhpEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, continueLabel),

                // this.ahpProcess = ev.AhpProcess;
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ChangingAhpEventArgs), nameof(ChangingAhpEventArgs.AhpProcess))),
                new CodeInstruction(OpCodes.Stloc_3),
            });

            // End of the loop.
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) - 1;

            newInstructions[index].WithLabels(continueLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}