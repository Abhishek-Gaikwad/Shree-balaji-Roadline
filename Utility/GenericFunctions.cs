using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using ExcelDataReader;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace Trans9.Utility
{
    public class GenericFunctions
    {
        public static readonly string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        public static MemoryStream GetExcelData<T>(XLWorkbook wb, List<T> tbl, String sheetname = "sheet 1", List<string> headers = null, bool showHeader = false, bool headerIncluded = false)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                if (tbl.Any())
                {
                    var ws = wb.Worksheets.Add(sheetname);
                    if (showHeader)
                    {
                        for (int i = 0; i < headers.Count; i++)
                        {
                            string header = (headers[i].ToLower().Contains("cgst") || headers[i].ToLower().Contains("sgst")) ? $"{headers[i]}[9%]" : headers[i];
                            ws.Cell(1, i + 1).Value = header;
                        }

                        ws.Cell(2, 1).InsertData(tbl);
                    }
                    else
                    {
                        if (headerIncluded)
                            ws.Cell(2, 1).InsertData(tbl);
                        else
                            ws.Cell(1, 1).InsertData(tbl);
                    }

                    wb.SaveAs(stream);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return stream;
        }

        public static string NumberToWord(decimal amount)
        {
            string toWord = string.Empty;

            string[] a = { "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };
            string[] b = { "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINTEEN" };
            string[] c = { "TEN", "TWENTY", "THIRTY", "FOURTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINTY" };
            string[] d = { "zero", "TEN", "HUNDRED", "THOUSAND", "THOUSAND", "LAKHS", "LAKHS", "CRORES", "CRORES", "HUNDRED AND" };
            long number, temp = 0, strlen, value = 0;
            try
            {
                number = Convert.ToInt64(amount);
                Boolean flag = false;
                if (number <= 9999999999)
                {
                    while (number >= 0)
                    {
                        strlen = number.ToString().Length;
                        if (number > 0)
                        {
                            value = (long)Math.Pow(10, strlen - 1);
                            temp = number / value;
                        }
                        switch (strlen)
                        {
                            case 1:
                                toWord += " " + a[temp - 1] + " ";
                                break;
                            case 2:
                                if (number >= 10 && number < 20)
                                {
                                    number = number % value;
                                    if (number == 0)
                                    {
                                        toWord += " " + b[number] + " ";
                                    }
                                    else
                                    {
                                        toWord += " " + b[number] + " ";
                                    }
                                    flag = true;
                                }
                                else
                                {
                                    toWord += " " + c[temp - 1] + " ";
                                }
                                break;
                            case 3:
                                toWord += a[temp - 1] + " " + d[strlen - 1];
                                break;
                            case 4:
                            case 6:
                            case 8:
                            case 10:
                                toWord += " " + a[temp - 1] + " " + d[strlen - 1] + " ";
                                break;
                            case 5:
                            case 7:
                            case 9:
                            case 11:
                                value = (long)Math.Pow(10, strlen - 2);
                                temp = number / value;
                                if (temp >= 10 && temp < 20)
                                {
                                    temp = temp % 10;
                                    if (temp == 0)
                                    {
                                        toWord += " " + c[temp] + " " + d[strlen - 1] + " ";
                                    }
                                    else
                                    {
                                        toWord += " " + b[temp] + " " + d[strlen - 1] + " ";
                                    }
                                }
                                else
                                {
                                    long i = temp % 10;
                                    temp = temp / 10;
                                    toWord += " " + c[temp - 1] + " ";
                                    if (i > 0)
                                    {
                                        toWord += " " + a[i - 1] + " ";
                                    }
                                    toWord += " " + d[strlen - 1] + " ";
                                }
                                break;
                        }
                        if (number == value)
                        {
                            toWord += " ";
                            break;
                        }
                        else
                        {
                            number = number % value;
                        }
                        if (number == 0 || flag == true)
                        {
                            break;
                        }
                        flag = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return (toWord += " RUPEES ONLY");
        }

        public static DataTable ReadExcel(IFormFile dataFile)
        {
            DataTable dt = new DataTable();
            DataRow row;
            DataTable dt_ = new DataTable();
            try
            {
                Stream stream = dataFile.OpenReadStream();

                // We return the interface, so that
                IExcelDataReader reader = null;

                if (dataFile.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (dataFile.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                /*else if (dataFile.FileName.EndsWith(".csv"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }*/

                int fieldcount = reader.FieldCount;

                dt_ = reader.AsDataSet().Tables[0];

                for (int i = 0; i < dt_.Columns.Count; i++)
                {
                    dt.Columns.Add(dt_.Rows[0][i].ToString());
                }

                for (int row_ = 1; row_ < dt_.Rows.Count; row_++)
                {
                    row = dt.NewRow();
                    for (int col = 0; col < dt_.Columns.Count; col++)
                    {
                        row[col] = dt_.Rows[row_][col];
                    }
                    dt.Rows.Add(row);
                }

                reader.Close();
                reader.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
            return dt;
        }
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static long ToUnixTime()
        {
            TimeSpan t = DateTime.Now - new DateTime(1970, 1, 1);
            int unixepoc = (int)t.TotalDays;
            return unixepoc;
        }

        public static string[] UploadDocuments(IHostEnvironment _env, List<IFormFile> formFiles, string dir)
        {
            int i = 0;
            string[] docs = new string[formFiles.Count];
            try
            {
                foreach (var file in formFiles)
                {
                    var path = $"docs/{dir}";
                    string filePath = Path.Combine(_env.ContentRootPath, $"wwwroot/{path}");

                    //create folder if not exist
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                    filePath = Path.Combine(filePath, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);

                    }
                    docs[i++] = $"{path}/{file.FileName}";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return docs;
        }

        public static string[] UploadDocumentByBase64(IHostEnvironment _env, string cameraPhoto, string dir)
        {
            int i = 0;
            string fileName = "photo.jpg";
            string[] docs = new string[1];
            try
            {

                    var path = $"docs/{dir}";
                    string filePath = Path.Combine(_env.ContentRootPath, $"wwwroot/{path}");

                    //create folder if not exist
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                cameraPhoto = cameraPhoto.Substring(cameraPhoto.IndexOf(',') + 1);
                // Convert Base64 string to byte array
                byte[] imageBytes = Convert.FromBase64String(cameraPhoto);

                // Create memory stream from byte array
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    // Create image from memory stream
                    Image image = Image.FromStream(ms);

                    // Output the image
                    image.Save(Path.Combine(filePath, fileName), ImageFormat.Png);

                }

                docs[i++] = $"{path}/{fileName}";
            }
            catch (Exception ex)
            {
                throw;
            }
            return docs;
        }
    }
}
