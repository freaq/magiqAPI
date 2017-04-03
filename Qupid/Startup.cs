using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qupid.Configuration;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            ConfigurationService configurationService = ConfigurationService.Instance;

            configurationService.LoadConfiguration(hostingEnvironment.ContentRootPath);

            if (configurationService.ApiConfiguration.ExtractConfigurationFromDatabase)
            {
                DatabaseAnalyzer databaseAnalyzer = new DatabaseAnalyzer();

                databaseAnalyzer.ExtractConfigurationFromDatabase();
            }

            app.UseMvc(routes =>
            {
                foreach (RouteConfiguration route in configurationService.Routes)
                {
                    // set the route configuration prefix or use the default
                    string routePrefix = route.Prefix;
                    if (String.IsNullOrEmpty(routePrefix))
                    {
                        routePrefix = configurationService.DefaultRoute.Prefix;
                    }

                    // set the route configuration controller or use the default
                    string routeController = route.Controller;
                    if (String.IsNullOrEmpty(routeController))
                    {
                        routeController = configurationService.DefaultRoute.Controller;
                    }

                    // set the route configuration actions or use the default actions
                    List<ActionConfiguration> routeActionConfigurations;
                    if (route.Actions.Any())
                    {
                        routeActionConfigurations = route.Actions;
                    }
                    else
                    {
                        routeActionConfigurations = configurationService.DefaultRoute.Actions;
                    }

                    foreach (ActionConfiguration action in routeActionConfigurations)
                    {
                        string name = Guid.NewGuid().ToString();
                        string template = routePrefix + "/";
                        if (!String.IsNullOrEmpty(route.Resource))
                        {
                            template += route.Resource;
                        }
                        else
                        {
                            template += route.Table;
                        }

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
                    }
                }
            });

        }
    }
}
