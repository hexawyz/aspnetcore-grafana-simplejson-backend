using Microsoft.AspNetCore.Mvc;
using SimpleJsonDataSource.Repositories;
using SimpleJsonDataSource.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleJsonDataSource.Controllers
{
	[Route("")]
	public class ValuesController : Controller
	{
		private static readonly Dictionary<string, IDataSeriesRepository<double>> DataSeriesGenerators = new Dictionary<string, IDataSeriesRepository<double>>
		{
			{ "Series1", new InMemorySeriesGenerator(555, new TimeSpan(0, 1, 0)) },
			{ "Series2", new InMemorySeriesGenerator(1234, new TimeSpan(0, 0, 5)) },
			{ "Series3", new InMemorySeriesGenerator(65, new TimeSpan(0, 0, 30)) },
			{ "Series4", new InMemorySeriesGenerator(20, new TimeSpan(0, 0, 10)) },
		};

		[HttpGet]
		public IActionResult Get() => Ok();

		[HttpPost("search")]
		public IActionResult Search()
		{
			return Ok(DataSeriesGenerators.Keys);
		}

		[HttpPost("query")]
		public IActionResult Query([FromBody] QueryViewModel query)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			
			return Ok
			(
				query.Targets.Select
					(
						target =>
						{
							DataSeriesGenerators.TryGetValue(target.Target, out var dsg);
							return (Name: target.Target, Repository: dsg);
						}
					).Select
					(
						dsg =>
						{
							var halfWindow = TimeSpan.FromTicks(150 * TimeSpan.TicksPerSecond);

							var from = query.Range.From - halfWindow;
							var to = query.Range.To + halfWindow;

							return new TimeSeriesViewModel<double>
							(
								dsg.Name,
								DataSeriesFilter.FilterDataPoints(dsg.Repository.GetDataPoints(from, to), query.Range.From, query.Range.To, new TimeSpan(query.IntervalMs * TimeSpan.TicksPerMillisecond), new TimeSpan(TimeSpan.TicksPerMinute)).ToArray()
							);
						}
					).ToArray()
			);
		}

		[HttpPost("annotations")]
		public IActionResult GetAnnotations()
		{
			return Ok();
		}
	}
}
