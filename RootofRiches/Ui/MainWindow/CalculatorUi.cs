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
            { "Jewelry", $"{A4NJewelry:N0}"},
            { "Hands", $"{A4NHand:N0}"},
            { "Shoes", $"{A4NShoes:N0}"},
            { "Body", $"{A4NBody:N0}"},
            { "Legs", $"{A4NLeg:N0}"},
            { "Total Gear Pieces", $"{TotalA4NGear:N0}"}
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
            TotalFCPoints = TotalA4NGear * A4NiLvl * FCPointCalc;
            TotalGCBase = TotalA4NGear * SealBase;
            TotalGC10 = TotalA4NGear * Seal10;
            TotalGC15 = TotalA4NGear * Seal15;
            OilclothBase = (TotalGCBase / OilclothBuy) * OilclothSell;
            Oilcloth10 = (TotalGC10 / OilclothBuy) * OilclothSell;
            Oilcloth15 = (TotalGC15 / OilclothBuy) * OilclothSell;
            VendorGil = (A4NJewelry * A4NJewelrySell) + (A4NHand * A4NHandSell) + (A4NShoes * A4NShoeSell) + (A4NBody * A4NBodySell) + (A4NLeg * A4NLegSell);
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
    }
}
