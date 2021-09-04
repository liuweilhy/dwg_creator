using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace DwgCreator
{
    /// <summary>
    /// Excel 帮助类
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// 将 Excel 转换为 DataTable
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetIndex">表下标 从0开始</param>
        /// <param name="rowIndex">行下标 从1开始</param>
        /// <param name="header"> 是否排除excel中第一行</param>
        /// <returns>DataTable</returns>
        public static DataTable ExcelToDataTable(string filePath, int sheetIndex, int rowIndex, bool header = true)
        {
            var ext = CheckExt(filePath);

            if (ext == "208207")//xls
                return ToDataTable2003(filePath, sheetIndex, rowIndex, header);

            if (ext == "8075")//xlsx
                return ToDataTable2007(filePath, sheetIndex, header);

            return new DataTable();
        }

        #region Private Method

        /// <summary>
        /// 通过文件头信息判断文件类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string CheckExt(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var r = new BinaryReader(fs);

            var bx = string.Empty;

            try
            {
                byte buffer = r.ReadByte();
                bx = buffer.ToString();
                buffer = r.ReadByte();
                bx += buffer.ToString();

                // 208207 xls
                // 8075 xlsx
            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                r.Close();
                fs.Close();
            }
            return bx;
        }

        /// <summary>
        /// 将 2003 Excel 转换为 DataTable
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetIndex">表下标 从0开始</param>
        /// <param name="rowIndex">行下标 从1开始</param>
        /// <param name="header"> excel中第一行是否属于列</param>
        /// <returns>DataTable</returns>
        private static DataTable ToDataTable2003(string filePath, int sheetIndex, int rowIndex, bool header)
        {
            var dt = new DataTable(Path.GetFileNameWithoutExtension(filePath) + "_Sheet" + sheetIndex);

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = new HSSFWorkbook(file);
                var sheet = workbook.GetSheetAt(sheetIndex);

                sheet.SetColumnHidden(0, false);
                sheet.SetColumnHidden(1, false);
                sheet.SetColumnHidden(2, false);

                var rows = sheet.GetRowEnumerator();
                for (int i = 0; i < rowIndex; i++)
                {
                    rows.MoveNext();
                }

                var row = (HSSFRow)rows.Current;

                for (var i = 0; i < row.LastCellNum; i++)
                {
                    var columnName = header ? row.GetCell(i).StringCellValue : i.ToString();
                    dt.Columns.Add(columnName, typeof(string));
                }

                if (!header)
                {
                    var dataRow = dt.NewRow();
                    for (var i = 0; i < row.LastCellNum; i++)
                    {
                        var item = row.GetCell(i);

                        dataRow[i] = GetRow(item);
                    }
                    dt.Rows.Add(dataRow);
                }

                while (rows.MoveNext())
                {
                    row = (HSSFRow)rows.Current;
                    var dataRow = dt.NewRow();
                    bool isNull = false;//是否空行
                    for (var i = 0; i < row.LastCellNum; i++)
                    {
                        var item = row.GetCell(i);
                        var _row = GetRow(item);
                        if (_row != null)
                        {
                            dataRow[i] = _row;
                            if (!string.IsNullOrWhiteSpace(dataRow[i].ToString()))
                            {
                                isNull = true;//设置非空
                            }
                        }
                    }
                    if (isNull)
                    {
                        dt.Rows.Add(dataRow);
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 将 2007 Excel 转换为 DataTable
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="index">表下标 从0开始</param>
        /// <param name="header"> excel中第一行是否属于列</param>
        /// <returns>DataTable</returns>
        private static DataTable ToDataTable2007(string filePath, int index, bool header)
        {
            var dt = new DataTable(Path.GetFileNameWithoutExtension(filePath) + "_Sheet" + index);

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(file);
                var sheet = workbook.GetSheetAt(index);

                var rows = sheet.GetRowEnumerator();
                rows.MoveNext();
                var row = (XSSFRow)rows.Current;

                for (var i = 0; i < row.LastCellNum; i++)
                {
                    var columnName = header ? row.GetCell(i).StringCellValue : i.ToString();
                    dt.Columns.Add(columnName, typeof(string));
                }

                if (!header)
                {
                    var dataRow = dt.NewRow();
                    for (var i = 0; i < row.LastCellNum; i++)
                    {
                        var item = row.GetCell(i);

                        dataRow[i] = GetRow(item);
                    }
                    dt.Rows.Add(dataRow);
                }

                while (rows.MoveNext())
                {
                    row = (XSSFRow)rows.Current;
                    var dataRow = dt.NewRow();
                    bool isNull = false;//是否空行
                    for (var i = 0; i < row.LastCellNum; i++)
                    {
                        var item = row.GetCell(i);
                        var _row = GetRow(item);
                        if (_row != null)
                        {
                            dataRow[i] = _row;
                            if (!string.IsNullOrWhiteSpace(dataRow[i].ToString()))
                            {
                                isNull = true;//设置非空
                            }
                        }
                    }
                    if (isNull)
                    {
                        dt.Rows.Add(dataRow);
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetRow(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;
                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;
                case CellType.String: //STRING:
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }

        #endregion

    }
}
