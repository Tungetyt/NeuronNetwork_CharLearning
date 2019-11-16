using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Linq;

namespace NAI_uczenie
{
    public class MyChart
    {
        private readonly CartesianChart chart;

        public MyChart(CartesianChart chart)
        {
            this.chart = chart;
            chart.AxisY.Clear();
            chart.AxisY.Add(new Axis
            {
                MinValue = 0
            });
        }

        public MyChart InputData(double[] errors)
        {
            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = GetPoints(errors)
                }
            };

            return this;
        }

        private ChartValues<ObservablePoint> GetPoints(double[] errors)
        {
            var points = new ChartValues<ObservablePoint>();
            for (int i = 1; i < errors.Count(); i++)
            {
                points.Add(new ObservablePoint
                {
                    X = i,
                    Y = errors[i]
                });
            }

            return points;
        }
    }
}