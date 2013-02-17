using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ApplicationMetro.Common
{
    /// <resumo>
    ///Implementação típica de página que fornece várias funcionalidades importantes:
    /// <list type="bullet">
    /// <item>
    /// <description>Estado de exibição de aplicativo para mapeamento visual de estados,</description>
    /// </item>
    /// <item>
    /// <description>Manipuladores de eventos GoBack, GoForward, e GoHome .</description>
    /// </item>
    /// <item>
    /// <description>Atalhos de teclado e mouse para navegação</description>
    /// </item>
    /// <item>
    /// <description>Gerenciamento de estados para navegação e gestão do ciclo de vida de projeto</description>
    /// </item>
    /// <item>
    /// &lt;description&gt;Um modelo de exibição padrão&lt;/description&gt;
    /// </item>
    /// </list>
    /// </resumo>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class LayoutAwarePage : Page
    {
        /// <resumo>
        /// Identifica a propriedade de dependência de <see cref="DefaultViewModel"/> .
        /// </resumo>
        public static readonly DependencyProperty DefaultViewModelProperty =
            DependencyProperty.Register("DefaultViewModel", typeof(IObservableMap<String, Object>),
            typeof(LayoutAwarePage), null);

        private List<Control> _layoutAwareControls;

        /// <resumo>
        /// Inicializa uma nova instância da classe <see cref="LayoutAwarePage"/> .
        /// </resumo>
        public LayoutAwarePage()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            // Criar um modelo de exibição padrão vazio
            this.DefaultViewModel = new ObservableDictionary<String, Object>();

            //Se esta página for parte da árvore visual faça duas alterações:
            //1) Mapear estado de exibição do aplicativo para o estado visual da página
            //2) Manipular solicitações de navegação do teclado e mouse
            this.Loaded += (sender, e) =>
            {
                this.StartLayoutUpdates(sender, e);

                //Navegação com teclado e mouse se aplica somente ao ocupar a tela inteira
                if (this.ActualHeight == Window.Current.Bounds.Height &&
                    this.ActualWidth == Window.Current.Bounds.Width)
                {
                    //Ouça a janela diretamente de modo que foco não é necessário
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        this.CoreWindow_PointerPressed;
                }
            };

            //Desfazer as alterações mesmo quando a página não estiver mais visível
            this.Unloaded += (sender, e) =>
            {
                this.StopLayoutUpdates(sender, e);
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    this.CoreWindow_PointerPressed;
            };
        }

        /// <resumo>
        ///Uma implementação de <see cref="IObservableMap&lt;String, Object&gt;"/> projetada para ser
        /// usada como um modelo de exibição trivial.
        /// </resumo>
        protected IObservableMap<String, Object> DefaultViewModel
        {
            get
            {
                return this.GetValue(DefaultViewModelProperty) as IObservableMap<String, Object>;
            }

            set
            {
                this.SetValue(DefaultViewModelProperty, value);
            }
        }

        #region Suporte a navegação

        /// <resumo>
        ///Chamado como manipulador de eventos para recuar na navegação da página
        /// <see cref="Frame"/> até alcançar a parte superior da pilha de navegação.
        /// </resumo>
        /// <param name="sender">Instância que disparou o evento.</param>
        /// <param name="e">Dados de evento que descrevem as condições que levaram ao evento.</param>
        protected virtual void GoHome(object sender, RoutedEventArgs e)
        {
            // Use o quadro de navegação para retornar à página mais superior
            if (this.Frame != null)
            {
                while (this.Frame.CanGoBack) this.Frame.GoBack();
            }
        }

        /// <resumo>
        ///Chamado como manipulador de eventos para recuar na pilha de navegação
        ///associados com os <see cref="Frame"/> desta página.
        /// </resumo>
        /// <param name="sender">Instância que disparou o evento.</param>
        /// &lt;param name="e"&gt;Dados que descrevem as condições levaram ao
        ///evento. </param>
        protected virtual void GoBack(object sender, RoutedEventArgs e)
        {
            // Use o quadro de navegação para retornar à página anterior
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }

        /// <resumo>
        ///Chamado como manipulador de eventos para avançar na pilha de navegação
        ///associados com os <see cref="Frame"/> desta página.
        /// </resumo>
        /// <param name="sender">Instância que disparou o evento.</param>
        /// &lt;param name="e"&gt;Dados que descrevem as condições levaram ao
        ///evento. </param>
        protected virtual void GoForward(object sender, RoutedEventArgs e)
        {
            //Use o quadro de navegação para mover para a próxima página
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }

        /// <resumo>
        ///Chamado a cada tecla pressionada, incluindo chaves do sistema, tias como combinações de teclas Alt, quando
        ///esta página está ativa e ocupa toda a janela.  Usado para detectar navegação do teclado
        ///entre páginas, mesmo quando a página em si não tem foco.
        /// </resumo>
        /// <param name="sender">Instância que disparou o evento.</param>
        ///&lt;param name="args"&gt;Dados que descrevem as condições que levaram ao evento.
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs args)
        {
            var virtualKey = args.VirtualKey;

            //Somente investigar adiante quando a teclas esquerda direitas ou teclas dedicadas Próximo ou Anterior
            //forem pressionadas
            if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                args.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    //Ao serem pressionadas a tecla Anterior ou ALt+esquerda navegar para trás
                    args.Handled = true;
                    this.GoBack(this, new RoutedEventArgs());
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    //Ao serem pressionadas a tecla Seguinte ou ALt+direita navegar adiante
                    args.Handled = true;
                    this.GoForward(this, new RoutedEventArgs());
                }
            }
        }

        /// <resumo>
        ///Chamado a cada clique do mouse, toque na touch screen ou interação equivalente quando esta
        ///página está ativa e ocupa toda a janela.  Usado para detectar avançar em estilo de browser
        ///e cliques anteriores do mouse para navegar entre páginas
        /// </resumo>
        /// <param name="sender">Instância que disparou o evento.</param>
        ///&lt;param name="args"&gt;Dados que descrevem as condições que levaram ao evento.
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs args)
        {
            var properties = args.CurrentPoint.Properties;

            //Ignorar combinações com os botões esquerdo, direito e central
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            //Navegar apropriadamente se atrás ou adiante forem pressionados, mas não ambos
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                args.Handled = true;
                if (backPressed) this.GoBack(this, new RoutedEventArgs());
                if (forwardPressed) this.GoForward(this, new RoutedEventArgs());
            }
        }

        #endregion

        #region Troca de estados visuais

        /// <resumo>
        ///Chamado como um manipulador de eventos, geralmente em <see cref="FrameworkElement.Loaded"/>;
        ///evento de um <see cref="Control"/> na página que indica que o remetente deve
        ///começar a receber mudanças de gerenciamento de estados visuais correspondentes às mudanças
        ///de estado de visualização do aplicativo
        /// </resumo>
        /// <param name="sender">Instância de <see cref="Control"/> que suporte gerenciamento de estados
        ///de visualização correspondente a estados de visualização </param>
        /// <param name="e">Dados de evento que descrevem como a solicitação foi feita.</param>
        /// <remarks>O estado de visualização atual será usado imediatamente para configurar o estado de visualização
        ///correspondente quanto atualizações de layout forem solicitadas. Um manipulador de evento
        /// <see cref="FrameworkElement.Unloaded"/> conectado a
        /// <see cref="StopLayoutUpdates"/> é fortemente recomendado. Instâncias de
        /// <see cref="LayoutAwarePage"/> chamam automaticamente estes manipuladores em seus eventos
        ///Carregados e Descarregados.</remarks>
        /// <seealso cref="DetermineVisualState"/>
        /// <seealso cref="InvalidateVisualState"/>
        public void StartLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            if (this._layoutAwareControls == null)
            {
                // Iniciar a escuta de alterações do estado de exibição quando houverem controles interessados em atualizações
                Window.Current.SizeChanged += this.WindowSizeChanged;
                this._layoutAwareControls = new List<Control>();
            }
            this._layoutAwareControls.Add(control);

            // Definir o estado visual inicial do controle
            VisualStateManager.GoToState(control, DetermineVisualState(ApplicationView.Value), false);
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.InvalidateVisualState();
        }

        /// <resumo>
        ///Chamado como manipulador de eventos, particularmente em um evento <see cref="FrameworkElement.Unloaded"/>
        ///de um <see cref="Control"/>, para indicar que o remetente deve começa a receber
        /// as alterações no gerenciamento de estado visual que correspondam às alterações de estado de exibição do aplicativo.
        /// </resumo>
        /// <param name="sender">Instância de <see cref="Control"/> que suporte gerenciamento de estados
        ///de visualização correspondente a estados de visualização </param>
        /// <param name="e">Dados de evento que descrevem como a solicitação foi feita.</param>
        /// <remarks>O estado de visualização atual será usado imediatamente para configurar o estado de visualização
        ///atualizações de estado de visualização quando atualizações de layout forem solicitadas. </remarks>
        /// <seealso cref="StartLayoutUpdates"/>
        public void StopLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null || this._layoutAwareControls == null) return;
            this._layoutAwareControls.Remove(control);
            if (this._layoutAwareControls.Count == 0)
            {
                // Parar a escuta de alterações do estado de exibição quando não houverem controles interessados em atualizações
                this._layoutAwareControls = null;
                Window.Current.SizeChanged -= this.WindowSizeChanged;
            }
        }

        /// <resumo>
        ///Traduz valores <see cref="ApplicationViewState"/> em strings para gestão de
        ///estados de visualização na página. A implementação padrão utiliza nomes de valores enum.
        ///Subclasses podem substituir esse método para controlar o esquema de mapeamento usado.
        /// </resumo>
        /// <param name="viewState">Estado de exibição para o qual um estado visual é desejado.</param>
        ///&lt;returns&gt;Nome de estado visual para guiar o
        /// <see cref="VisualStateManager"/></returns>
        /// <seealso cref="InvalidateVisualState"/>
        protected virtual string DetermineVisualState(ApplicationViewState viewState)
        {
            return viewState.ToString();
        }

        /// <resumo>
        ///Atualiza todos os controles de detecção de mudanças de estados de visualização no
        ///estado correto.
        /// </resumo>
        /// <comentários>
        /// Geralmente usado em conjunto com a substituição de <see cref="DetermineVisualState"/> para
        ///sinalizar que um valor distinto pode ser retornado mesmo que o estado de visualização não tenha sido
        ///alterado.
        /// </comentários>
        public void InvalidateVisualState()
        {
            if (this._layoutAwareControls != null)
            {
                string visualState = DetermineVisualState(ApplicationView.Value);
                foreach (var layoutAwareControl in this._layoutAwareControls)
                {
                    VisualStateManager.GoToState(layoutAwareControl, visualState, false);
                }
            }
        }

        #endregion

        #region Gerenciamento de vida útil de processos

        private String _pageKey;

        /// <resumo>
        /// Chamado quando esta página é exibida num Frame.
        /// </resumo>
        /// <param name="e">Dados de evento que descrevem como essa página foi atingida.  O parâmetro
        /// propriedade fornece o grupo a ser exibido.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Retornar a uma página em cache por meio de navegação não deve causar carregamento de estado
            if (this._pageKey != null) return;

            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                //Limpar estado existente para avançar na navegação ao adicionar uma nova página à
                //pilha de navegação
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                //Passar o parâmetro de navegação para a nova página
                this.LoadState(e.Parameter, null);
            }
            else
            {
                //Passar o parâmetro de navegação e o estado preservado para a página usando
                //a mesma estratégia para carregar estados suspenso e recriar páginas descartadas
                //do cache
                this.LoadState(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]);
            }
        }

        /// <resumo>
        ///Chamado quando esta página não é exibida em  frame.
        /// </resumo>
        /// <param name="e">Dados de evento que descrevem como essa página foi atingida.  O parâmetro
        /// propriedade fornece o grupo a ser exibido.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<String, Object>();
            this.SaveState(pageState);
            frameState[_pageKey] = pageState;
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
        protected virtual void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <resumo>
        /// Preserva o estado associado a esta página no caso do aplicativo ser suspenso ou a
        /// página é descartada do cache de navegação. Valores devem estar de acordo com os requisitos de
        /// serialização <see cref="SuspensionManager.SessionState"/>.
        /// </resumo>
        /// <param name="pageState">Um dicionário vazio a ser preenchido com o estado serializável.</param>
        protected virtual void SaveState(Dictionary<String, Object> pageState)
        {
        }

        #endregion

        /// <resumo>
        ///Implementação de IObservableMap que suporta reentrada para uso como um modo de exibição padrão
        ///modelo.
        /// </resumo>
        private class ObservableDictionary<K, V> : IObservableMap<K, V>
        {
            private class ObservableDictionaryChangedEventArgs : IMapChangedEventArgs<K>
            {
                public ObservableDictionaryChangedEventArgs(CollectionChange change, K key)
                {
                    this.CollectionChange = change;
                    this.Key = key;
                }

                public CollectionChange CollectionChange { get; private set; }
                public K Key { get; private set; }
            }

            private Dictionary<K, V> _dictionary = new Dictionary<K, V>();
            public event MapChangedEventHandler<K, V> MapChanged;

            private void InvokeMapChanged(CollectionChange change, K key)
            {
                var eventHandler = MapChanged;
                if (eventHandler != null)
                {
                    eventHandler(this, new ObservableDictionaryChangedEventArgs(change, key));
                }
            }

            public void Add(K key, V value)
            {
                this._dictionary.Add(key, value);
                this.InvokeMapChanged(CollectionChange.ItemInserted, key);
            }

            public void Add(KeyValuePair<K, V> item)
            {
                this.Add(item.Key, item.Value);
            }

            public bool Remove(K key)
            {
                if (this._dictionary.Remove(key))
                {
                    this.InvokeMapChanged(CollectionChange.ItemRemoved, key);
                    return true;
                }
                return false;
            }

            public bool Remove(KeyValuePair<K, V> item)
            {
                V currentValue;
                if (this._dictionary.TryGetValue(item.Key, out currentValue) &&
                    Object.Equals(item.Value, currentValue) && this._dictionary.Remove(item.Key))
                {
                    this.InvokeMapChanged(CollectionChange.ItemRemoved, item.Key);
                    return true;
                }
                return false;
            }

            public V this[K key]
            {
                get
                {
                    return this._dictionary[key];
                }
                set
                {
                    this._dictionary[key] = value;
                    this.InvokeMapChanged(CollectionChange.ItemChanged, key);
                }
            }

            public void Clear()
            {
                var priorKeys = this._dictionary.Keys.ToArray();
                this._dictionary.Clear();
                foreach (var key in priorKeys)
                {
                    this.InvokeMapChanged(CollectionChange.ItemRemoved, key);
                }
            }

            public ICollection<K> Keys
            {
                get { return this._dictionary.Keys; }
            }

            public bool ContainsKey(K key)
            {
                return this._dictionary.ContainsKey(key);
            }

            public bool TryGetValue(K key, out V value)
            {
                return this._dictionary.TryGetValue(key, out value);
            }

            public ICollection<V> Values
            {
                get { return this._dictionary.Values; }
            }

            public bool Contains(KeyValuePair<K, V> item)
            {
                return this._dictionary.Contains(item);
            }

            public int Count
            {
                get { return this._dictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
            {
                return this._dictionary.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this._dictionary.GetEnumerator();
            }

            public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
            {
                int arraySize = array.Length;
                foreach (var pair in this._dictionary)
                {
                    if (arrayIndex >= arraySize) break;
                    array[arrayIndex++] = pair;
                }
            }
        }
    }
}
