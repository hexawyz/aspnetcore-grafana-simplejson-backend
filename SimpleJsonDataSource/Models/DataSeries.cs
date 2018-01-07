namespace SimpleJsonDataSource.Models
{
	public struct DataSeries<T>
	{
		public DataSeries(string name, DataPoint<T>[] dataPoints) : this()
		{
			Name = name;
			DataPoints = dataPoints;
		}

		public string Name { get; }
		public DataPoint<T>[] DataPoints { get; }
    }
}
