using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LandonApi.Filters;
using LandonApi.Infrastructure;
using LandonApi.Models;
using LandonApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            services.Configure<HotelInfo>(Configuration.GetSection("Info")); //zabezpeci vytvorenie a naplnneie objektu triedy "HotelInfo" s datami z appsettings.json suboru zo sekcie Info
            services.Configure<HotelOptions>(Configuration);
            services.Configure<PagingOptions>(Configuration.GetSection("DefaultPagingOptions"));


            //EF core triedy su casto Scoped, preto aj ine ktore s nimi interaguju by mali byt scoped
            services.AddScoped<IRoomService, DefaultRoomService>(); //definovanie pre DI, DefaultRoomService bude injektovane pre kazdy incomming request
            services.AddScoped<IOpeningService, DefaultOpeningService>();
            services.AddScoped<IBookingService, DefaultBookingService>();
            services.AddScoped<IDateLogicService, DefaultDateLogicService>();

            //Use in-memory database for quick dev and testing
            //TODO: swap out for real DB in PROD
            services.AddDbContext<HotelApiDbContext>(options => options.UseInMemoryDatabase("landondb")); //pre DEV ucely je pouzita in-memory DB

            //na poradi definovania servisou v tejto metode nazalezi
            services.AddMvc(options => 
                    {
                        options.CacheProfiles.Add("Static", new CacheProfile //cachovanie je mozne definovat aj cez cache profile
                        {
                            Duration = 86400
                        });

                        options.Filters.Add<JsonExceptionFilter>(); //zareferencovanie custom filtra
                        options.Filters.Add<RequireHttpsOrCloseAttribute>();
                        options.Filters.Add<LinkRewritingFilter>();
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(options => 
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        // These should be the defaults, but we can be explicit:
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                        options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;

                    });

            services.AddRouting(options => options.LowercaseUrls = true); //vsetky vygenerovane URL budu lowercase

            // Add OpenAPI/Swagger document
            services.AddOpenApiDocument(); // registers a OpenAPI v3.0 document with the name "v1" (default)
            // services.AddSwaggerDocument(); // registers a Swagger v2.0 document with the name "v1" (default)

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); //definovanie defailt api verzie
                options.ApiVersionReader = new MediaTypeApiVersionReader(); //zadefinovanie co bude zdrojom urcujucim verziu
                options.AssumeDefaultVersionWhenUnspecified = true; //ak verzia nebude definovana ma byt predpokladana
                options.ReportApiVersions = true; //vramci response je zahrnuta info o verzii API
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            services.AddCors(options => 
            {
                options.AddPolicy("AllowMyApp", policy => policy.AllowAnyOrigin()); //pre DEV ucely je mozne pouzit rozny origin (AllowAnyOrigin), pre PROD to musi byt upravene
            });

            services.AddAutoMapper(options => options.AddProfile<MappingProfile>());

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context => //uprava exception response pri nevalidnom stave
                {
                    var errorResponse = new ApiError(context.ModelState);
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddResponseCaching();
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

            app.UseCors("AllowMyApp"); //applikovanie CORS policy, musi byt definovane pred middlewarom ktory vytvara response (pr. UseMVC)

            //aplikuje rovnaky header caching atribut
            app.UseResponseCaching(); //UseResponseCaching musi byt vramci pipeline nad inymi middleware ktore produkuju response (pr. MVC)

            //app.UseHttpsRedirection(); //zabezpecuje automaticky redirect ak sa klient snazi pristupovat na server cez HTTP port (redirect na HTTPS port)
            app.UseMvc();
        }
    }
}
