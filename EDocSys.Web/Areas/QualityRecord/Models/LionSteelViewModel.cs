using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.QualityRecord.Models
{
    public class LionSteelViewModel
    {
        public int Id { get; set; }
        public string FormNo { get; set; }
        public string Title { get; set; }
        public string Section { get; set; }
        public string Type { get; set; }
        public int CompanyId { get; set; }
        public SelectList Companies { get; set; }
        public string CompanyName { get; set; }
        public int DepartmentId { get; set; }
        public SelectList Departments { get; set; }
        public string ProcessName { get; set; }
        public string Location { get; set; }
        public string RetentionPrd { get; set; }
        public string PIC { get; set; }
        public string FilingSystem { get; set; }
        public int RevisionNo { get; set; }

        public List<IFormFile> MyFiles { get; set; }
        public List<AttachmentViewModel> MyAttachments { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RevisionDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string Body { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ArchiveDate { get; set; }
        public int PrintCount { get; set; }  
    }

}