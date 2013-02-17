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

// O modelo do Aplicativo em Branco está documentado em http://go.microsoft.com/fwlink/?LinkId=234227

namespace ApplicationMetro
{
    /// <resumo>
    /// Fornece o comportamento de aplicativos específicos para complementar a classe Application padrão.
    /// </resumo>
    sealed partial class App : Application
    {
        /// <resumo>
        /// Inicializa o objeto de aplicativo singleton.  Esta é a primeira linha de código criado
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
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Não repita a inicialização do aplicativo quando a Janela já tiver conteúdo,
            // apenas verifique se a janela está ativa
            if (rootFrame == null)
            {
                // Crie um Quadro para atuar como o contexto de navegação e navegue para a primeira página
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Carregar estado do aplicativo suspenso anteriormente
                }

                // Coloque o quadro na Janela atual
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Quando a pilha de navegação não for restaurada, navegar para a primeira página,
                // configurando a nova página passando as informações necessárias como um parâmetro
                // de navegação
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
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
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Salvar o estado do aplicativo e parar qualquer atividade de segundo plano
            deferral.Complete();
        }
    }
}
