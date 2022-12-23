using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.QualityRecord.Models
{
    public class AttachmentViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNameBatch { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
        public string FileLoc { get; set; }
        public int DocId { get; set; }
        public string DocName { get; set; }
        public bool IsActive { get; set; } = true;

        public bool Deleted { get; set; } = false;
    }

}