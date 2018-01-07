using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace SimpleJsonDataSource
{
	public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSerilog((ctx, builder) => builder.WriteTo.Console())
                .Build();
    }
}
