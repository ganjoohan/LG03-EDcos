using EDocSys.Application.Interfaces.Shared;
using System;

namespace EDocSys.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}