using SimpleJsonDataSource.Models;
using System;
using System.Collections.Generic;

namespace SimpleJsonDataSource.Repositories
{
	public interface IDataSeriesRepository<T>
    {
		IEnumerable<DataPoint<T>> GetDataPoints(DateTime startDateTime, DateTime endDateTime);
    }
}
