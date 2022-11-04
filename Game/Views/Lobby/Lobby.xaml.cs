using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChessDotNET.Views
{
    /// <summary>
    /// Interaction logic for Lobby.xaml
    /// </summary>
    public partial class Lobby : Window
    {
        public Lobby()
        {
            InitializeComponent();
        }

        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.DataGridAllPlayers.SelectedItems.Count > 0)
            {
                int rowindex = this.DataGridAllPlayers.SelectedIndex;
                DataGridRow dataRow = (DataGridRow)DataGridAllPlayers.ItemContainerGenerator.ContainerFromIndex(rowindex);
                if (dataRow != null)
                {
                    FocusManager.SetFocusedElement(DataGridAllPlayers, dataRow as IInputElement);
                }
            }
        }
    }
}
