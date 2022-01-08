using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class ImageHandler
{
    private readonly IConfiguration _configuration;

    public ImageHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void GetFileFromFtp(string filename)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}{1}", _configuration["FTPConfig:FtpBaseUrl"], filename));
        request.Credentials = new NetworkCredential(_configuration["FTPConfig:FtpUsername"], _configuration["FTPConfig:FtpPassword"]);
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        request.KeepAlive = false;
        request.UsePassive = true;

        using (var ftpresponse = request.GetResponse())
        {
            using (var ftpStream = ftpresponse.GetResponseStream())
            {
                //return ftpStream;
            }
        }

        /*FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(_configuration["FTPConfig:FtpBaseUrl"]); 
        ftpRequest.Credentials = new NetworkCredential(_configuration["FTPConfig:FtpUsername"], _configuration["FTPConfig:FtpPassword"]);
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
        using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
        {
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                List<string> directories = new List<string>();
            }
        }*/
    }

    public string GetFileFromLocal(string filename, bool isthumbnail = false)
    {
        try
        {
            string filepath = string.Format("{0}{1}{2}", isthumbnail ? _configuration["LocalThumbnailPath"] : _configuration["LocalFilePath"], filename, isthumbnail ? ".png" : ".png");

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    file.CopyTo(ms);
                    return string.Format("data:{0};base64,{1}", isthumbnail ? "image/png" : "image/png", Convert.ToBase64String(ms.ToArray()));
                }
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
