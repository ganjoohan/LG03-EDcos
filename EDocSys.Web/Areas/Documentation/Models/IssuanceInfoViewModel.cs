using EDocSys.Domain.Entities.Documentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDocSys.Web.Areas.Documentation.Models
{
    public class IssuanceInfoViewModel
    {
        public int Id { get; set; }
        public int HId { get; set; }
        public int No { get; set; }
        public string DOCId { get; set; }

        public string DOCNo { get; set; } //temp
        public SelectList DOCNos { get; set; }
        public SelectList DocTypes { get; set; }

        public string DocType { get; set; }
        public string RecipientName1 { get; set; }
        public string RecipientName2 { get; set; }
        public string RecipientName3 { get; set; }
        public string RecipientName4 { get; set; }
        public string RecipientName5 { get; set; }
        public string RecipientName6 { get; set; }
        public string Purpose { get; set; }
        public string Amendment { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsAmend { get; set; }

        public string DOCUrl { get; set; } //temp

        public bool Deleted { get; set; } = false; //temp

    }

}