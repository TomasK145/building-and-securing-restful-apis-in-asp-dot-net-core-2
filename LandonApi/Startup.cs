﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;

namespace LandonApi
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
            //na poradi definovania servisou v tejto metode nazalezi
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(options => 
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    });

            services.AddRouting(options => options.LowercaseUrls = true); //vsetky vygenerovane URL budu lowercase

            // Add OpenAPI/Swagger document
            services.AddOpenApiDocument(); // registers a OpenAPI v3.0 document with the name "v1" (default)
            // services.AddSwaggerDocument(); // registers a Swagger v2.0 document with the name "v1" (default)
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //na poradi definovania zalezi, definuje poradie vykonavania middleware pipeliny
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Add OpenAPI/Swagger middlewares
                app.UseOpenApi(); // Serves the registered OpenAPI/Swagger documents by default on `/swagger/{documentName}/swagger.json`
                app.UseSwaggerUi3(); // Serves the Swagger UI 3 web ui to view the OpenAPI/Swagger documents by default on `/swagger`
                //swagger pre API je nasledne dostupny na url ~/swagger
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
