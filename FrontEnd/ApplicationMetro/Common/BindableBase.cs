using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Data;

namespace ApplicationMetro.Common
{
    /// <resumo>
    /// Implementação de <see cref="INotifyPropertyChanged"/> para simplificar modelos.
    /// </resumo>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <resumo>
        /// Evento de multicast para notificações de alteração de propriedade.
        /// </resumo>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <resumo>
        /// Verifica se uma propriedade já corresponde a um valor desejado. Define a propriedade e
        /// notifica os ouvintes somente quando necessário.
        /// </resumo>
        /// <typeparam name="T">Tipo da propriedade.</typeparam>
        /// <param name="storage">Referência a uma propriedade com ambos getter e setter.</param>
        /// <param name="value">Valor desejado para a propriedade.</param>
        /// <param name="propertyName">Nome da propriedade usada para notificar os ouvintes. Este
        /// valor é opcional e pode ser fornecido automaticamente quando chamado a partir de compiladores que
        /// suportem CallerMemberName.</param>
        /// <returns>True se o valor foi alterado, false se o valor existente corresponde ao
        /// valor desejado.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <resumo>
        /// Notifica aos ouvintes que o valor da propriedade foi alterado.
        /// </resumo>
        /// <param name="propertyName">Nome da propriedade usada para notificar os ouvintes. Este
        /// valor é opcional e pode ser fornecido automaticamente quando chamado a partir de compiladores
        /// que suportem <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
