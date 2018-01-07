using Newtonsoft.Json;
using SimpleJsonDataSource.Models;

namespace SimpleJsonDataSource.ViewModels
{
	public struct TimeSeriesViewModel<T>
    {
		public TimeSeriesViewModel(string target, DataPoint<T>[] dataPoints)
		{
			Target = target;
			DataPoints = dataPoints;
		}

		public string Target { get; }
		[JsonProperty("datapoints")]
		public DataPoint<T>[] DataPoints { get; }
    }
}
