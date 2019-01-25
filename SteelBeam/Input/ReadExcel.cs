using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace SteelBeam
{
    public class ReadExcel
    {
        public const int WIDTH_COL = 5;
        public const int HEIGHT_COL = 6;
        public const int LENGTH_COL = 29;
        public const int NUM_STEEL_FIRST_LAYER = 12;
        public const int DIA_STEEL_FIRST_LAYER = 14;
        public const int NUM_STEEL_SECOND_LAYER = 16;
        public const int DIA_STEEL_SECOND_LAYER = 18;
        public const int THEP_DAI_INFO = 27;
        public const int THEP_DAI_2_INFO = 28;

        private static string CellValue(Excel.Worksheet sheet,int row, int col)
        {
            return ((Excel.Range)sheet.Cells[row, col]).Value2.ToString();
        }

        private static bool IsValidCell(Excel.Worksheet sheet, int row, int col)
        {
            return null != sheet.Cells[row, col].Value2;
        }

        private static int GetIntValue(Excel.Worksheet sheet, int row, int col)
        {
            int value = 0;
            var str = CellValue(sheet, row, col);
            if (!Int32.TryParse(CellValue(sheet, row, col), out value))
                throw new Exception("Dong " + (row + 1).ToString() + ", Cot " + (col + 1).ToString() + " bi loi!");

            return value;
        }

        private static int ConvertInt(string str)
        {
            int value = 0;
            if (!Int32.TryParse(str, out value))
                throw new Exception("Loi convert chuoi thanh so!");

            return value;
        }

        public static List<BeamData> DoReadExcel(string path)
        {
            const int offset_row = 5;
            try
            {
                Excel.Application excel_app = new Excel.Application();

                excel_app.DisplayAlerts = false; //Don't want Excel to display error messageboxes

                Excel.Workbook workbook = excel_app.Workbooks.Open(path,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing); //This opens the file

                Excel.Worksheet sheet = (Excel.Worksheet)workbook.Sheets.get_Item(1); //Get the first sheet in the file

                int last_row = sheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;
                int last_col = sheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Column;

                List<BeamData> beam_data = new List<BeamData>();
                for(int i = offset_row; i < last_row; )
                {
                    BeamData data = new BeamData();

                    try
                    {
                        data.Width = GetIntValue(sheet, i, WIDTH_COL);
                        data.Height = GetIntValue(sheet, i, HEIGHT_COL);
                        data.Length = GetIntValue(sheet, i, LENGTH_COL);
                    }
                    catch(Exception ex)
                    {
                        i++;
                        continue;
                    }

                    {
                        data.giua_data.number_steel_first_layer = GetIntValue(sheet, i, NUM_STEEL_FIRST_LAYER);
                        data.giua_data.first_layer_diameter = GetIntValue(sheet, i, DIA_STEEL_FIRST_LAYER);
                        data.giua_data.number_steel_second_layer = GetIntValue(sheet, i, NUM_STEEL_SECOND_LAYER);
                        data.giua_data.second_layer_diameter = GetIntValue(sheet, i, DIA_STEEL_SECOND_LAYER);

                        string thep_dai_info = CellValue(sheet, i, THEP_DAI_INFO);
                        if (thep_dai_info[0] != '2' && thep_dai_info[0] != '3' && thep_dai_info[0] != '4')
                        {
                            i += 2;
                            continue;
                        }

                        data.giua_data.type_beam = ConvertInt(thep_dai_info.Substring(0, 1));
                        data.giua_data.dai_diameter = ConvertInt(thep_dai_info.Substring(2, 2));
                        data.giua_data.spacing = ConvertInt(thep_dai_info.Substring(5));

                        if(IsValidCell(sheet, i, THEP_DAI_2_INFO))
                        {
                            string thep_dai_info2 = CellValue(sheet, i, THEP_DAI_2_INFO);
                            data.giua_data.dai_diameter2 = ConvertInt(thep_dai_info2.Substring(0, 1));
                            data.giua_data.spacing2 = ConvertInt(thep_dai_info2.Substring(2));
                        }
                    }

                    i++;

                    {
                        data.dau_goi_data.number_steel_first_layer = GetIntValue(sheet, i, NUM_STEEL_FIRST_LAYER);
                        data.dau_goi_data.first_layer_diameter = GetIntValue(sheet, i, DIA_STEEL_FIRST_LAYER);
                        data.dau_goi_data.number_steel_second_layer = GetIntValue(sheet, i, NUM_STEEL_SECOND_LAYER);
                        data.dau_goi_data.second_layer_diameter = GetIntValue(sheet, i, DIA_STEEL_SECOND_LAYER);

                        string thep_dai_info = CellValue(sheet, i, THEP_DAI_INFO);
                        if (thep_dai_info[0] != '2' && thep_dai_info[0] != '3' && thep_dai_info[0] != '4')
                        {
                            i++;
                            continue;
                        }

                        data.dau_goi_data.type_beam = ConvertInt(thep_dai_info.Substring(0, 1));
                        data.dau_goi_data.dai_diameter = ConvertInt(thep_dai_info.Substring(2, 2));
                        data.dau_goi_data.spacing = ConvertInt(thep_dai_info.Substring(5));

                        if (IsValidCell(sheet, i, THEP_DAI_2_INFO))
                        {
                            string thep_dai_info2 = CellValue(sheet, i, THEP_DAI_2_INFO);
                            data.dau_goi_data.dai_diameter2 = ConvertInt(thep_dai_info2.Substring(0, 1));
                            data.dau_goi_data.spacing2 = ConvertInt(thep_dai_info2.Substring(2));
                        }
                    }

                    i++;

                    beam_data.Add(data);
                }

                workbook.Close(false, Type.Missing, Type.Missing);
                excel_app.Quit();

                return beam_data;
            }
            catch(Exception ex)
            {

            }

            return new List<BeamData>();
        }
    }
}
