using EDocSys.Application.Extensions;
using EDocSys.Infrastructure.Extensions;
using EDocSys.Web.Abstractions;
using EDocSys.Web.Extensions;
using EDocSys.Web.Permission;
using EDocSys.Web.Services;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using EDocSys.Web.Authorization;

namespace EDocSys.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();


            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanViewProcedure", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewProcedureRequirement());
                });

                options.AddPolicy("CanCreateEditProcedure", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditProcedureRequirement());
                });
                options.AddPolicy("CanViewWI", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewWIRequirement());
                });

                options.AddPolicy("CanCreateEditWI", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditWIRequirement());
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanViewSOP", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewSOPRequirement());
                });

                options.AddPolicy("CanCreateEditSOP", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditSOPRequirement());
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanViewDocumentManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewDocumentManualRequirement());
                });

                options.AddPolicy("CanCreateEditDocumentManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditDocumentManualRequirement());
                });

                options.AddPolicy("CanViewQualityManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewQualityManualRequirement());
                });

                options.AddPolicy("CanCreateEditQualityManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditQualityManualRequirement());
                });
                options.AddPolicy("CanViewEnvironmentalManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewEnvironmentalManualRequirement());
                });

                options.AddPolicy("CanCreateEditEnvironmentalManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditEnvironmentalManualRequirement());
                });
                options.AddPolicy("CanViewLabAccreditationManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewLabAccreditationManualRequirement());
                });

                options.AddPolicy("CanCreateEditLabAccreditationManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditLabAccreditationManualRequirement());
                });
                options.AddPolicy("CanViewSafetyHealthManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewSafetyHealthManualRequirement());
                });

                options.AddPolicy("CanCreateEditSafetyHealthManual", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditSafetyHealthManualRequirement());
                });                
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanViewIssuance", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewIssuanceRequirement());
                });
                options.AddPolicy("CanCreateEditIssuance", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditIssuanceRequirement());
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanViewExternalLionSteel", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewExternalLionSteelRequirement());
                });

                options.AddPolicy("CanCreateEditExternalLionSteel", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditExternalLionSteelRequirement());
                });
                options.AddPolicy("CanViewQualityLionSteel", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanViewQualityLionSteelRequirement());
                });

                options.AddPolicy("CanCreateEditQualityLionSteel", policyBuilder =>
                {
                    policyBuilder.AddRequirements(
                        new CanCreateEditQualityLionSteelRequirement());
                });
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthorizationHandler, CanViewProcedureHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditProcedureHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewSOPHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditSOPHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewWIHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditWIHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewDocumentManualHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditDocumentManualHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewQualityManualHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditQualityManualHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewEnvironmentalManualHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditEnvironmentalManualHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewLabAccreditationManualHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditLabAccreditationManualHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewSafetyHealthManualHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditSafetyHealthManualHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewExternalLionSteelHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditExternalLionSteelHandler>();

            services.AddScoped<IAuthorizationHandler, CanViewQualityLionSteelHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditQualityLionSteelHandler>();
            services.AddScoped<IAuthorizationHandler, CanViewIssuanceHandler>();
            services.AddScoped<IAuthorizationHandler, CanCreateEditIssuanceHandler>();


            services.AddNotyf(o =>
            {
                o.DurationInSeconds = 10;
                o.IsDismissable = true;
                o.HasRippleEffect = true;
            });
            services.AddApplicationLayer();
            services.AddInfrastructure(_configuration);
            services.AddPersistenceContexts(_configuration);
            services.AddRepositories();
            services.AddSharedInfrastructure(_configuration);
            services.AddMultiLingualSupport();
            services.AddControllersWithViews().AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddDistributedMemoryCache();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IViewRenderService, ViewRenderService>();

            // Disable Runtime Compilation
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseNotyf();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMultiLingualFeature();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Dashboard}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}