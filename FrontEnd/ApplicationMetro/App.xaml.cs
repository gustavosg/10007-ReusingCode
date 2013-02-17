using ApplicationMetro.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// O modelo de Aplicativo de Grade é documentado em http://go.microsoft.com/fwlink/?LinkId=234226

namespace ApplicationMetro
{
    /// <resumo>
    /// Fornece o comportamento de aplicativos específicos para complementar a classe Application padrão.
    /// </resumo>
    sealed partial class App : Application
    {
        /// <resumo>
        /// Inicializa o objeto de Aplicativo singleton. Esta é a primeira linha do código criado
        /// executado e, como tal, é o equivalente lógico de main() ou WinMain().
        /// </resumo>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <resumo>
        /// Chamado quando o aplicativo é iniciado normalmente pelo usuário final.  Outros pontos de entrada
        /// serão usados quando o aplicativo é iniciado para abrir um arquivo específico, para exibir
        /// resultados da pesquisa e assim por diante.
        /// </resumo>
        /// <param name="args">Detalhes sobre o processo e solicitação de inicialização.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Não repita a inicialização do aplicativo quando a Janela já tiver conteúdo,
            // apenas verifique se a janela está ativa
            
            if (rootFrame == null)
            {
                // Crie um Quadro para atuar como o contexto de navegação e navegue para a primeira página
                rootFrame = new Frame();
                //Associe o quadro a uma chave SuspensionManager                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restaurar o estado da sessão salvo apenas quando apropriado
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Erro ao restaurar o estado.
                        //Suponha que não há nenhum estado e continue
                    }
                }

                // Coloque o quadro na Janela atual
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // Quando a pilha de navegação não for restaurada, navegar para a primeira página,
                // configurando a nova página passando as informações necessárias como um parâmetro
                // de navegação
                if (!rootFrame.Navigate(typeof(GroupedItemsPage), "AllGroups"))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Verifique se a janela atual está ativa
            Window.Current.Activate();
        }

        /// <resumo>
        /// Chamado quando a execução do aplicativo está sendo suspensa.  O estado do aplicativo é salvo
        /// sem saber se o aplicativo será encerrado ou reiniciado com o conteúdo
        /// da memória ainda intacto.
        /// </resumo>
        /// <param name="sender">A fonte da solicitação de suspensão.</param>
        /// <param name="e">Detalhes sobre a solicitação de suspensão.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
