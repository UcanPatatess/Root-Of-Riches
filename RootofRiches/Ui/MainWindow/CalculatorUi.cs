using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Ui.MainWindow;

internal class CalculatorUi
{
    public static void Draw()
    {

        // All references have been moved for this to the Data.cs file
        string[,] testTableData = new string[,]
        {
            { "Jewelry", $"{TotalJewelry:N0}"},
            { "Head", $"{TotalHead:N0}"},
            { "Hands", $"{TotalHand:N0}"},
            { "Shoes", $"{TotalShoes:N0}"},
            { "Body", $"{TotalBody:N0}"},
            { "Legs", $"{TotalLeg:N0}"},
            { "Total Gear Pieces", $"{TotalGear:N0}"}
        };

        string[,] fcPointTableData = new string[,]
        {
            { "Total FC Points", $"{TotalFCPoints:N0}"},
        };

        string[,] calculationTableData = new string[,]
        {
            { "0%%", $"{TotalGCBase:N0}", $"{OilclothBase:N0}" },
            { "10%%", $"{TotalGC10:N0}", $"{Oilcloth10:N0}" },
            { "15%%", $"{TotalGC15:N0}", $"{Oilcloth15:N0}" },
        };

        string[,] sellGearVendorTableData = new string[,]
        {
            { "Vendor Gil -> Gear", $"{VendorGil:N0}" }
        };

        if (ImGui.Button("Update Table"))
        {
            A4NJewelry = GetItemCount(GordianBoltID);
            A4NHand = GetItemCount(GordianCrankID) / 2;
            A4NShoes = GetItemCount(GordianPedalID) / 2;
            A4NBody = GetItemCount(GordianShaftID) / 4;
            A4NLeg = GetItemCount(GordianSpringID) / 4;
            TotalA4NGear = A4NJewelry + A4NHand + A4NShoes + A4NBody + A4NLeg;

            O3NJewelry = GetItemCount(DeltascapeBoltID);
            O3NHead = GetItemCount(DeltascapeLensID) / 2;
            O3NHand = GetItemCount(DeltascapeCrankID) / 2;
            O3NShoes = GetItemCount(DeltascapePedalID) / 2;
            O3NBody = GetItemCount(DeltascapeShaftID) / 4;
            O3NLeg = GetItemCount(DeltascapeSpringID) / 4;
            TotalO3NGear = O3NJewelry + O3NHead + O3NHand + O3NShoes + O3NBody + O3NLeg;

            TotalJewelry = A4NJewelry + O3NJewelry;
            TotalHead = A4NHead + O3NHead;
            TotalHand = A4NHand + O3NHand;
            TotalShoes = A4NShoes + O3NShoes;
            TotalBody = A4NBody + O3NBody;
            TotalLeg = A4NLeg + O3NLeg;
            TotalGear = TotalA4NGear + TotalO3NGear;

            TotalFCPoints = (TotalA4NGear * A4NiLvl * FCPointCalc) + (TotalO3NGear * O3NiLvl * FCPointCalc);
            TotalGCBase = (TotalA4NGear * A4NSealBase) + (TotalO3NGear * O3NSealBase);
            TotalGC10 = (TotalA4NGear * A4NSeal10) + (TotalO3NGear * O3NSeal10);
            TotalGC15 = (TotalA4NGear * A4NSeal15) + (TotalO3NGear * O3NSeal15);
            OilclothBase = (TotalGCBase / OilclothBuy) * OilclothSell;
            Oilcloth10 = (TotalGC10 / OilclothBuy) * OilclothSell;
            Oilcloth15 = (TotalGC15 / OilclothBuy) * OilclothSell;
            VendorGil = (A4NJewelry * A4NJewelrySell) + (A4NHand * A4NHandSell)
                        + (A4NShoes * A4NShoeSell) + (A4NBody * A4NBodySell) + (A4NLeg * A4NLegSell)
                        + (O3NJewelry * O3NJewelrySell) + (O3NHead + O3NHeadSell) + (O3NHand + O3NHandSell)
                        + (O3NShoes * O3NShoeSell) + (O3NBody * O3NBodySell) + (O3NLeg * O3NLegSell);
        }
        ImGui.NewLine();

        if (ImGui.BeginTable("TestTable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Set up columns without automatic headers
            ImGui.TableSetupColumn("Armory Slot");
            ImGui.TableSetupColumn("Amount of Gear");

            // Custom header row
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            string[] headers = { "Armory Slot", "Amount of Gear" };
            for (int col = 0; col < headers.Length; col++)
            {
                ImGui.TableSetColumnIndex(col);

                // Calculate the available space and text size
                var header = headers[col];
                var textSize = ImGui.CalcTextSize(header);
                var columnWidth = ImGui.GetColumnWidth();
                var cursorPosX = ImGui.GetCursorPosX();

                // Set the cursor position to center the text
                ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                ImGui.Text(header);
            }

            // Render table rows
            for (int i = 0; i < testTableData.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 2; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = testTableData[i, col];
                    var textSize = ImGui.CalcTextSize(text);
                    var columnWidth = ImGui.GetColumnWidth();
                    var cursorPosX = ImGui.GetCursorPosX();

                    // Set the cursor position to center the text
                    ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                    ImGui.Text(text);
                }
            }

            ImGui.EndTable();
        }

        if (ImGui.BeginTable("Vendor Sell Gear Table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Render table rows
            for (int i = 0; i < sellGearVendorTableData.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 2; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = sellGearVendorTableData[i, col];
                    var textSize = ImGui.CalcTextSize(text);
                    var columnWidth = ImGui.GetColumnWidth();
                    var cursorPosX = ImGui.GetCursorPosX();

                    // Set the cursor position to center the text
                    ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                    ImGui.Text(text);
                }
            }

            ImGui.EndTable();
        }

        if (ImGui.BeginTable("Fc Points Table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Render table rows
            for (int i = 0; i < fcPointTableData.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 2; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = fcPointTableData[i, col];
                    var textSize = ImGui.CalcTextSize(text);
                    var columnWidth = ImGui.GetColumnWidth();
                    var cursorPosX = ImGui.GetCursorPosX();

                    // Set the cursor position to center the text
                    ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                    ImGui.Text(text);

                    if (col == 0 && text == "Total FC Points" && ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("You only gain fc points if you turn in directly to the Grand Company!");
                        ImGui.EndTooltip();
                    }
                }
            }

            ImGui.EndTable();
        }

        ImGui.Spacing();

        if (ImGui.BeginTable("Oilcloth Tables", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Set up columns without automatic headers
            ImGui.TableSetupColumn("Seal Buff");
            ImGui.TableSetupColumn("Total Seals");
            ImGui.TableSetupColumn("Total Gil");

            // Custom header row
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            string[] headers = { "Seal Buff", "Total Seals", "Total Gil" };
            for (int col = 0; col < headers.Length; col++)
            {
                ImGui.TableSetColumnIndex(col);

                // Calculate the available space and text size
                var header = headers[col];
                var textSize = ImGui.CalcTextSize(header);
                var columnWidth = ImGui.GetColumnWidth();
                var cursorPosX = ImGui.GetCursorPosX();

                // Set the cursor position to center the text
                ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                ImGui.Text(header);
            }

            // Render table rows
            for (int i = 0; i < calculationTableData.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 3; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = calculationTableData[i, col];
                    var textSize = ImGui.CalcTextSize(text);
                    var columnWidth = ImGui.GetColumnWidth();
                    var cursorPosX = ImGui.GetCursorPosX();

                    // Set the cursor position to center the text
                    ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                    ImGui.Text(text);
                }
            }

            ImGui.EndTable();
        }

    }
}
