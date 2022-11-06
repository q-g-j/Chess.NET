using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.ViewModels
{
    internal class SidebarViewModel : ViewModelBase
    {

        public SidebarViewModel()
        {
            WeakReferenceMessenger.Default.Register<SideMenuVisibilityMessage>(this, (r, m) =>
            {
                SideMenuVisibility = m.Value;
            });
        }

        private string sideMenuVisibility = "Hidden";

        public string SideMenuVisibility
        {
            get => sideMenuVisibility;
            set { sideMenuVisibility = value; OnPropertyChanged(); }
        }

        internal class SideMenuVisibilityMessage : ValueChangedMessage<string>
        {
            public SideMenuVisibilityMessage(string sideMenuVisibility) : base(sideMenuVisibility)
            {
            }
        }

    }
}
