using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ApplicationPC.VIews
{
    /// <summary>
    /// Interação lógica para GraficoPropagacao.xam
    /// </summary>
    public partial class GraficoPropagacao : Page
    {
        public GraficoPropagacao()
        {
            InitializeComponent();

            InicializaGrafico();
        }

        private void InicializaGrafico()
        {
            List<KeyValuePair<String, int>> chartGraphic = new List<KeyValuePair<string, int>>();

            chartGraphic.Add(new KeyValuePair<String, int>("Sound 1", 50));
            chartGraphic.Add(new KeyValuePair<String, int>("Sound 2", 60));
            chartGraphic.Add(new KeyValuePair<String, int>("Sound 3", 70));
            chartGraphic.Add(new KeyValuePair<String, int>("Sound 4", 80));
            chartGraphic.Add(new KeyValuePair<String, int>("Sound 5", 90));

            columnSeries.DataContext = chartGraphic;


        }

    }
}
