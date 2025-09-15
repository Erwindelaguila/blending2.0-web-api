using ClosedXML.Excel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Function.Blending.Upload.Helpers.Excel;

    public static class ExcelValidateHelper
    {
        /// <summary>
        /// Asegura que un archivo Excel esté en formato .xlsx válido.
        /// Si el archivo es .xls, lo convierte. Si es .xlsx válido, lo retorna igual.
        /// Lanza excepción si el archivo no es un Excel válido.
        /// </summary>
        /// <param name="fileBytes">Contenido del archivo Excel como byte[]</param>
        /// <returns>Archivo en formato .xlsx</returns>
        /// <exception cref="InvalidDataException">Cuando el archivo no es .xls ni .xlsx válido</exception>
        public static async Task<byte[]> EnsureXlsxAsync(byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                throw new ArgumentException("El archivo está vacío.", nameof(fileBytes));

            if (IsXls(fileBytes))
            {
                return ConvertXlsToXlsx(fileBytes);
            }

            if (await IsValidXlsxAsync(fileBytes))
            {
                return fileBytes;
            }

            throw new InvalidDataException("El archivo no es un Excel válido (.xls o .xlsx).");
        }

        /// <summary>
        /// Detecta si el archivo tiene encabezado binario típico de un .xls
        /// </summary>
        private static bool IsXls(byte[] bytes)
        {
            return bytes.Length >= 8 && bytes[0] == 0xD0 && bytes[1] == 0xCF;
        }

        /// <summary>
        /// Verifica si el archivo puede abrirse como un .xlsx válido
        /// </summary>
        private static async Task<bool> IsValidXlsxAsync(byte[] bytes)
        {
            try
            {
                using var stream = new MemoryStream(bytes);
                using var _ = new XLWorkbook(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Convierte un archivo .xls a .xlsx usando NPOI
        /// </summary>
        private static byte[] ConvertXlsToXlsx(byte[] xlsBytes)
        {
            using var inputStream = new MemoryStream(xlsBytes);
            var hssfWorkbook = new HSSFWorkbook(inputStream);
            var xssfWorkbook = new XSSFWorkbook();

            for (int i = 0; i < hssfWorkbook.NumberOfSheets; i++)
            {
                var sourceSheet = hssfWorkbook.GetSheetAt(i);
                var targetSheet = xssfWorkbook.CreateSheet(sourceSheet.SheetName);

                CopySheetContent(sourceSheet, targetSheet);
            }

            using var output = new MemoryStream();
            xssfWorkbook.Write(output, true);
            return output.ToArray();
        }

        /// <summary>
        /// Copia el contenido de una hoja .xls a una hoja .xlsx
        /// </summary>
        private static void CopySheetContent(ISheet sourceSheet, ISheet targetSheet)
        {
            for (int rowIndex = sourceSheet.FirstRowNum; rowIndex <= sourceSheet.LastRowNum; rowIndex++)
            {
                var sourceRow = sourceSheet.GetRow(rowIndex);
                if (sourceRow == null) continue;

                var targetRow = targetSheet.CreateRow(rowIndex);

                for (int colIndex = sourceRow.FirstCellNum; colIndex < sourceRow.LastCellNum; colIndex++)
                {
                    var sourceCell = sourceRow.GetCell(colIndex);
                    if (sourceCell == null) continue;

                    var targetCell = targetRow.CreateCell(colIndex);
                    CopyCellValue(sourceCell, targetCell);
                }
            }
        }

        /// <summary>
        /// Copia el valor de una celda de .xls a .xlsx
        /// </summary>
        private static void CopyCellValue(ICell source, ICell target)
        {
            switch (source.CellType)
            {
                case CellType.String:
                    target.SetCellValue(source.StringCellValue);
                    break;
                case CellType.Numeric:
                    target.SetCellValue(source.NumericCellValue);
                    break;
                case CellType.Boolean:
                    target.SetCellValue(source.BooleanCellValue);
                    break;
                case CellType.Formula:
                    target.SetCellFormula(source.CellFormula);
                    break;
                case CellType.Blank:
                    target.SetBlank();
                    break;
                default:
                    target.SetCellValue(source.ToString());
                    break;
            }
        }
    }