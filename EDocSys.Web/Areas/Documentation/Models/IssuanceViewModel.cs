using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class IssuanceViewModel
    {
        public int Id { get; set; }
        public string DOCNo { get; set; }

        public string TitleName { get; set; }

        public int CompanyId { get; set; }
        public SelectList Companies { get; set; }
        public string CompanyName { get; set; }

        public int DepartmentId { get; set; }
        public SelectList Departments { get; set; }
        public string ProcessName { get; set; }

        public IEnumerable<string> IssuanceStatus { get; set; }

        public string IssuanceStatusView { get; set; }

        public List<IssuanceInfoViewModel> IssuanceInfo { get; set; } = new List<IssuanceInfoViewModel>();

        public string RequestedBy { get; set; }
        public string VerifiedBy { get; set; }
        public string VerifiedName { get; set; }
        public string ApprovedBy { get; set; }
        public string AcknowledgedBy { get; set; }

        public SelectList UserList { get; set; }
        public SelectList UserListVer { get; set; }
        public SelectList UserListApp { get; set; }
        public SelectList UserListAck { get; set; }

        public string RequestedByPosition { get; set; }
        public string PositionVer { get; set; }
        public string PositionApp { get; set; }
        public string PositionAck { get; set; }

        public int PrintCount { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }

        public string DOCStatus { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RequestedByDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateVerified { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateApproved { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateAcknowledged { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ArchiveDate { get; set; }
    }

}