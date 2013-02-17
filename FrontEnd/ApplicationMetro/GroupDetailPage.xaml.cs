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

// O modelo de item de Página de Detalhes de Grupo está documentado em http://go.microsoft.com/fwlink/?LinkId=234229

namespace ApplicationMetro
{
    /// <resumo>
    /// Uma página que exibe uma visão geral de um único grupo, incluindo uma visualização dos itens
    /// dentro do grupo.
    /// </resumo>
    public sealed partial class GroupDetailPage : ApplicationMetro.Common.LayoutAwarePage
    {
        public GroupDetailPage()
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
            var group = SampleDataSource.GetGroup((String)navigationParameter);
            this.DefaultViewModel["Group"] = group;
            this.DefaultViewModel["Items"] = group.Items;
        }

        /// <resumo>
        /// Chamado quando um item é clicado.
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
