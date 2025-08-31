using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using Pictomancy;
using RootofRiches.Scheduler.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Ui.MainWindow
{
    internal class FunMode
    {
        private static List<Vector3> Waypoints = new List<Vector3>();
        private static bool RunRoute = false;

        private static uint ToUintABGR(Vector4 col)
        {
            byte a = (byte)(col.W * 255);
            byte b = (byte)(col.Z * 255);
            byte g = (byte)(col.Y * 255);
            byte r = (byte)(col.X * 255);
            return (uint)((a << 24) | (b << 16) | (g << 8) | r);
        }

        private static Vector4 FromUintABGR(uint color)
        {
            float a = ((color >> 24) & 0xFF) / 255f;
            float b = ((color >> 16) & 0xFF) / 255f;
            float g = ((color >> 8) & 0xFF) / 255f;
            float r = (color & 0xFF) / 255f;
            return new Vector4(r, g, b, a);
        }

        private static Vector4 ImGuiCircleCol = FromUintABGR(C.PictoCircleColor); // ABGR Red
        private static Vector4 ImguiDotCol = FromUintABGR(C.PictoDotColor); // ABGR Red
        private static float DotSize = C.PictoDotRadius;
        private static List<Vector3> GenerateCircularWaypoints(Vector3 center, float radius, int pointCount)
        {
            List<Vector3> waypoints = new();

            for (int i = 0; i < pointCount; i++)
            {
                float angle = (float)(2 * Math.PI * i / pointCount); // Radians
                float x = center.X + radius * (float)Math.Cos(angle);
                float z = center.Z + radius * (float)Math.Sin(angle); // Z for forward/back in FFXIV

                waypoints.Add(new Vector3(x, center.Y, z));
            }

            return waypoints;
        }

        public static void Draw()
        {
            var target = Svc.Targets.Target;
            var targetPos = C.TargetPos;

            if (ImGui.Button("Set Target"))
            {
                if (target != null)
                {
                    targetPos = target.Position;
                    targetPos.Y = Svc.ClientState.LocalPlayer?.Position.Y + 1 ?? target.Position.Y;
                    C.TargetPos = targetPos;
                    C.Save();
                }
            }

            ImGui.Text($"{Player.DistanceTo(targetPos):N2}");
            if (ImGui.DragFloat3("Target Position", ref targetPos))
            {
                C.TargetPos = targetPos;
                C.Save();
            }
            ImGui.SetNextItemWidth(100);
            float targetRadius = C.Radius;
            if (ImGui.DragFloat("Radius", ref targetRadius))
            {
                C.Radius = targetRadius;
                C.Save();
            }
            ImGui.SetNextItemWidth(100);
            int pointCount = C.PointCount;
            if (ImGui.InputInt("Point Count", ref pointCount))
            {
                C.PointCount = pointCount;
                C.Save();
            }
            ImGui.SetNextItemWidth(100);
            if (ImGui.InputFloat("Point Radius", ref DotSize))
            {
                C.PictoDotRadius = DotSize;
                C.Save();
            }
            if (ImGui.ColorEdit4("Circle Color", ref ImGuiCircleCol))
            {
                C.PictoCircleColor = ToUintABGR(ImGuiCircleCol);
                C.Save();
            }
            if (ImGui.ColorEdit4("Dot Color", ref ImguiDotCol))
            {
                C.PictoDotColor = ToUintABGR(ImguiDotCol);
                C.Save();
            }

            Waypoints = GenerateCircularWaypoints(targetPos, targetRadius, pointCount);

            using (var drawList = PictoService.Draw())
            {
                if (drawList == null)
                    return;
                // Draw a circle around a GameObject's hitbox
                drawList.AddCircle(targetPos, targetRadius, C.PictoCircleColor);

                foreach (var entry in Waypoints)
                {
                    drawList.AddDot(entry, DotSize, C.PictoDotColor);
                }
            }

            if (ImGui.Button("Walk Route"))
            {
                RunRoute = true;
            }
            ImGui.SameLine();
            if (ImGui.Button("Stop Route"))
            {
                RunRoute = false;
            }

            if (P.taskManager.NumQueuedTasks == 0 && RunRoute)
            {
                Task_MoveTo.Enqueue(Waypoints);
            }
        }
    }
}
