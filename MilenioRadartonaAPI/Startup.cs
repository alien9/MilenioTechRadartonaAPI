using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MilenioRadartonaAPI.Areas.Identity.Data;
using MilenioRadartonaAPI.Context;
using MilenioRadartonaAPI.Models;
using MilenioRadartonaAPI.Repository;
using MilenioRadartonaAPI.Service;
using MilenioRadartonaAPI.Token;
using Service;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using MilenioRadartonaAPI.Models.Postgres;
using Microsoft.AspNetCore.Identity.UI.Services;
using Amazon.S3;

namespace MilenioRadartonaAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            Configuration = configuration;
        }

        public Startup(IHostingEnvironment env)
        {
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            //if (env.IsDevelopment())
            //{
            //    builder.AddUserSecrets<Startup>();
            //}

            //Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ===== Contexts do Postgres =====

            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("postgres")));

            services.AddDbContext<ApplicationContextCamadaVizualizacao>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("CamadaVisualizacao")));


            // ================================

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient<IRadartonaService, RadartonaService>();
            services.AddTransient<IRadartonaRepository, RadartonaRepository>();

            services.AddTransient<IOptimusRepository, OptimusRepository>();

            services.AddTransient<IRadartonaService, RadartonaService>();
            services.AddTransient<IOptimusService, OptimusService>();
            services.AddTransient<IOptimusRepository, OptimusRepository>();


            services.AddTransient<Service.IEmailSender, EmailSender>();
            services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            /* token oauth identity configs */

            services.AddDbContext<MilenioRadartonaAPIContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("postgres")));

            // Ativando a utilização do ASP.NET Identity, a fim de
            // permitir a recuperação de seus objetos via injeção de
            // dependências

            services.AddIdentity<MilenioRadartonaAPIUser, IdentityRole>()
                .AddEntityFrameworkStores<MilenioRadartonaAPIContext>()
                .AddDefaultTokenProviders();


            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);


            services.AddSingleton(tokenConfigurations);


            services.AddAuthentication(authOptions =>
            {
            //    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //se comentar funfa identity
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

            //    // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

            //    // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

            //    // Tempo de tolerância para a expiração de um token (utilizado
            //    // caso haja problemas de sincronismo de horário entre diferentes
            //    // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;
            });

            //// Ativa o uso do token como forma de autorizar o acesso
            //// a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            services.AddDistributedMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MilenioRadartonaAPIContext contextIdentity, UserManager<MilenioRadartonaAPIUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Habilitar este código quando o site possuir certificado SSL
            //app.UseForwardedHeaders();

            //app.Use(async (context, next) =>
            //{
            //    if (context.Request.IsHttps || context.Request.Headers["X-Forwarded-Proto"] == Uri.UriSchemeHttps)
            //    {
            //        await next();
            //    }
            //    else
            //    {
            //        string queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
            //        var https = "https://" + context.Request.Host + context.Request.Path + queryString;
            //        context.Response.Redirect(https);
            //    }
            //});


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //The default HSTS value is 30 days.You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            ///* USO DE TOKENS IDENTITY OAUTH CONFIGS */
            //// Criação de estruturas, usuários e permissões
            //// na base do ASP.NET Identity Core (caso ainda não
            //// existam)
            //new IdentityInitializer(contextIdentity, userManager, roleManager)
            //    .Initialize();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "intra",
                    template: "{controller=Intra}/{action=Index}/{erro?}");
            });


        }








    }
}
