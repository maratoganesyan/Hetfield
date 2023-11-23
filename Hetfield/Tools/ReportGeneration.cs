using Hetfield.Tools.ModelsForGeneration;
using Hetfield.Windows;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using ScottPlot.Palettes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace Hetfield.Tools
{
    internal class ReportGeneration
    {
        #region PaidContract
        public static async System.Threading.Tasks.Task DoAPaidContractAsync(PaidContractsModel Model, FrameworkElement ControlToDisable, FrameworkElement ControlToVisible)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = "Microsoft Word Document (*.docx)|*.docx";
            if (sv.ShowDialog() == true)
            {
                ControlToDisable.IsEnabled = false;
                ControlToVisible.Visibility = Visibility.Visible;
                await System.Threading.Tasks.Task.Run(() => GeneratePaidContract(Model, sv.FileName));
                new MessageBoxWindow("Генерация ДКП успешно завершена!").ShowDialog();
                ControlToDisable.IsEnabled = true;
                ControlToVisible.Visibility = Visibility.Hidden;
            }
        }

        private static void GeneratePaidContract(PaidContractsModel Model, string NewPath)
        {
            try
            {
                byte[] FileBytes = Properties.Resources.HetfieldPaidContract;
                string TempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(TempFilePath, FileBytes);

                Word.Application App = new Word.Application();
                App.Visible = false;
                Word.Document document = App.Documents.Open(TempFilePath);

                ChangeWordsPaidContract(Model, document);

                document.SaveAs2(FileName: NewPath);
                document.Close();
                App.Quit();
            }
            catch (System.Exception ex)
            {
                new MessageBoxWindow(ex.Message).ShowDialog();
            }
        }

        private static void ChangeWordsPaidContract(PaidContractsModel Model, Word.Document document)
        {
            ToolsForGeneration.ReplaceWordWithUnderline("{DateOfDrawingUp}", $"{Model.DateOfDrawingUp:dd.MM.yyyy}", document);
            ToolsForGeneration.ReplaceWord("{MarkAndModel}", Model.MarkAndModel, document);
            ToolsForGeneration.ReplaceWord("{VIN}", Model.VIN, document);
            ToolsForGeneration.ReplaceWord("{CarType}", Model.CarType, document);
            ToolsForGeneration.ReplaceWord("{ManufactureYear}", Model.ManufactureYear.ToString(), document);
            ToolsForGeneration.ReplaceWord("{CarColor}", Model.CarColor, document);
            ToolsForGeneration.ReplaceWord("{CarPower}", Model.CarPower.ToString(), document);
            ToolsForGeneration.ReplaceWord("{EngineDisplacement}", Model.EngineDisplacement.ToString(), document);
            ToolsForGeneration.ReplaceWord("{PassportSeriesAndNumber}", Model.PassportSeriesAndNumber, document);
            ToolsForGeneration.ReplaceWord("{Transmission}", Model.Transmission, document);
            ToolsForGeneration.ReplaceWord("{Configuration}", Model.Configuration, document);
            ToolsForGeneration.ReplaceWord("{Mileage}", Model.Mileage.ToString(), document);
            ToolsForGeneration.ReplaceWord("{TankCapacity}", Model.TankCapacity.ToString(), document);
            ToolsForGeneration.ReplaceWord("{OwnerInitials}", Model.OwnerInitials, document);
            ToolsForGeneration.ReplaceWord("{OwnerInitials2}", Model.OwnerInitials, document);
            if (Model.BuyerInitials.Length > 28)
            {
                ToolsForGeneration.ReplaceWord("{BuyerInitials}", "", document);
                ToolsForGeneration.ReplaceWord("{BuyerInitials2}", "", document);
                return;
            }
            ToolsForGeneration.ReplaceWord("{BuyerInitials}", Model.BuyerInitials, document);
            ToolsForGeneration.ReplaceWord("{BuyerInitials2}", Model.BuyerInitials, document);

            ToolsForGeneration.ReplaceWord("{Buyer}", Model.Buyer, document);
            ToolsForGeneration.ReplaceWord("{Owner}", Model.Owner, document);
            ToolsForGeneration.ReplaceWord("{TotalPrice}", Model.TotalPrice.ToString(), document);
            ToolsForGeneration.ReplaceWord("{TotalPrice2}", Model.TotalPrice.ToString(), document);

        }
        #endregion

        #region WorkReport
        public static async System.Threading.Tasks.Task DoAWorkReportAsync(WorkReportModel Model, FrameworkElement ControlToDisable, FrameworkElement ControlToVisible)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = "Microsoft Word Document (*.docx)|*.docx";
            if (sv.ShowDialog() == true)
            {
                ControlToDisable.IsEnabled = false;
                ControlToVisible.Visibility = Visibility.Visible;
                await System.Threading.Tasks.Task.Run(() => GenerateWorkReport(Model, sv.FileName));
                new MessageBoxWindow("Генерация отчёта по выполненным договорам успешно завершена!").ShowDialog();
                ControlToDisable.IsEnabled = true;
                ControlToVisible.Visibility = Visibility.Hidden;
            }
        }
        private static void GenerateWorkReport(WorkReportModel Model, string NewPath)
        {
            try
            {
                byte[] FileBytes = Properties.Resources.HetfieldWorkReport;
                string TempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(TempFilePath, FileBytes);

                Word.Application App = new Word.Application();
                App.Visible = false;
                Word.Document document = App.Documents.Open(TempFilePath);

                ChangeWordsWorkReport(Model, document);
                GenerateTableWorkReport(Model, document);

                document.SaveAs2(FileName: NewPath);
                document.Close();
                App.Quit();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private static void ChangeWordsWorkReport(WorkReportModel Model, Word.Document document)
        {
            ToolsForGeneration.ReplaceWordWithUnderline("{DateOfDrawingUp}", $"{Model.DateOfDrawingUp:dd.MM.yyyy}", document);
            ToolsForGeneration.ReplaceWord("{TotalCost}", $"{Model.TotalCost} ₽", document);
            ToolsForGeneration.ReplaceWord("{StartDate}", $"{Model.StartDate:dd.MM.yyyy}", document);
            ToolsForGeneration.ReplaceWord("{EndDate}", $"{Model.EndDate:dd.MM.yyyy}", document);
            ToolsForGeneration.ReplaceWord("{CurrentDate}", $"{Model.CurrentDate:dd.MM.yyyy}", document);
        }
        private static void GenerateTableWorkReport(WorkReportModel Model, Word.Document document)
        {
            Table table = document.Tables[1];
            int CountOfOrders = Model.Orders.Count;
            for (int i = 1; i <= CountOfOrders; i++)
                table.Rows.Add();
            int index = 0;
            foreach (Row row in table.Rows)
            {
                if (row.Index == 1)
                    continue;
                foreach (Cell cell in row.Cells)
                {
                    _ = cell.ColumnIndex == 1 ? cell.Range.Text = Model.Orders[index].idOrder.ToString() :
                        cell.ColumnIndex == 2 ? cell.Range.Text = Model.Orders[index].Owner :
                        cell.ColumnIndex == 3 ? cell.Range.Text = Model.Orders[index].Buyer :
                        cell.ColumnIndex == 4 ? cell.Range.Text = Model.Orders[index].Staff :
                        cell.ColumnIndex == 5 ? cell.Range.Text = Model.Orders[index].CarMarkAndModel :
                        cell.ColumnIndex == 6 ? cell.Range.Text = $"{Model.Orders[index].CarPrice} ₽" :
                        cell.ColumnIndex == 7 ? cell.Range.Text = $"{Model.Orders[index].FinalPrice} ₽" :
                        cell.Range.Text = $"{Model.Orders[index].DateOfOrder:dd.MM.yyyy}";
                }
                index++;
            }
        }
        #endregion

        #region EmployeeReport
        public static async System.Threading.Tasks.Task DoAEmployeeReportAsync(EmployeeReportModel Model, FrameworkElement ControlToDisable, FrameworkElement ControlToVisible)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = "Microsoft Word Document (*.docx)|*.docx";
            if (sv.ShowDialog() == true)
            {
                ControlToDisable.IsEnabled = false;
                ControlToVisible.Visibility = Visibility.Visible;
                await System.Threading.Tasks.Task.Run(() => GenerateEmployeeReport(Model, sv.FileName));
                new MessageBoxWindow("Генерация отчёта по выполненным договорам успешно завершена!").ShowDialog();
                ControlToDisable.IsEnabled = true;
                ControlToVisible.Visibility = Visibility.Hidden;
            }
        }
        private static void GenerateEmployeeReport(EmployeeReportModel Model, string NewPath)
        {
            try
            {
                byte[] FileBytes = Properties.Resources.HetfieldEmployeeReport;
                string TempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(TempFilePath, FileBytes);

                Word.Application App = new Word.Application();
                App.Visible = false;
                Word.Document document = App.Documents.Open(TempFilePath);

                ChangeWordsEmployeeReport(Model, document);
                GenerateTableEmployeeReport(Model, document);

                document.SaveAs2(FileName: NewPath);
                document.Close();
                App.Quit();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private static void GenerateTableEmployeeReport(EmployeeReportModel Model, Document document)
        {
            Table table = document.Tables[1];
            int CountOfOrders = Model.Orders.Count;
            for (int i = 1; i <= CountOfOrders; i++)
                table.Rows.Add();
            int index = 0;
            foreach (Row row in table.Rows)
            {
                if (row.Index == 1)
                    continue;
                foreach (Cell cell in row.Cells)
                {
                    /*if (cell.ColumnIndex == 6)
					{
						cell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
					}*/
                    _ = cell.ColumnIndex == 1 ? cell.Range.Text = Model.Orders[index].idOrder.ToString() :
                        cell.ColumnIndex == 2 ? cell.Range.Text = Model.Orders[index].CarMarkAndModel :
                        cell.ColumnIndex == 3 ? cell.Range.Text = Model.Orders[index].Owner :
                        cell.ColumnIndex == 4 ? cell.Range.Text = Model.Orders[index].Buyer :
                        cell.ColumnIndex == 5 ? cell.Range.Text = $"{Model.Orders[index].CarPrice} ₽" :
                        cell.ColumnIndex == 6 ? cell.Range.Text = $"{Model.Orders[index].FinalPrice} ₽" :
                        cell.Range.Text = $"{Model.Orders[index].DateOfOrder:dd.MM.yyyy}";
                }
                index++;
            }
            //ToolsForGeneration.AutoSizeColumn(table, 4);
        }
        private static void ChangeWordsEmployeeReport(EmployeeReportModel Model, Document document)
        {
            ToolsForGeneration.ReplaceWordWithUnderline("{DateOfDrawingUp}", $"{Model.DateOfDrawingUp:dd.MM.yyyy}", document);
            ToolsForGeneration.ReplaceWord("{Staff}", $"{Model.Staff}", document);
            ToolsForGeneration.ReplaceWord("{TotalCost}", $"{Model.TotalCost} ₽", document);
            ToolsForGeneration.ReplaceWord("{StartDate}", $"{Model.StartDate:dd.MM.yyyy}", document);
            ToolsForGeneration.ReplaceWord("{EndDate}", $"{Model.EndDate:dd.MM.yyyy}", document);
            ToolsForGeneration.ReplaceWord("{CurrentDate}", $"{Model.CurrentDate:dd.MM.yyyy}", document);
        }
        #endregion
    }
}
