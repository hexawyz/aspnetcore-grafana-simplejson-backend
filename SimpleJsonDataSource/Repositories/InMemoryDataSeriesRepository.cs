using SimpleJsonDataSource.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SimpleJsonDataSource.Repositories
{
	public class InMemoryDataSeriesRepository<T> : IDataSeriesRepository<T>
	{
		private ImmutableSortedDictionary<DateTime, T> _dataPoints = ImmutableSortedDictionary.Create<DateTime, T>();

		public void Add(DateTime date, T value)
			=> _dataPoints = _dataPoints.Add(date, value);

		public IEnumerable<DataPoint<T>> GetDataPoints(DateTime startDateTime, DateTime endDateTime)
			=> _dataPoints.Select(kvp => new DataPoint<T>(kvp.Key, kvp.Value))
				.SkipWhile(p => p.DateTime < startDateTime)
				.TakeWhile(p => p.DateTime <= endDateTime);
	}
}
