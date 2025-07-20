using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Seacher
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class licenseFlyout : ContentPage
    {
        public ListView ListView;

        public licenseFlyout()
        {
            InitializeComponent();

            BindingContext = new licenseFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        private class licenseFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<licenseFlyoutMenuItem> MenuItems { get; set; }

            public licenseFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<licenseFlyoutMenuItem>(new[]
                {
                    new licenseFlyoutMenuItem { Id = 0, Title = "Page 1" },
                    new licenseFlyoutMenuItem { Id = 1, Title = "Page 2" },
                    new licenseFlyoutMenuItem { Id = 2, Title = "Page 3" },
                    new licenseFlyoutMenuItem { Id = 3, Title = "Page 4" },
                    new licenseFlyoutMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}