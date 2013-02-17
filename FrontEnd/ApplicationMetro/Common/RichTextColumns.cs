using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;

namespace ApplicationMetro.Common
{
    /// <resumo>
    ///Pano de embrulho para &lt;see cref="RichTextBlock"&gt;&lt;/see&gt; que criar uma quantidade adicional de overflows
    ///colunas conforme necessário para o ajuste do conteúdo disponível.
    /// </resumo>
    ///<exemplo>
    ///Cria uma coleção de colunas de 400 pixels de largura espaçadas entre si de 50 pixels
    ///para conter conteúdo com ligações de dados arbitrárias:
    ///<código>
    /// <RichTextColumns>
    ///     <RichTextColumns.ColumnTemplate>
    ///         <DataTemplate>
    ///             <RichTextBlockOverflow Width="400" Margin="50,0,0,0"/>
    ///         </DataTemplate>
    ///     </RichTextColumns.ColumnTemplate>
    ///     
    ///     <RichTextBlock Width="400">
    ///         <Paragraph>
    ///             <Run Text="{Binding Content}"/>
    ///         </Paragraph>
    ///     </RichTextBlock>
    /// </RichTextColumns>
    /// </code>
    /// </example>
    /// <remarks> Normalmente usado em regiões de rolagem horizontais em que a quantidade ilimitada de
    ///espaço permite que todas as colunas necessárias seja criada. Quando utilizada em  um espaço de rolagem vertical
    ///não haverá colunas adicionais. </remarks>
    [Windows.UI.Xaml.Markup.ContentProperty(Name = "RichTextContent")]
    public sealed class RichTextColumns : Panel
    {
        /// <resumo>
        /// Identifica a propriedade de dependência <see cref="RichTextContent"/> .
        /// </resumo>
        public static readonly DependencyProperty RichTextContentProperty =
            DependencyProperty.Register("RichTextContent", typeof(RichTextBlock),
            typeof(RichTextColumns), new PropertyMetadata(null, ResetOverflowLayout));

        /// <resumo>
        /// Identifica a propriedade de dependência <see cref="ColumnTemplate"/> .
        /// </resumo>
        public static readonly DependencyProperty ColumnTemplateProperty =
            DependencyProperty.Register("ColumnTemplate", typeof(DataTemplate),
            typeof(RichTextColumns), new PropertyMetadata(null, ResetOverflowLayout));

        /// <resumo>
        /// Inicializa uma nova instância da classe <see cref="RichTextColumns"/> .
        /// </resumo>
        public RichTextColumns()
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
        }

        /// <resumo>
        ///Obtém ou define o conteúdo rich text inicial a ser usado como a primeira coluna.
        /// </resumo>
        public RichTextBlock RichTextContent
        {
            get { return (RichTextBlock)GetValue(RichTextContentProperty); }
            set { SetValue(RichTextContentProperty, value); }
        }

        /// <resumo>
        ///Obtém ou define o modelo usado para criar adicionais
        /// instâncias de <see cref="RichTextBlockOverflow"/>.
        /// </resumo>
        public DataTemplate ColumnTemplate
        {
            get { return (DataTemplate)GetValue(ColumnTemplateProperty); }
            set { SetValue(ColumnTemplateProperty, value); }
        }

