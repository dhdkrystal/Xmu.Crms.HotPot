using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Shared
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly CrmsStartupConfig _startupConfig;
        private string _connString = string.Empty;

        private SymmetricSecurityKey _signingKey;

        private TokenValidationParameters _tokenValidationParameters;

        public Startup(IConfiguration configuration, IHostingEnvironment env, CrmsStartupConfig crmsStartupConfig)
        {
            _hostingEnvironment = env;
            _startupConfig = crmsStartupConfig;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // JWT参数
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Keys:ServerSecretKey"]));

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ValidateAudience = false,
                ValidateActor = false,
                ValidateIssuer = false
            };

            services.AddSingleton(
                new JwtHeader(new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)));

            // 登录与鉴权
            services
                .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(
                    options =>
                    {
                        options.Events = new JwtBearerEvents();
                        options.TokenValidationParameters = _tokenValidationParameters;
                        options.Events.OnChallenge += async eventContext =>
                        {
                            if (eventContext.AuthenticateFailure != null)
                            {
                                if (string.IsNullOrEmpty(eventContext.Error) &&
                                    string.IsNullOrEmpty(eventContext.ErrorDescription) &&
                                    string.IsNullOrEmpty(eventContext.ErrorUri))
                                {
                                    eventContext.Response.Headers.Append(HeaderNames.WWWAuthenticate,
                                        eventContext.Options.Challenge);
                                }
                                else
                                {
                                    // https://tools.ietf.org/html/rfc6750#section-3.1
                                    // WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
                                    var builder = new StringBuilder(eventContext.Options.Challenge);
                                    if (eventContext.Options.Challenge.IndexOf(" ", StringComparison.Ordinal) > 0)
                                    {
                                        // Only add a comma after the first param, if any
                                        builder.Append(',');
                                    }
                                    if (!string.IsNullOrEmpty(eventContext.Error))
                                    {
                                        builder.Append(" error=\"");
                                        builder.Append(eventContext.Error);
                                        builder.Append("\"");
                                    }
                                    if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
                                    {
                                        if (!string.IsNullOrEmpty(eventContext.Error))
                                        {
                                            builder.Append(",");
                                        }

                                        builder.Append(" error_description=\"");
                                        builder.Append(eventContext.ErrorDescription);
                                        builder.Append('\"');
                                    }
                                    if (!string.IsNullOrEmpty(eventContext.ErrorUri))
                                    {
                                        if (!string.IsNullOrEmpty(eventContext.Error) ||
                                            !string.IsNullOrEmpty(eventContext.ErrorDescription))
                                        {
                                            builder.Append(",");
                                        }

                                        builder.Append(" error_uri=\"");
                                        builder.Append(eventContext.ErrorUri);
                                        builder.Append('\"');
                                    }

                                    eventContext.Response.Headers.Append(HeaderNames.WWWAuthenticate,
                                        builder.ToString());
                                }
                                eventContext.Response.StatusCode = 401;
                                eventContext.Response.Headers.Append(HeaderNames.ContentType, "application/json");

                                eventContext.HandleResponse();
                                var msg = "登录无效";

                                var ex = eventContext.AuthenticateFailure;
                                var exceptions = new ReadOnlyCollection<Exception>(new[] {ex});
                                if (ex is AggregateException agEx)
                                {
                                    exceptions = agEx.InnerExceptions;
                                }
                                if (exceptions.Select(e => e is SecurityTokenExpiredException).Any())
                                {
                                    msg = "登录已过期，请重新登录";
                                }
                                // 检查更多错误情况
                                var json = $"{{\"msg\": \"{msg}\"}}";
                                var b = Encoding.UTF8.GetBytes(json);
                                await eventContext.Response.Body.WriteAsync(b, 0, b.Length);
                            }
                        };
                    });

            // 数据库
            if (_hostingEnvironment.IsDevelopment() && Convert.ToBoolean(_configuration["Database:UseInMem"]))
            {
                //$env:ASPNETCORE_ENVIRONMENT="Development"
                if (_startupConfig.SqliteConnection != null)
                {
                    services.AddDbContext<CrmsContext>(options => options.UseSqlite(_startupConfig.SqliteConnection));
                }
                else
                {
                    services.AddDbContextPool<CrmsContext>(options => options.UseInMemoryDatabase("CRMS"));
                }
            }
            else
            {
                //$env:ASPNETCORE_ENVIRONMENT="Production"
                _connString = _configuration.GetConnectionString("MYSQL57");
                services.AddDbContextPool<CrmsContext>(options =>
                    options.UseMySql(_connString)
                );
            }
            foreach (var assembly in _startupConfig.ControllerAssemblies)
            {
                var basePath =
                    Path.GetFullPath(_hostingEnvironment.ContentRootPath + "\\..\\" + assembly.GetName().Name);
                if (TryPath(basePath) != null)
                {
                    _startupConfig.ViewPath.Add(basePath);
                }
                if (TryPath(basePath + "\\wwwroot") != null)
                {
                    _startupConfig.WebRootPath.Add(Path.GetFullPath(basePath + "\\wwwroot"));
                }
            }

            _hostingEnvironment.ContentRootFileProvider = new CompositeFileProvider(_startupConfig.ViewPath
                .Select(p => new PhysicalFileProvider(p)).Cast<IFileProvider>()
                .Concat(_startupConfig.ControllerAssemblies.Select(t => new EmbeddedFileProvider(t))));
            _hostingEnvironment.WebRootFileProvider = new CompositeFileProvider(_startupConfig.WebRootPath
                .Select(p => new PhysicalFileProvider(p)).Cast<IFileProvider>().Concat(
                    _startupConfig.ControllerAssemblies.Select(t => new EmbeddedFileProvider(t, "webroot"))));

            // MVC
            services
                .AddMvc()
                .AddJsonOptions(
                    option =>
                    {
                        option.SerializerSettings.Converters.Add(new StringEnumConverter());
                        option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    }
                )
                .AddApplicationParts(_startupConfig.ControllerAssemblies)
                .AddControllersAsServices();

            // 定时任务
            services.AddScheduler();

            IFileProvider TryPath(string p)
            {
                try
                {
                    return new PhysicalFileProvider(p);
                }
                catch
                {
                    return null;
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_startupConfig.ConfigureCrmsApp != null)
            {
                _startupConfig.ConfigureCrmsApp.Invoke(app);
            }
            else
            {
                app.UseStaticFiles();

                app.UseAuthentication();

                app.UseMvc();
            }
        }
    }

    public class CrmsStartupConfig
    {
        public ISet<Assembly> ControllerAssemblies { get; set; } = new HashSet<Assembly> {Assembly.GetEntryAssembly()};

        public ISet<string> ViewPath { get; set; } = new HashSet<string>();

        public ISet<string> WebRootPath { get; set; } = new HashSet<string>();

        public Action<IApplicationBuilder> ConfigureCrmsApp { get; set; }

        public SqliteConnection SqliteConnection { get; set; }
    }
}