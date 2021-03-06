﻿using System.IO;
using System.Reflection;
using CognitiveServices.Translator.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace CognitiveServices.Translator.Client.WebSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info {
                        Title = "My app API",
                        Description = "A simple example Core Web API",
                        TermsOfService = "None",
                        Version = "v1",
                        Contact = new Contact
                            {
                                Name = "Nordes Lamarre",
                                Email = string.Empty,
                                Url = "https://twitter.com/nordes"
                            },
                            License = new License
                            {
                                Name = "Use under MIT",
                                Url = "https://github.com/Nordes/HoNoSoFt.DotNet.Web.Spa.ProjectTemplates/blob/master/LICENSE"
                            }
                    });

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
                
            // Enable the Gzip compression especially for Kestrel
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                });

            services.AddSpaStaticFiles(config => { config.RootPath = "wwwroot/"; });

            // Add the cognitive service (in case you want to use DI using the appsettings.json)
            services.AddCognitiveServicesTranslator(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression(); // This is especially true for Kestrel!
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.ApplicationBuilder.UseWebpackDevMiddleware(
                        new WebpackDevMiddlewareOptions
                        {
                            HotModuleReplacement = true,
                            ConfigFile = "./build/webpack.config.js"
                        });
                }
            });
        }
    }
}
