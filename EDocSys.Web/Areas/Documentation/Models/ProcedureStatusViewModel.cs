using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class ProcedureStatusViewModel
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ProcedureId { get; set; }
        public int ProcedureCompanyId { get; set; }

        //public Procedure Procedure { get; set; }
        public int DocumentStatusId { get; set; }
        public virtual DocumentStatus DocumentStatus { get; set; }
        public virtual Procedure Procedure { get; set; }

    }
}
