using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class WIViewModel
    {
        public int Id { get; set; }
        public string WSCPNo { get; set; }
        public string Title { get; set; }
        public string SOPNo { get; set; }
        public string WINo { get; set; }
        public string Purpose { get; set; }
        public string Scope { get; set; }
        public string Definition { get; set; }
        public string Body { get; set; }
        public int DepartmentId { get; set; }
        public SelectList Departments { get; set; }
        public string ProcessName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EffectiveDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RevisionDate { get; set; }
        
        public int? RevisionNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EstalishedDate { get; set; }

        public int CompanyId { get; set; }
        public SelectList Companies { get; set; }
        public string CompanyName { get; set; }

        public int ProcedureId { get; set; }

    }
}