        /// <resumo>
        /// Chamado quando o modelo de conteúdo ou overflow é alterado para recriar o layout da coluna.
        /// </resumo>
        /// <param name="d">Instância de <see cref="RichTextColumns"/> onde a alteração
        /// ocorreu.</param>
        /// <param name="e">Dados do evento descrevendo alteração específica.</param>
        private static void ResetOverflowLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Se houver mudanças dramáticas reconstrua do início o modelo de colunas
            var target = d as RichTextColumns;
            if (target != null)
            {
                target._overflowColumns = null;
                target.Children.Clear();
                target.InvalidateMeasure();
            }
        }

        /// <resumo>
        ///Lista colunas de overflow já criadas. Deve manter uma relação 1:1 com
        ///instâncias <see cref="Panel.Children"/>  na coleção seguindo o
        ///RichTextBlock filho.
        /// </resumo>
        private List<RichTextBlockOverflow> _overflowColumns = null;

        /// <resumo>
        /// Determina se colunas de overflow adicionais são necessárias e se as colunas existentes podem
        /// ser removidas.
        /// </resumo>
        /// <param name="availableSize">O tamanho do espaço disponível, usado para restringir o
        /// número de colunas adicionais que podem ser criadas.</param>
        /// <returns>O tamanho resultante do conteúdo original mais quaisquer colunas adicionais.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.RichTextContent == null) return new Size(0, 0);

            // Verifique se o RichTextBlock é um filho, usando a falta de
            // uma lista de colunas adicionais, como um sinal de que isso não foi
            // feito ainda
            if (this._overflowColumns == null)
            {
                Children.Add(this.RichTextContent);
                this._overflowColumns = new List<RichTextBlockOverflow>();
            }

            // Comece medindo o conteúdo original do RichTextBlock
            this.RichTextContent.Measure(availableSize);
            var maxWidth = this.RichTextContent.DesiredSize.Width;
            var maxHeight = this.RichTextContent.DesiredSize.Height;
            var hasOverflow = this.RichTextContent.HasOverflowContent;

            // Verifique se há colunas de overflow suficientes
            int overflowIndex = 0;
            while (hasOverflow && maxWidth < availableSize.Width && this.ColumnTemplate != null)
            {
                // Usar colunas de overflow existentes até que elas acabem, em seguida, criar
                // mais do modelo fornecido
                RichTextBlockOverflow overflow;
                if (this._overflowColumns.Count > overflowIndex)
                {
                    overflow = this._overflowColumns[overflowIndex];
                }
                else
                {
                    overflow = (RichTextBlockOverflow)this.ColumnTemplate.LoadContent();
                    this._overflowColumns.Add(overflow);
                    this.Children.Add(overflow);
                    if (overflowIndex == 0)
                    {
                        this.RichTextContent.OverflowContentTarget = overflow;
                    }
                    else
                    {
                        this._overflowColumns[overflowIndex - 1].OverflowContentTarget = overflow;
                    }
                }

                // Mede a nova coluna e se prepara para repetir conforme necessário
                overflow.Measure(new Size(availableSize.Width - maxWidth, availableSize.Height));
                maxWidth += overflow.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, overflow.DesiredSize.Height);
                hasOverflow = overflow.HasOverflowContent;
                overflowIndex++;
            }

            // Desconecta colunas extras da cadeia de overflow, remove-as da nossa lista particular
            // de colunas e remove-as como filhos
            if (this._overflowColumns.Count > overflowIndex)
            {
                if (overflowIndex == 0)
                {
                    this.RichTextContent.OverflowContentTarget = null;
                }
                else
                {
                    this._overflowColumns[overflowIndex - 1].OverflowContentTarget = null;
                }
                while (this._overflowColumns.Count > overflowIndex)
                {
                    this._overflowColumns.RemoveAt(overflowIndex);
                    this.Children.RemoveAt(overflowIndex + 1);
                }
            }

            // Relata o tamanho final determinado
            return new Size(maxWidth, maxHeight);
        }

        /// <resumo>
        /// Organiza o conteúdo original e todas as colunas extras.
        /// </resumo>
        /// <param name="finalSize">Define o tamanho da área em que os filhos devem ser organizados
        /// dentro.</param>
        /// <returns>O tamanho da área que os filhos realmente necessitam.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double maxWidth = 0;
            double maxHeight = 0;
            foreach (var child in Children)
            {
                child.Arrange(new Rect(maxWidth, 0, child.DesiredSize.Width, finalSize.Height));
                maxWidth += child.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }
            return new Size(maxWidth, maxHeight);
        }
    }
}
