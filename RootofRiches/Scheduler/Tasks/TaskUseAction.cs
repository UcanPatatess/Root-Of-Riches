using ECommons.Throttlers;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskUseAction
    {
        internal static unsafe void Enqueue(int Rotato)
        {
            uint Action1 = MCH_A4N[Rotato].ActionID;
            uint Action2 = MCH_A4N[Rotato].WeaveOneID;
            uint Action3 = MCH_A4N[Rotato].WeaveTwoID;

            P.taskManager.Enqueue(() => ExecuteAction(Action1));
            P.taskManager.Enqueue(() => Cooldown(Action1, 1));
            P.taskManager.Enqueue(() => ExecuteAbility(Action2));
            P.taskManager.Enqueue(() => Cooldown(Action1, 2));
            P.taskManager.Enqueue(() => ExecuteAbility(Action3));
        }

        internal static unsafe bool? Cooldown(uint Action, int Weave)
        {
            float Weave1 = 0.0f;
            float Weave2 = 0.0f;

            if (GetRealRecastTime(Action) == 0)
                return false;
            else if (GetRealRecastTime(Action) != 0)
            {
                Weave1 = GetRealRecastTime(Action) * 0.66f;
                Weave2 = GetRealRecastTime(Action) * 0.33f;
            }

            if (Weave == 1 && GetSpellCooldown(Action) <= Weave1)
                return true;
            else if (Weave == 2 && GetSpellCooldown(Action) <= Weave2)
                return true;

            return false;
        }
    }
}
