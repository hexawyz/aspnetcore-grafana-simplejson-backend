using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SimpleJsonDataSource.ViewModels.Converters;

namespace SimpleJsonDataSource
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
				.AddJsonFormatters()
				.AddJsonOptions
				(
					options =>
					{
						options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
						options.SerializerSettings.Converters.Add(new StringEnumConverter(true));

						// Add serialization support for specific data point types.
						options.SerializerSettings.Converters.Add(DataPointConverter<byte>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<sbyte>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<short>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<ushort>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<int>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<uint>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<long>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<ulong>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<float>.Instance);
						options.SerializerSettings.Converters.Add(DataPointConverter<double>.Instance);
					}
				);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
