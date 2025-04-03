using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using EDocSys.Application.DTOs.Mail;
using EDocSys.Application.Interfaces.Contexts;
using EDocSys.Application.Interfaces.Shared;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Infrastructure.DbContexts;
using EDocSys.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using static EDocSys.Application.Constants.Permissions;

namespace EDocSys.Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class EmailReminderJob : IJob
    {
        private readonly ILogger<EmailReminderJob> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityContext _identityContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMailService _mailService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        // Configuration settings from appsettings.json
        private int _reminderThresholdDays;
        private bool _sendCopyToAdmin;
        private string _adminEmail;
        private string _baseUrl;

        public EmailReminderJob(
            ILogger<EmailReminderJob> logger,
            UserManager<ApplicationUser> userManager,
            IdentityContext identityContext,
            RoleManager<IdentityRole> roleManager,
            IServiceProvider serviceProvider,
            IMailService mailService,
            IMediator mediator,
            IMapper mapper,
            IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _identityContext = identityContext;
            _roleManager = roleManager;
            _serviceProvider = serviceProvider;
            _mailService = mailService;
            _mediator = mediator;
            _mapper = mapper;
            _configuration = configuration;

            // Load configuration values
            var emailConfig = _configuration.GetSection("EmailReminders");
            _reminderThresholdDays = emailConfig.GetValue<int>("ReminderThresholdDays");
            _sendCopyToAdmin = emailConfig.GetValue<bool>("SendCopyToAdmin");
            _adminEmail = emailConfig.GetValue<string>("AdminEmail");
            _baseUrl = emailConfig.GetValue<string>("BaseUrl");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //start the job
            _logger.LogInformation("Document reminder scheduler triggered.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                // Process different document types
                await ProcessProcedureReminders(dbContext);
                await ProcessSOPReminders(dbContext);
                //await ProcessWIReminders(dbContext);
                await ProcessDocumentManualReminders(dbContext);
                await ProcessEnvironmentalManualReminders(dbContext);
                await ProcessLabAccreditationManualReminders(dbContext);
                await ProcessQualityManualReminders(dbContext);
                await ProcessSafetyHealthManualReminders(dbContext);

                // Send completion notification to admin
                await SendCompletionNotification();
            }

            _logger.LogInformation("Document reminder scheduler completed.");
        }

        #region Document Type Processors

        private async Task ProcessProcedureReminders(IApplicationDbContext dbContext)
        {
            _logger.LogInformation("Processing procedure reminders with enhanced logic...");

            try
            {
                var currentDate = DateTime.Now;
                var thresholdDate = currentDate.AddDays(-_reminderThresholdDays);

                // Get all active procedures with their statuses
                var procedures = await dbContext.Procedures
                    .Include(p => p.ProcedureStatus)
                    .Include(p => p.Department)
                    .Where(p => p.IsActive == true && p.ArchiveDate == null)
                    .ToListAsync();

                _logger.LogInformation($"Found {procedures.Count} active procedures to check for reminders");

                // Process each procedure
                foreach (var procedure in procedures)
                {
                    // Get the latest status
                    var latestStatus = procedure.ProcedureStatus
                        .OrderByDescending(s => s.CreatedOn)
                        .FirstOrDefault();

                    // Skip if no status or if approved
                    if (latestStatus == null || latestStatus.DocumentStatusId == 4)
                    {
                        continue;
                    }

                    // Check if enough time has passed since the last status update
                    if (latestStatus.CreatedOn > thresholdDate)
                    {
                        // Not enough time has passed, skip this procedure
                        continue;
                    }

                    // Send reminder based on current status
                    await SendProcedureReminderBasedOnStatus(procedure, latestStatus.DocumentStatusId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing procedure reminders");
            }
        }
        private async Task ProcessSOPReminders(IApplicationDbContext dbContext)
        {
            _logger.LogInformation("Processing SOP reminders...");

            try
            {
                var currentDate = DateTime.Now;
                var thresholdDate = currentDate.AddDays(-_reminderThresholdDays);

                // Get all active SOPs with their statuses
                var sops = await dbContext.StandardOperatingPractices
                    .Include(s => s.SOPStatus)
                    .Include(s => s.Department)
                    .Where(s => s.IsActive == true && s.ArchiveDate == null)
                    .ToListAsync();

                _logger.LogInformation($"Found {sops.Count} active SOPs to check for reminders");

                // Process each SOP - similar logic to procedures but adapted for SOPs
                foreach (var sop in sops)
                {
                    var latestStatus = sop.SOPStatus
                        .OrderByDescending(s => s.CreatedOn)
                        .FirstOrDefault();

                    if (latestStatus == null || latestStatus.DocumentStatusId == 4)
                    {
                        continue;
                    }

                    if (latestStatus.CreatedOn > thresholdDate)
                    {
                        continue;
                    }

                    // Implement SOP reminder logic
                    await SendSOPReminderBasedOnStatus(sop, latestStatus.DocumentStatusId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SOP reminders");
            }
        }
        private async Task ProcessWIReminders(IApplicationDbContext dbContext)
        {
            _logger.LogInformation("Processing Work Instruction reminders...");

            try
            {
                var currentDate = DateTime.Now;
                var thresholdDate = currentDate.AddDays(-_reminderThresholdDays);

                // Add a more targeted query instead of loading all WIs first
                var wis = await dbContext.WorkInstructions
                    .Include(s => s.WIStatus.OrderByDescending(status => status.CreatedOn).Take(1))
                    .Include(s => s.Department)
                    .Where(s => s.IsActive == true
                              && s.ArchiveDate == null
                              && s.WIStatus.Any(status => status.CreatedOn <= thresholdDate
                                               && status.DocumentStatusId != 4))
                    .ToListAsync();

                _logger.LogInformation($"Found {wis.Count} active WIs to check for reminders");

                // Process each SOP - similar logic to procedures but adapted for SOPs
                foreach (var wi in wis)
                {
                    var latestStatus = wi.WIStatus
                        .OrderByDescending(s => s.CreatedOn)
                        .FirstOrDefault();

                    if (latestStatus == null || latestStatus.DocumentStatusId == 4)
                    {
                        continue;   
                    }

                    if (latestStatus.CreatedOn > thresholdDate)
                    {
                        continue;
                    }

                    // Implement SOP reminder logic
                    await SendWIReminderBasedOnStatus(wi, latestStatus.DocumentStatusId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing WI reminders");
            }
        }
        private async Task ProcessDocumentManualReminders(IApplicationDbContext dbContext)
        {
            _logger.LogInformation("Processing Document Manual reminders...");

            try
            {
                var currentDate = DateTime.Now;
                var thresholdDate = currentDate.AddDays(-_reminderThresholdDays);

                // Get all active SOPs with their statuses
                var dms = await dbContext.DocumentManuals
                    .Include(s => s.DocumentManualStatus)
                    .Where(s => s.IsActive == true && s.ArchiveDate == null)
                    .ToListAsync();

                _logger.LogInformation($"Found {dms.Count} active DMs to check for reminders");

                foreach (var dm in dms)
                {
                    var latestStatus = dm.DocumentManualStatus
                        .OrderByDescending(s => s.CreatedOn)
                        .FirstOrDefault();

                    if (latestStatus == null || latestStatus.DocumentStatusId == 4)
                    {
                        continue;
                    }

                    if (latestStatus.CreatedOn > thresholdDate)
                    {
                        continue;
                    }

                    // Implement SOP reminder logic
                    await SendDocumentManualReminderBasedOnStatus(dm, latestStatus.DocumentStatusId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Document Manual reminders");
            }
        }
        private async Task ProcessEnvironmentalManualReminders(IApplicationDbContext dbContext)
        {
            _logger.LogInformation("Processing Environmental Manual reminders...");
            try
            {
                var currentDate = DateTime.Now;
                var thresholdDate = currentDate.AddDays(-_reminderThresholdDays);

                // Get all active Environmental Manuals with their statuses
                var ems = await dbContext.EnvironmentalManuals
                    .Include(s => s.EnvironmentalManualStatus)
                    .Where(s => s.IsActive == true && s.ArchiveDate == null)
                    .ToListAsync();

                _logger.LogInformation($"Found {ems.Count} active Environmental Manuals to check for reminders");

                // Process each Environmental Manual
                foreach (var em in ems)
                {
                    var latestStatus = em.EnvironmentalManualStatus
                        .OrderByDescending(s => s.CreatedOn)
                        .FirstOrDefault();

                    if (latestStatus == null || latestStatus.DocumentStatusId == 4)
                    {
                        continue;
                    }

                    if (latestStatus.CreatedOn > thresholdDate)
                    {
                        continue;
                    }

                    // Send reminder based on status
                    await SendEnvironmentalManualReminderBasedOnStatus(em, latestStatus.DocumentStatusId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Environmental Manual reminders");
            }
        }
        private async Task ProcessLabAccreditationManualReminders(IApplicationDbContext dbContext)
        {
            _logger.LogInformation("Processing Lab Accreditation Manual reminders...");
            try
            {
                var currentDate = DateTime.Now;
                var thresholdDate = currentDate.AddDays(-_reminderThresholdDays);

                // Get all active Lab Accreditation Manuals with their statuses
                var lams = await dbContext.LabAccreditationManuals
                    .Include(s => s.LabAccreditationManualStatus)
                    .Where(s => s.IsActive == true && s.ArchiveDate == null)
                    .ToListAsync();

                _logger.LogInformation($"Found {lams.Count} active Lab Accreditation Manuals to check for reminders");

                // Process each Lab Accreditation Manual
                foreach (var lam in lams)
                {
                    var latestStatus = lam.LabAccreditationManualStatus
                        .OrderByDescending(s => s.CreatedOn)
                        .FirstOrDefault();

                    if (latestStatus == null || latestStatus.DocumentStatusId == 4)
                    {
                        continue;
                    }

                    if (latestStatus.CreatedOn > thresholdDate)
                    {
                        continue;
                    }

                    // Send reminder based on status
                    await SendLabAccreditationManualReminderBasedOnStatus(lam, latestStatus.DocumentStatusId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Lab Accreditation Manual reminders");
            }
        }
        private async Task ProcessQualityManualReminders(IApplicationDbContext dbContext)
        {
            _logger.LogInformation("Processing Quality Manual reminders...");
            try
            {
                var currentDate = DateTime.Now;
                var thresholdDate = currentDate.AddDays(-_reminderThresholdDays);

                // Get all active Quality Manuals with their statuses
                var qms = await dbContext.QualityManuals
                    .Include(s => s.QualityManualStatus)
                    .Where(s => s.IsActive == true && s.ArchiveDate == null)
                    .ToListAsync();

                _logger.LogInformation($"Found {qms.Count} active Quality Manuals to check for reminders");

                // Process each Quality Manual
                foreach (var qm in qms)
                {
                    var latestStatus = qm.QualityManualStatus
                        .OrderByDescending(s => s.CreatedOn)
                        .FirstOrDefault();

                    if (latestStatus == null || latestStatus.DocumentStatusId == 4)
                    {
                        continue;
                    }

                    if (latestStatus.CreatedOn > thresholdDate)
                    {
                        continue;
                    }

                    // Send reminder based on status
                    await SendQualityManualReminderBasedOnStatus(qm, latestStatus.DocumentStatusId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Quality Manual reminders");
            }
        }
        private async Task ProcessSafetyHealthManualReminders(IApplicationDbContext dbContext)
        {
            _logger.LogInformation("Processing Safety & Health Manual reminders...");
            try
            {
                var currentDate = DateTime.Now;
                var thresholdDate = currentDate.AddDays(-_reminderThresholdDays);

                // Get all active Safety & Health Manuals with their statuses
                var shms = await dbContext.SafetyHealthManuals
                    .Include(s => s.SafetyHealthManualStatus)
                    .Where(s => s.IsActive == true && s.ArchiveDate == null)
                    .ToListAsync();

                _logger.LogInformation($"Found {shms.Count} active Safety & Health Manuals to check for reminders");

                // Process each Safety & Health Manual
                foreach (var shm in shms)
                {
                    var latestStatus = shm.SafetyHealthManualStatus
                        .OrderByDescending(s => s.CreatedOn)
                        .FirstOrDefault();

                    if (latestStatus == null || latestStatus.DocumentStatusId == 4)
                    {
                        continue;
                    }

                    if (latestStatus.CreatedOn > thresholdDate)
                    {
                        continue;
                    }

                    // Send reminder based on status
                    await SendSafetyHealthManualReminderBasedOnStatus(shm, latestStatus.DocumentStatusId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Safety & Health Manual reminders");
            }
        }

        #endregion



        #region Reminder Senders

        private async Task SendProcedureReminderBasedOnStatus(Procedure procedure, int statusId)
        {
            string recipientEmail = null;
            string statusDescription = null;
            string subject = null;

            switch (statusId)
            {
                case 1: // SUBMITTED: remind company admin
                    var companyAdminEmail = await GetCompanyAdminEmail(procedure.CompanyId);
                    if (!string.IsNullOrEmpty(companyAdminEmail))
                    {
                        recipientEmail = companyAdminEmail;
                        statusDescription = "awaiting format check";
                        subject = "Reminder: Request for Format Check: Procedure " + procedure.WSCPNo;
                    }
                    break;

                case 2: // CONCURRED 1: remind the second concurrer or approver
                    if (!string.IsNullOrEmpty(procedure.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(procedure.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(procedure.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(procedure.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Procedure " + procedure.WSCPNo;
                    break;

                case 3: // CONCURRED 2: remind the approver
                    if (!string.IsNullOrEmpty(procedure.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(procedure.ApprovedBy);
                        statusDescription = "awaiting final approval";
                        subject = "Reminder: Request for Approval: Procedure " + procedure.WSCPNo;
                    }
                    break;

                case 5: // REJECTED: remind department admin
                    var deptAdminEmail = await GetDepartmentAdminEmail(procedure.CompanyId, procedure.DepartmentId);
                    if (!string.IsNullOrEmpty(deptAdminEmail))
                    {
                        recipientEmail = deptAdminEmail;
                        statusDescription = "rejected and requires revision";
                        subject = "Reminder: Action Required for Rejected Procedure " + procedure.WSCPNo;
                    }
                    break;

                case 6: // FORMAT CHECKED: remind the first available approver in the chain
                    if (!string.IsNullOrEmpty(procedure.Concurred1))
                    {
                        recipientEmail = await GetUserEmail(procedure.Concurred1);
                        statusDescription = "awaiting first concurrence";
                    }
                    else if (!string.IsNullOrEmpty(procedure.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(procedure.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(procedure.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(procedure.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Procedure " + procedure.WSCPNo;
                    break;

                default:
                    _logger.LogWarning($"Unhandled status ID {statusId} for procedure {procedure.Id}");
                    return;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning($"Could not determine recipient for procedure {procedure.Id} with status {statusId}");
                return;
            }


            string documentUrl = _baseUrl + "documentation/procedure/preview?id=" + procedure.Id;
            string documentType = "Procedure";
            string documentId = procedure.WSCPNo;
            string documentTitle = procedure.Title;
            string departmentName = procedure.Department?.Name ?? "Unknown";
            DateTime? lastUpdatedDate = procedure.ProcedureStatus
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefault()?.CreatedOn;

            await SendReminderEmail(
                recipientEmail,
                subject,
                statusDescription,
                documentType,
                documentId,
                documentTitle,
                departmentName,
                documentUrl,
                lastUpdatedDate);
        }

        private async Task SendSOPReminderBasedOnStatus(SOP sop, int statusId)
        {
            string recipientEmail = null;
            string statusDescription = null;
            string subject = null;

            switch (statusId)
            {
                case 1: // SUBMITTED: remind company admin
                    var companyAdminEmail = await GetCompanyAdminEmail(sop.CompanyId);
                    if (!string.IsNullOrEmpty(companyAdminEmail))
                    {
                        recipientEmail = companyAdminEmail;
                        statusDescription = "awaiting format check";
                        subject = "Reminder: Request for Format Check: SOP " + sop.WSCPNo;
                    }
                    break;

                case 2: // CONCURRED 1: remind the second concurrer or approver
                    if (!string.IsNullOrEmpty(sop.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(sop.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(sop.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(sop.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: SOP " + sop.WSCPNo;
                    break;

                case 3: // CONCURRED 2: remind the approver
                    if (!string.IsNullOrEmpty(sop.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(sop.ApprovedBy);
                        statusDescription = "awaiting final approval";
                        subject = "Reminder: Request for Approval: SOP " + sop.WSCPNo;
                    }
                    break;

                case 5: // REJECTED: remind department admin
                    var deptAdminEmail = await GetDepartmentAdminEmail(sop.CompanyId, sop.DepartmentId);
                    if (!string.IsNullOrEmpty(deptAdminEmail))
                    {
                        recipientEmail = deptAdminEmail;
                        statusDescription = "rejected and requires revision";
                        subject = "Reminder: Action Required for Rejected SOP " + sop.WSCPNo;
                    }
                    break;

                case 6: // FORMAT CHECKED: remind the first available approver in the chain
                    if (!string.IsNullOrEmpty(sop.Concurred1))
                    {
                        recipientEmail = await GetUserEmail(sop.Concurred1);
                        statusDescription = "awaiting first concurrence";
                    }
                    else if (!string.IsNullOrEmpty(sop.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(sop.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(sop.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(sop.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: SOP " + sop.WSCPNo;
                    break;

                default:
                    _logger.LogWarning($"Unhandled status ID {statusId} for SOP {sop.Id}");
                    return;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning($"Could not determine recipient for SOP {sop.Id} with status {statusId}");
                return;
            }

            string documentUrl = _baseUrl + "documentation/sop/preview?id=" + sop.Id;
            string documentType = "SOP";
            string documentId = sop.WSCPNo;
            string documentTitle = sop.Title;
            string departmentName = sop.Department?.Name ?? "Unknown";
            DateTime? lastUpdatedDate = sop.SOPStatus
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefault()?.CreatedOn;

            await SendReminderEmail(
                recipientEmail,
                subject,
                statusDescription,
                documentType,
                documentId,
                documentTitle,
                departmentName,
                documentUrl,
                lastUpdatedDate);
        }

        private async Task SendWIReminderBasedOnStatus(WI wi, int statusId)
        {
            string recipientEmail = null;
            string statusDescription = null;
            string subject = null;

            switch (statusId)
            {
                case 1: // SUBMITTED: remind company admin
                    var companyAdminEmail = await GetCompanyAdminEmail(wi.CompanyId);
                    if (!string.IsNullOrEmpty(companyAdminEmail))
                    {
                        recipientEmail = companyAdminEmail;
                        statusDescription = "awaiting format check";
                        subject = "Reminder: Request for Format Check: Work Instruction " + wi.WSCPNo;
                    }
                    break;

                case 2: // CONCURRED 1: remind the second concurrer or approver
                    if (!string.IsNullOrEmpty(wi.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(wi.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(wi.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(wi.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Work Instruction " + wi.WSCPNo;
                    break;

                case 3: // CONCURRED 2: remind the approver
                    if (!string.IsNullOrEmpty(wi.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(wi.ApprovedBy);
                        statusDescription = "awaiting final approval";
                        subject = "Reminder: Request for Approval: Work Instruction " + wi.WSCPNo;
                    }
                    break;

                case 5: // REJECTED: remind department admin
                    var deptAdminEmail = await GetDepartmentAdminEmail(wi.CompanyId, wi.DepartmentId);
                    if (!string.IsNullOrEmpty(deptAdminEmail))
                    {
                        recipientEmail = deptAdminEmail;
                        statusDescription = "rejected and requires revision";
                        subject = "Reminder: Action Required for Rejected Work Instruction " + wi.WSCPNo;
                    }
                    break;

                case 6: // FORMAT CHECKED: remind the first available approver in the chain
                    if (!string.IsNullOrEmpty(wi.Concurred1))
                    {
                        recipientEmail = await GetUserEmail(wi.Concurred1);
                        statusDescription = "awaiting first concurrence";
                    }
                    else if (!string.IsNullOrEmpty(wi.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(wi.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(wi.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(wi.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Work Instruction " + wi.WSCPNo;
                    break;

                default:
                    _logger.LogWarning($"Unhandled status ID {statusId} for Work Instruction {wi.Id}");
                    return;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning($"Could not determine recipient for Work Instruction {wi.Id} with status {statusId}");
                return;
            }

            string documentUrl = _baseUrl + "documentation/wi/preview?id=" + wi.Id;
            string documentType = "Work Instruction";
            string documentId = wi.WSCPNo;
            string documentTitle = wi.Title;
            string departmentName = wi.Department?.Name ?? "Unknown";
            DateTime? lastUpdatedDate = wi.WIStatus
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefault()?.CreatedOn;

            await SendReminderEmail(
                recipientEmail,
                subject,
                statusDescription,
                documentType,
                documentId,
                documentTitle,
                departmentName,
                documentUrl,
                lastUpdatedDate);
        }

        private async Task SendQualityManualReminderBasedOnStatus(QualityManual qm, int statusId)
        {
            string recipientEmail = null;
            string statusDescription = null;
            string subject = null;

            switch (statusId)
            {
                case 1: // SUBMITTED: remind company admin
                    var companyAdminEmail = await GetCompanyAdminEmail(qm.CompanyId);
                    if (!string.IsNullOrEmpty(companyAdminEmail))
                    {
                        recipientEmail = companyAdminEmail;
                        statusDescription = "awaiting format check";
                        subject = "Reminder: Request for Format Check: Quality Manual " + qm.DOCNo;
                    }
                    break;

                case 2: // CONCURRED 1: remind the second concurrer or approver
                    if (!string.IsNullOrEmpty(qm.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(qm.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(qm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(qm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Quality Manual " + qm.DOCNo;
                    break;

                case 3: // CONCURRED 2: remind the approver
                    if (!string.IsNullOrEmpty(qm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(qm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                        subject = "Reminder: Request for Approval: Quality Manual " + qm.DOCNo;
                    }
                    break;

                case 5: // REJECTED: remind creator
                    if (!string.IsNullOrEmpty(qm.PreparedBy))
                    {
                        recipientEmail = await GetUserEmail(qm.PreparedBy);
                        statusDescription = "rejected and requires revision";
                        subject = "Reminder: Action Required for Rejected Quality Manual " + qm.DOCNo;
                    }
                    break;

                case 6: // FORMAT CHECKED: remind the first available approver in the chain
                    if (!string.IsNullOrEmpty(qm.Concurred1))
                    {
                        recipientEmail = await GetUserEmail(qm.Concurred1);
                        statusDescription = "awaiting first concurrence";
                    }
                    else if (!string.IsNullOrEmpty(qm.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(qm.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(qm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(qm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Quality Manual " + qm.DOCNo;
                    break;

                default:
                    _logger.LogWarning($"Unhandled status ID {statusId} for Quality Manual {qm.Id}");
                    return;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning($"Could not determine recipient for Quality Manual {qm.Id} with status {statusId}");
                return;
            }

            string documentUrl = _baseUrl + "documentation/qualitymanual/preview?id=" + qm.Id;
            string documentType = "Work Instruction";
            string documentId = qm.DOCNo;
            string documentTitle = qm.Title;
            DateTime? lastUpdatedDate = qm.QualityManualStatus
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefault()?.CreatedOn;

            await SendManualReminderEmail(
                recipientEmail,
                subject,
                statusDescription,
                documentType,
                documentId,
                documentTitle,
                documentUrl,
                lastUpdatedDate);
        }

        private async Task SendDocumentManualReminderBasedOnStatus(DocumentManual dm, int statusId)
        {
            string recipientEmail = null;
            string statusDescription = null;
            string subject = null;

            switch (statusId)
            {
                case 1: // SUBMITTED: remind company admin
                    var companyAdminEmail = await GetCompanyAdminEmail(dm.CompanyId);
                    if (!string.IsNullOrEmpty(companyAdminEmail))
                    {
                        recipientEmail = companyAdminEmail;
                        statusDescription = "awaiting format check";
                        subject = "Reminder: Request for Format Check: Document Manual " + dm.DOCNo;
                    }
                    break;

                case 2: // CONCURRED 1: remind the second concurrer or approver
                    if (!string.IsNullOrEmpty(dm.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(dm.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(dm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(dm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Document Manual " + dm.DOCNo;
                    break;

                case 3: // CONCURRED 2: remind the approver
                    if (!string.IsNullOrEmpty(dm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(dm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                        subject = "Reminder: Request for Approval: Document Manual " + dm.DOCNo;
                    }
                    break;

                case 5: // REJECTED: remind creator
                    if (!string.IsNullOrEmpty(dm.PreparedBy))
                    {
                        recipientEmail = await GetUserEmail(dm.PreparedBy);
                        statusDescription = "rejected and requires revision";
                        subject = "Reminder: Action Required for Rejected Document Manual " + dm.DOCNo;
                    }
                    break;

                case 6: // FORMAT CHECKED: remind the first available approver in the chain
                    if (!string.IsNullOrEmpty(dm.Concurred1))
                    {
                        recipientEmail = await GetUserEmail(dm.Concurred1);
                        statusDescription = "awaiting first concurrence";
                    }
                    else if (!string.IsNullOrEmpty(dm.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(dm.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(dm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(dm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Document Manual " + dm.DOCNo;
                    break;

                default:
                    _logger.LogWarning($"Unhandled status ID {statusId} for Document Manual {dm.Id}");
                    return;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning($"Could not determine recipient for Document Manual {dm.Id} with status {statusId}");
                return;
            }

            string documentUrl = _baseUrl + "documentation/documentmanual/preview?id=" + dm.Id;
            string documentType = "Document Manual";
            string documentId = dm.DOCNo;
            string documentTitle = dm.Title;
            DateTime? lastUpdatedDate = dm.DocumentManualStatus
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefault()?.CreatedOn;

            await SendManualReminderEmail(
                recipientEmail,
                subject,
                statusDescription,
                documentType,
                documentId,
                documentTitle,
                documentUrl,
                lastUpdatedDate);
        }

        private async Task SendLabAccreditationManualReminderBasedOnStatus(LabAccreditationManual lam, int statusId)
        {
            string recipientEmail = null;
            string statusDescription = null;
            string subject = null;

            switch (statusId)
            {
                case 1: // SUBMITTED: remind company admin
                    var companyAdminEmail = await GetCompanyAdminEmail(lam.CompanyId);
                    if (!string.IsNullOrEmpty(companyAdminEmail))
                    {
                        recipientEmail = companyAdminEmail;
                        statusDescription = "awaiting format check";
                        subject = "Reminder: Request for Format Check: Lab Accreditation Manual " + lam.DOCNo;
                    }
                    break;

                case 2: // CONCURRED 1: remind the second concurrer or approver
                    if (!string.IsNullOrEmpty(lam.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(lam.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(lam.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(lam.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Lab Accreditation Manual " + lam.DOCNo;
                    break;

                case 3: // CONCURRED 2: remind the approver
                    if (!string.IsNullOrEmpty(lam.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(lam.ApprovedBy);
                        statusDescription = "awaiting final approval";
                        subject = "Reminder: Request for Approval: Lab Accreditation Manual " + lam.DOCNo;
                    }
                    break;

                case 5: // REJECTED: remind creator
                    if (!string.IsNullOrEmpty(lam.PreparedBy))
                    {
                        recipientEmail = await GetUserEmail(lam.PreparedBy);
                        statusDescription = "rejected and requires revision";
                        subject = "Reminder: Action Required for Rejected Lab Accreditation Manual " + lam.DOCNo;
                    }
                    break;

                case 6: // FORMAT CHECKED: remind the first available approver in the chain
                    if (!string.IsNullOrEmpty(lam.Concurred1))
                    {
                        recipientEmail = await GetUserEmail(lam.Concurred1);
                        statusDescription = "awaiting first concurrence";
                    }
                    else if (!string.IsNullOrEmpty(lam.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(lam.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(lam.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(lam.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Lab Accreditation Manual " + lam.DOCNo;
                    break;

                default:
                    _logger.LogWarning($"Unhandled status ID {statusId} for Lab Accreditation Manual {lam.Id}");
                    return;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning($"Could not determine recipient for Lab Accreditation Manual {lam.Id} with status {statusId}");
                return;
            }

            string documentUrl = _baseUrl + "documentation/labaccreditationmanual/preview?id=" + lam.Id;
            string documentType = "Lab Accreditation Manual";
            string documentId = lam.DOCNo;
            string documentTitle = lam.Title;
            DateTime? lastUpdatedDate = lam.LabAccreditationManualStatus
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefault()?.CreatedOn;

            await SendManualReminderEmail(
                recipientEmail,
                subject,
                statusDescription,
                documentType,
                documentId,
                documentTitle,
                documentUrl,
                lastUpdatedDate);
        }

        private async Task SendSafetyHealthManualReminderBasedOnStatus(SafetyHealthManual shm, int statusId)
        {
            string recipientEmail = null;
            string statusDescription = null;
            string subject = null;

            switch (statusId)
            {
                case 1: // SUBMITTED: remind company admin
                    var companyAdminEmail = await GetCompanyAdminEmail(shm.CompanyId);
                    if (!string.IsNullOrEmpty(companyAdminEmail))
                    {
                        recipientEmail = companyAdminEmail;
                        statusDescription = "awaiting format check";
                        subject = "Reminder: Request for Format Check: Safety & Health Manual " + shm.DOCNo;
                    }
                    break;

                case 2: // CONCURRED 1: remind the second concurrer or approver
                    if (!string.IsNullOrEmpty(shm.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(shm.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(shm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(shm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Safety & Health Manual " + shm.DOCNo;
                    break;

                case 3: // CONCURRED 2: remind the approver
                    if (!string.IsNullOrEmpty(shm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(shm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                        subject = "Reminder: Request for Approval: Safety & Health Manual " + shm.DOCNo;
                    }
                    break;

                case 5: // REJECTED: remind creator
                    if (!string.IsNullOrEmpty(shm.PreparedBy))
                    {
                        recipientEmail = await GetUserEmail(shm.PreparedBy);
                        statusDescription = "rejected and requires revision";
                        subject = "Reminder: Action Required for Rejected Safety & Health Manual " + shm.DOCNo;
                    }
                    break;

                case 6: // FORMAT CHECKED: remind the first available approver in the chain
                    if (!string.IsNullOrEmpty(shm.Concurred1))
                    {
                        recipientEmail = await GetUserEmail(shm.Concurred1);
                        statusDescription = "awaiting first concurrence";
                    }
                    else if (!string.IsNullOrEmpty(shm.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(shm.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(shm.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(shm.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Safety & Health Manual " + shm.DOCNo;
                    break;

                default:
                    _logger.LogWarning($"Unhandled status ID {statusId} for Safety & Health Manual {shm.Id}");
                    return;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning($"Could not determine recipient for Safety & Health Manual {shm.Id} with status {statusId}");
                return;
            }

            string documentUrl = _baseUrl + "documentation/safetyhealthmanual/preview?id=" + shm.Id;
            string documentType = "Safety & Health Manual";
            string documentId = shm.DOCNo;
            string documentTitle = shm.Title;
            DateTime? lastUpdatedDate = shm.SafetyHealthManualStatus
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefault()?.CreatedOn;

            await SendManualReminderEmail(
                recipientEmail,
                subject,
                statusDescription,
                documentType,
                documentId,
                documentTitle,
                documentUrl,
                lastUpdatedDate);
        }

        private async Task SendEnvironmentalManualReminderBasedOnStatus(EnvironmentalManual em, int statusId)
        {
            string recipientEmail = null;
            string statusDescription = null;
            string subject = null;

            switch (statusId)
            {
                case 1: // SUBMITTED: remind company admin
                    var companyAdminEmail = await GetCompanyAdminEmail(em.CompanyId);
                    if (!string.IsNullOrEmpty(companyAdminEmail))
                    {
                        recipientEmail = companyAdminEmail;
                        statusDescription = "awaiting format check";
                        subject = "Reminder: Request for Format Check: Environmental Manual " + em.DOCNo;
                    }
                    break;

                case 2: // CONCURRED 1: remind the second concurrer or approver
                    if (!string.IsNullOrEmpty(em.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(em.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(em.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(em.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Environmental Manual " + em.DOCNo;
                    break;

                case 3: // CONCURRED 2: remind the approver
                    if (!string.IsNullOrEmpty(em.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(em.ApprovedBy);
                        statusDescription = "awaiting final approval";
                        subject = "Reminder: Request for Approval: Environmental Manual " + em.DOCNo;
                    }
                    break;

                case 5: // REJECTED: remind creator
                    if (!string.IsNullOrEmpty(em.PreparedBy))
                    {
                        recipientEmail = await GetUserEmail(em.PreparedBy);
                        statusDescription = "rejected and requires revision";
                        subject = "Reminder: Action Required for Rejected Environmental Manual " + em.DOCNo;
                    }
                    break;

                case 6: // FORMAT CHECKED: remind the first available approver in the chain
                    if (!string.IsNullOrEmpty(em.Concurred1))
                    {
                        recipientEmail = await GetUserEmail(em.Concurred1);
                        statusDescription = "awaiting first concurrence";
                    }
                    else if (!string.IsNullOrEmpty(em.Concurred2))
                    {
                        recipientEmail = await GetUserEmail(em.Concurred2);
                        statusDescription = "awaiting second concurrence";
                    }
                    else if (!string.IsNullOrEmpty(em.ApprovedBy))
                    {
                        recipientEmail = await GetUserEmail(em.ApprovedBy);
                        statusDescription = "awaiting final approval";
                    }
                    subject = "Reminder: Request for Approval: Environmental Manual " + em.DOCNo;
                    break;

                default:
                    _logger.LogWarning($"Unhandled status ID {statusId} for Environmental Manual {em.Id}");
                    return;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                _logger.LogWarning($"Could not determine recipient for Environmental Manual {em.Id} with status {statusId}");
                return;
            }

            string documentUrl = _baseUrl + "documentation/environmentalmanual/preview?id=" + em.Id;
            string documentType = "Environmental Manual";
            string documentId = em.DOCNo;
            string documentTitle = em.Title;
            DateTime? lastUpdatedDate = em.EnvironmentalManualStatus
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefault()?.CreatedOn;

            await SendManualReminderEmail(
                recipientEmail,
                subject,
                statusDescription,
                documentType,
                documentId,
                documentTitle,
                documentUrl,
                lastUpdatedDate);
        }

        #endregion

        #region Helper Methods

        // Shared method for sending reminder emails - reduces code duplication
        private async Task SendReminderEmail(
            string recipientEmail,
            string subject,
            string statusDescription,
            string documentType,
            string documentId,
            string documentTitle,
            string departmentName,
            string documentUrl,
            DateTime? lastUpdatedDate)
        {
            string formattedDate = lastUpdatedDate?.ToString("yyyy-MM-dd") ?? "N/A";

            var mail = new MailRequest
            {
                To = recipientEmail,
                Subject = subject ?? $"Reminder: Action Required for {documentType} {documentId}",
                Body = $@"<p>This is a friendly reminder that {documentType} <a href='{HtmlEncoder.Default.Encode(documentUrl)}'>{documentId}</a> is {statusDescription}.</p>
                        <p>This {documentType.ToLower()} has been in its current status since {formattedDate}.</p>
                        <p><strong>Document Details:</strong><br>
                        Doc No: {documentId}<br>
                        Title: {documentTitle}<br>
                        Function: {departmentName}</p>
                        <p>Thank you for your prompt attention to this matter.</p>
                        <p>You may access the website using the link provided below:<br>
                        {_baseUrl}</p>
                        <p><b>This is an auto-generated email, PLEASE DO NOT REPLY.</b></p>"
            };

            // Add BCC to admin if configured
            if (_sendCopyToAdmin && !string.IsNullOrEmpty(_adminEmail))
            {
                mail.Bcc = _adminEmail;
            }

            try
            {
                await _mailService.SendAsync(mail);
                _logger.LogInformation($"Sent reminder for {documentType} {documentId} to {recipientEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send reminder email for {documentType} {documentId}");
            }
        }

        private async Task SendManualReminderEmail(
            string recipientEmail,
            string subject,
            string statusDescription,
            string documentType,
            string documentId,
            string documentTitle,
            string documentUrl,
            DateTime? lastUpdatedDate)
        {
            string formattedDate = lastUpdatedDate?.ToString("yyyy-MM-dd") ?? "N/A";

            var mail = new MailRequest
            {
                To = recipientEmail,
                Subject = subject ?? $"Reminder: Action Required for {documentType} {documentId}",
                Body = $@"<p>This is a friendly reminder that {documentType} <a href='{HtmlEncoder.Default.Encode(documentUrl)}'>{documentId}</a> is {statusDescription}.</p>
                        <p>This {documentType.ToLower()} has been in its current status since {formattedDate}.</p>
                        <p><strong>Document Details:</strong><br>
                        Doc No: {documentId}<br>
                        Title: {documentTitle}<br>
                        <p>Thank you for your prompt attention to this matter.</p>
                        <p>You may access the website using the link provided below:<br>
                        {_baseUrl}</p>
                        <p><b>This is an auto-generated email, PLEASE DO NOT REPLY.</b></p>"
            };

            // Add BCC to admin if configured
            if (_sendCopyToAdmin && !string.IsNullOrEmpty(_adminEmail))
            {
                mail.Bcc = _adminEmail;
            }

            try
            {
                await _mailService.SendAsync(mail);
                _logger.LogInformation($"Sent reminder for {documentType} {documentId} to {recipientEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send reminder email for {documentType} {documentId}");
            }
        }

        private async Task<string> GetCompanyAdminEmail(int companyId)
        {
            try
            {
                var allUsersByCompany = _userManager.Users
                    .Where(a => a.UserCompanyId == companyId && a.IsActive == true)
                    .ToList();

                var companyAdmins = (from a1 in allUsersByCompany
                                     join a2 in _identityContext.UserRoles on a1.Id equals a2.UserId
                                     join a3 in _roleManager.Roles on a2.RoleId equals a3.Id
                                     where a3.Name == "E"
                                     select a1.Email)
                                   .ToList();

                return string.Join(";", companyAdmins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company admin email");
                return null;
            }
        }

        private async Task<string> GetDepartmentAdminEmail(int companyId, int departmentId)
        {
            try
            {
                var allUsersByDept = _userManager.Users
                    .Where(a => a.UserCompanyId == companyId &&
                           a.UserDepartmentId == departmentId &&
                           a.IsActive == true)
                    .ToList();

                var deptAdmins = (from a1 in allUsersByDept
                                  join a2 in _identityContext.UserRoles on a1.Id equals a2.UserId
                                  join a3 in _roleManager.Roles on a2.RoleId equals a3.Id
                                  where a3.Name == "D"
                                  select a1.Email)
                               .ToList();

                return string.Join(";", deptAdmins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department admin email");
                return null;
            }
        }

        private async Task<string> GetUserEmail(string userId)
        {
            try
            {
                return _userManager.Users
                    .Where(a => a.Id == userId && a.IsActive == true)
                    .Select(a => a.Email)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user email");
                return null;
            }
        }

        private async Task SendCompletionNotification()
        {
            if (!_sendCopyToAdmin || string.IsNullOrEmpty(_adminEmail))
            {
                return;
            }

            MailRequest email = new MailRequest()
            {
                To = _adminEmail,
                Subject = "Reminder: Document Reminder Job Completed",
                Body = $"The scheduled document reminder job has completed successfully at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.<br><br>You may access the website using the link provided below: <br>\r\n{_baseUrl}\r\n <br><br><b>This is an auto generated email, PLEASE DO NOT REPLY.</b>"
            };

            try
            {
                await _mailService.SendAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send completion notification");
            }
        }

        #endregion


    }
}