﻿using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Web.Hosting;
using System.ComponentModel.DataAnnotations;
namespace MVC_DATABASE.Models.ViewModels
{
    public class RFIVendorRespond
    {
        public class RFIList
        {
            public RFI RFI { get; set; }
            public RFIINVITE rfiInvite { get; set; }
            public ICollection<RFIINVITE> rfiInviteList { get; set; }

            [Required]
            [FileExtensions(Extensions = "xlsx,xls")]
            public HttpPostedFileBase File { get; set; }

            [FileExtensions(Extensions = "xlsx,xls,pdf")]
            public HttpPostedFileBase Catalog { get; set; }
        }

        public class FileNames_RFIResponse
        {
            public int FileId { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        public List<FileNames_RFIResponse> GetFiles()
        {
            List<FileNames_RFIResponse> fileList = new List<FileNames_RFIResponse>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/RFIs"));

            int i = 0;
            foreach(var file in dirInfo.GetFiles())
            {
                fileList.Add(new FileNames_RFIResponse()
                {
                    FileId = i + 1, FileName = file.Name, FilePath = dirInfo.FullName+@"\"+file.Name
                });
            }
           
            return fileList;

        }
            
    }    

}
