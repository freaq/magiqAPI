using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qupid.Configuration;
using Tavis.OpenApi;
using Tavis.OpenApi.Export;
using System.IO;


namespace Qupid
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;


        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();            
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // add SwaggerGen to the available services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Qupid Swagger API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
                      
            ConfigurationService configurationService = ConfigurationService.Instance;

            configurationService.LoadConfiguration(hostingEnvironment.ContentRootPath);

            ApiConfiguration apiConfiguration = configurationService.ApiConfiguration;

            if (apiConfiguration.ExtractConfigurationFromDatabase)
            {
                DatabaseAnalyzer databaseAnalyzer = new DatabaseAnalyzer();

                databaseAnalyzer.ExtractConfigurationFromDatabase();

                // reload the configuration to include the newly created route configuration
                configurationService.LoadConfiguration(hostingEnvironment.ContentRootPath);

                apiConfiguration = configurationService.ApiConfiguration;
            }

            app.UseMvc(routes =>
            {
                Tavis.OpenApi.Model.OpenApiDocument openApiDocument = new Tavis.OpenApi.Model.OpenApiDocument();
                openApiDocument.Info = new Tavis.OpenApi.Model.Info();
                openApiDocument.Info.Title = apiConfiguration.Title;
                openApiDocument.Info.Description = apiConfiguration.Description;

                foreach (RouteConfiguration route in apiConfiguration.Routes)
                {
                    // set the route configuration prefix or use the default
                    string routePrefix = route.Prefix;
                    if (String.IsNullOrEmpty(routePrefix))
                    {
                        routePrefix = apiConfiguration.DefaultRoute.Prefix;
                    }

                    string resourceUri = routePrefix + "/";
                    if (!String.IsNullOrEmpty(route.Resource))
                    {
                        resourceUri += route.Resource;
                    }
                    else
                    {
                        resourceUri += route.Table;
                    }

                    // set the route configuration controller or use the default
                    string routeController = route.Controller;
                    if (String.IsNullOrEmpty(routeController))
                    {
                        routeController = apiConfiguration.DefaultRoute.Controller;
                    }

                    // set the route configuration actions or use the default actions
                    List<ActionConfiguration> routeActionConfigurations;
                    if (route.Actions.Any())
                    {
                        routeActionConfigurations = route.Actions;
                    }
                    else
                    {
                        routeActionConfigurations = apiConfiguration.DefaultRoute.Actions;
                    }
                    
                    foreach (ActionConfiguration action in routeActionConfigurations)
                    {
                        string name = Guid.NewGuid().ToString();
                        string template = resourceUri;
                        if (!String.IsNullOrEmpty(action.Template))
                        {
                            template += action.Template;
                        }

                        object defaults = new { controller = routeController, action = action.Name };
                        object constraints = null;
                        object dataTokens = new { apiConfiguration = configurationService.ApiConfiguration, routeConfiguration = route, actionConfiguration = action };

                        routes.MapRoute(
                            name: name,
                            template: template,
                            defaults: defaults,
                            constraints: constraints,
                            dataTokens: dataTokens
                        );
                        

                        // add the route action as an operation to the OpenAPI definition                        
                        Tavis.OpenApi.Model.Operation operation = new Tavis.OpenApi.Model.Operation();
                        operation.Tags = new List<Tavis.OpenApi.Model.Tag>();
                        operation.Tags.Add(new Tavis.OpenApi.Model.Tag() { Name = resourceUri });
                        operation.OperationId = name;
                        operation.Responses = new Dictionary<string, Tavis.OpenApi.Model.Response>();
                        operation.Responses.Add("200", new Tavis.OpenApi.Model.Response()
                        {
                            Description = "Success"
                        });

                        // define one Path for every template
                        // with multiple operations for every HTTP method under those templates
                        Tavis.OpenApi.Model.PathItem pathItem = null;
                        try
                        {
                            pathItem = openApiDocument.Paths.GetPath(template);                            
                        }
                        catch
                        {
                            // the path does not yet exist in the OpenAPIDocument
                            // so we create a new path
                            pathItem = new Tavis.OpenApi.Model.PathItem();
                            openApiDocument.Paths.AddPathItem(template, pathItem);
                        }
                        
                        pathItem.AddOperation(action.HttpMethod.ToLower(), operation);
                        
                    }            
                }

                // write the OpenAPI definition to the file
                string swaggerJsonFilePath = Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot", "swagger.json");

                using (FileStream fileStream = new FileStream(swaggerJsonFilePath, FileMode.Create))
                {
                    Func<Stream, IParseNodeWriter> jsonWriterFactory = s => new JsonParseNodeWriter(s);

                    OpenApiV2Writer writer = new OpenApiV2Writer(jsonWriterFactory);

                    writer.Write(fileStream, openApiDocument);

                    fileStream.Close();
                }
            });
                       

            // enable Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger.json", "freaqs awesome new api");
            });



        }        
    }
}
