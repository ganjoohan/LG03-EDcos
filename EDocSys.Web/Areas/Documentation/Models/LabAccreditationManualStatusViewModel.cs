using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class LabAccreditationManualStatusViewModel
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int LabAccreditationManualId { get; set; }
        public int DocManualCompanyId { get; set; }

        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
        public virtual LabAccreditationManual LabAccreditationManual { get; set; }

    }
}
