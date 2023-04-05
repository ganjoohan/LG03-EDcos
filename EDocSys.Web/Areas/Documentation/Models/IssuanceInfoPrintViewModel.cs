using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class IssuanceInfoPrintViewModel
    {

        public int Id { get; set; }
        public int IssInfoId { get; set; }
        public string RecipientName { get; set; }
        public bool IsPrinted { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PrintedDate { get; set; }
        public string PrintedBy { get; set; }
        public bool IsReturned { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ReturnedDate { get; set; }
        public string ReturnedBy { get; set; }
        public bool IsActive { get; set; }

        public string DocNo { get; set; }

        public string DocUrl { get; set; }

        public string IssId { get; set; }

    }

}