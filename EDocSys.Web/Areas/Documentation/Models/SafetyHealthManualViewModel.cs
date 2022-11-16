using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class SafetyHealthManualViewModel
    {
        public int Id { get; set; }
        public string DOCNo { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Body { get; set; }
       
        public string ProcessName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EffectiveDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RevisionDate { get; set; }
        
        public int? RevisionNo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EstalishedDate { get; set; }

        public int CompanyId { get; set; }
        public SelectList Companies { get; set; }
        public string CompanyName { get; set; }


        public IEnumerable<string> SafetyHealthManualStatus { get; set; }

        public string SafetyHealthManualStatusView { get; set; }

        public string Concurred1 { get; set; }
        public string Concurred1Name { get; set; }
        
        public string Concurred2 { get; set; }
        public string Concurred2Name { get; set; }
        public string ApprovedBy { get; set; }

        public SelectList UserList { get; set; }
        public SelectList UserListC1 { get; set; }
        public SelectList UserListC2 { get; set; }
        public SelectList UserListAPP { get; set; }

        public string PositionC1 { get; set; }
        public string PositionC2 { get; set; }
        public string PositionApp { get; set; }

        public string PreparedBy { get; set; }
        public string PreparedByPosition { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public int PrintCount { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PreparedByDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateApprovedC1 { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateApprovedC2 { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateApprovedAPP { get; set; }
    }

}