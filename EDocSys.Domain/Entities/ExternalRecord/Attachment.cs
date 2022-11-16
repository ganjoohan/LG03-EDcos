using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Domain.Entities.DocumentationMaster;
using System;
using System.Collections.Generic;

namespace EDocSys.Domain.Entities.ExternalRecord
{
    public class Attachment : AuditableEntity
    {
       public string FileName { get; set; }        
        public int FileSize { get; set; }
        public string FileType { get; set; }
        public string FileLoc { get; set; }
        public int DocId { get; set; }
        public string DocName { get; set; }
        public bool IsActive { get; set; }
        public string FileNameBatch { get; set; }
    }
}
