using ApplicationMetro.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página de Itens Agrupados está documentado em http://go.microsoft.com/fwlink/?LinkId=234231

namespace ApplicationMetro
{
    /// <resumo>
    /// Uma página que exibe uma coleção agrupada de itens.
    /// </resumo>
    public sealed partial class GroupedItemsPage : ApplicationMetro.Common.LayoutAwarePage
    {
        public GroupedItemsPage()
        {
            this.InitializeComponent();
        }

        /// <resumo>
        /// Preenche a página com conteúdo transmitido durante a navegação. Qualquer estado salvo também é
        /// fornecido ao recriar uma página a partir de uma sessão anterior.
        /// </resumo>
        /// <param name="navigationParameter">O valor do parâmetro passado para
        /// <see cref="Frame.Navigate(Type, Object)"/> quando esta página foi solicitada inicialmente.
        /// </param>
        /// <param name="pageState">Um dicionário de estado preservado por esta página durante uma sessão
        /// anterior. Será nulo na primeira vez que uma página for visitada.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: criar um modelo de dados apropriado ao seu domínio de problema para substituir os dados de exemplo
            var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        /// <resumo>
        /// Chamado quando um cabeçalho de grupo é clicado.
        /// </resumo>
        /// <param name="sender">O Button usado como um cabeçalho de grupo para o grupo selecionado.</param>
        /// <param name="e">Dados de evento que descreve como o clique foi iniciado.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determina qual grupo representa a instância de Button
            var group = (sender as FrameworkElement).DataContext;

            // Navega até a página de destino apropriado, configurando a nova página
            // passando as informações necessárias como um parâmetro de navegação
            this.Frame.Navigate(typeof(GroupDetailPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <resumo>
        /// Chamado quando um item dentro de um grupo é clicado.
        /// </resumo>
        /// <param name="sender">GridView (ou ListView quando o aplicativo é encaixado)
        /// exibindo o item clicado.</param>
        /// <param name="e">Dados de evento que descreve o item clicado.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navega até a página de destino apropriado, configurando a nova página
            // passando as informações necessárias como um parâmetro de navegação
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }
    }
}
