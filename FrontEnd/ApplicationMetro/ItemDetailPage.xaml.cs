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

// O modelo de item de Página de Detalhes do Item está documentado em http://go.microsoft.com/fwlink/?LinkId=234232

namespace ApplicationMetro
{
    /// <resumo>
    /// Uma página que exibe os detalhes de um único item dentro de um grupo permitindo gestos
    /// para percorrer por outros itens pertencentes ao mesmo grupo.
    /// </resumo>
    public sealed partial class ItemDetailPage : ApplicationMetro.Common.LayoutAwarePage
    {
        public ItemDetailPage()
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
            // Permitir que o estado da página salva substitua o item inicial a ser exibido
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            // TODO: criar um modelo de dados apropriado ao seu domínio de problema para substituir os dados de exemplo
            var item = SampleDataSource.GetItem((String)navigationParameter);
            this.DefaultViewModel["Group"] = item.Group;
            this.DefaultViewModel["Items"] = item.Group.Items;
            this.flipView.SelectedItem = item;
        }

        /// <resumo>
        /// Preserva o estado associado a esta página no caso do aplicativo ser suspenso ou a
        /// página é descartada do cache de navegação. Valores devem estar de acordo com os requisitos de
        /// serialização <see cref="SuspensionManager.SessionState"/>.
        /// </resumo>
        /// <param name="pageState">Um dicionário vazio a ser preenchido com o estado serializável.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (SampleDataItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.UniqueId;
        }
    }
}
