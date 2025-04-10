﻿using System;

namespace EDocSys.Application.Features.Procedures.Queries.GetByParameter
{
    public class GetProceduresByParameterResponse
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
        public DateTime? EffectiveDate { get; set; }
        public DateTime? RevisionDate { get; set; }
        public int? RevisionNo { get; set; }
        public DateTime? EstalishedDate { get; set; }
        public int DepartmentId { get; set; }
        public string ProcessName { get; set; }
       
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string ProcedureStatusView { get; set; }
        public string ApprovedBy { get; set; }
        public string Concurred1 { get; set; }
        public string Concurred2 { get; set; }
       
        public string PreparedBy { get; set; }
        public string PreparedByPosition { get; set; }
        public DateTime? PreparedByDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int ArchiveId { get; set; }
        public int PrintCount { get; set; }
        public DateTime? ArchiveDate { get; set; }

    }
}


