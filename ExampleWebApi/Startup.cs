using Autofac;
using Autofac.Integration.WebApi;
using EntityConverter.Core;
using ExampleWebApi.Converters;
using ExampleWebApi.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace ExampleWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration httpConfiguration = new HttpConfiguration();

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ExampleService>().InstancePerLifetimeScope();


            builder.Register(x => new CrmServiceClient(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString))
                .As<IOrganizationService>()
                .SingleInstance();

            // add converters
            EntityModelConverter.DefaultAttributeConverters = new List<CrmAttributeConverter>()
            {
                new EntityReferenceConverter(),
                new OptionSetConverter(),
                new MoneyDecimalConverter()
                // multioptionset ?
            };



            var container = builder.Build();
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            WebApiConfig.Register(httpConfiguration);

            appBuilder.UseWebApi(httpConfiguration);
        }
    }

    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.EnableCors();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}