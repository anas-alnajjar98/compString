using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace compString
{
    public partial class compreser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            string uploadDirectory = Server.MapPath("~/uploads/textfiles");

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            if (upload_text_file.HasFiles)
            {
                foreach (HttpPostedFile file in upload_text_file.PostedFiles)
                {
                    string filePath = Path.Combine(uploadDirectory, Path.GetFileName(file.FileName));

                    if (file.ContentType == "text/plain")
                    {
                        file.SaveAs(filePath);
                        string fileContent = File.ReadAllText(filePath);
                        string compressedContent = Zip(fileContent);
                        string compressedFilePath = Path.Combine(uploadDirectory, Path.GetFileNameWithoutExtension(file.FileName) + ".gz");
                        File.WriteAllText(compressedFilePath, compressedContent);
                    }
                    else if (Path.GetExtension(file.FileName).ToLower() == ".gz")
                    {
                        file.SaveAs(filePath);
                    }
                    else
                    {
                        // Handle invalid file type
                    }
                }
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            string downloadDirectory = Server.MapPath("~/downloads/textfiles");

            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }

            if (upload_text_file.HasFiles)
            {
                foreach (HttpPostedFile file in upload_text_file.PostedFiles)
                {
                    if (Path.GetExtension(file.FileName).ToLower() == ".gz")
                    {
                        string filePath = Path.Combine(downloadDirectory, Path.GetFileName(file.FileName));
                        file.SaveAs(filePath);

                        string fileContent = File.ReadAllText(filePath);
                        string decompressedContent = UnZip(fileContent);
                        string decompressedFilePath = Path.Combine(downloadDirectory, Path.GetFileNameWithoutExtension(file.FileName) + ".txt");
                        File.WriteAllText(decompressedFilePath, decompressedContent);
                    }
                    else
                    {
                        // Handle invalid file type
                    }
                }
            }
        }

        public static string Zip(string value)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(value);

            using (var ms = new MemoryStream())
            {
                using (var sw = new GZipStream(ms, CompressionMode.Compress))
                {
                    sw.Write(byteArray, 0, byteArray.Length);
                }

                byte[] compressedData = ms.ToArray();
                return Convert.ToBase64String(compressedData);
            }
        }

        public static string UnZip(string value)
        {
            byte[] byteArray = Convert.FromBase64String(value);

            using (var ms = new MemoryStream(byteArray))
            {
                using (var sr = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (var resultMs = new MemoryStream())
                    {
                        sr.CopyTo(resultMs);
                        byte[] decompressedData = resultMs.ToArray();
                        return Encoding.UTF8.GetString(decompressedData);
                    }
                }
            }
        }
    }
}
