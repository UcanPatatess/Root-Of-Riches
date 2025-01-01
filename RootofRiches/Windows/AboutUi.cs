using Dalamud.Interface.Colors;
using Dalamud.Interface;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Utility;
using System.Diagnostics;

namespace RootofRiches.Windows;

public static class AboutUi
{
    private static string GetImageURL()
    {
        return Svc.PluginInterface.Manifest.IconUrl ?? "";
    }

    public static void Draw()
    {
        ImGuiEx.LineCentered("RoRAbout0", delegate
        {
            ImGuiEx.Text($"Root of Riches - {Svc.PluginInterface.Manifest.AssemblyVersion}");
        });

        ImGuiEx.LineCentered("RoRAbout1", () =>
        {
            ImGuiEx.Text($"Published and developed by Ucan and Ice");
        });

        ImGuiHelpers.ScaledDummy(10f);

        ImGuiEx.LineCentered("RoRAbout2", delegate
        {
            if (ThreadLoadImageHandler.TryGetTextureWrap(GetImageURL(), out var texture))
            {
                ImGui.Image(texture.ImGuiHandle, new(200f, 200f));
            }
        });
        ImGuiHelpers.ScaledDummy(10f);
        ImGuiEx.LineCentered("RoRAbout3", delegate
        {
            ImGui.TextWrapped("Join the Puni.sh Discord for support, questions, announcements.");
        });
        ImGuiEx.LineCentered("RoRAbout4", delegate
        {
            if (ImGui.Button("Discord Link"))
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "https://discord.gg/Zzrcc8kmvy",
                    UseShellExecute = true
                });
            }
            ImGui.SameLine();
            if (ImGui.Button("Repository"))
            {
                ImGui.SetClipboardText("PLEASE MAKE SURE TO PUT THIS AFTER UPLOAD");
                Notify.Success("Link copied to clipboard");
            }
            ImGui.SameLine();
            if (ImGui.Button("Source Code"))
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Svc.PluginInterface.Manifest.RepoUrl,
                    UseShellExecute = true
                });
            }
            ImGui.Dummy(new(0, 10));
            ImGuiEx.LineCentered("RoRDonations", () =>
            {
                ImGui.Text("Want to donate?");
                ImGui.SameLine();
                if (ImGui.Button("Ice's Kofi"))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "https://ko-fi.com/ice643269",
                        UseShellExecute = true
                    });
                }

            });
        });
    }
}
