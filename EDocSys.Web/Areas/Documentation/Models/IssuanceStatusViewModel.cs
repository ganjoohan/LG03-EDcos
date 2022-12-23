using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class IssuanceStatusViewModel
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int IssuanceId { get; set; }
        public int IssuanceCompanyId { get; set; }

        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
        public virtual Issuance Issuance { get; set; }

    }
}
