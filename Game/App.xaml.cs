using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Windows;


namespace ChessDotNET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Globals globals = new Globals();

        internal App()
        {

            WeakReferenceMessenger.Default.Register<GlobalsValueChangedMessage>(this, (r, m) =>
            {
                globals = m.Value;
            });

            // Register the receiver in a module
            WeakReferenceMessenger.Default.Register<App, GlobalsRequestMessage>(this, (r, m) =>
            {
                // Assume that "CurrentUser" is a private member in our viewmodel.
                // As before, we're accessing it through the recipient passed as
                // input to the handler, to avoid capturing "this" in the delegate.
                m.Reply(r.globals);
            });
        }
        internal class GlobalsRequestMessage : RequestMessage<Globals>
        {
        }
        internal class GlobalsValueChangedMessage : ValueChangedMessage<Globals>
        {
            internal GlobalsValueChangedMessage(Globals globals) : base(globals)
            {
            }
        }
    }
}
