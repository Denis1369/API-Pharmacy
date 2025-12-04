using API_Pharmacy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using Wpf_Pharmacy.Card_Control;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Wpf_Pharmacy.View
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private List<API_Pharmacy.Model.Item> _allItems = new List<API_Pharmacy.Model.Item>();
        public ObservableCollection<string> StatusOptionsClient { get; } = new ObservableCollection<string> { "все клинеты", "активен", "заблокирован" };
        public ObservableCollection<string> StatusOptionsItem { get; } = new ObservableCollection<string> { "все товары", "да", "нет" };
        private string type = "";


        public AdminWindow()
        {
            InitializeComponent();
            LoadItem("все товары");
            type = "item";
            SortComboBox.SelectedIndex = 0;
        }

        public async void LoadItem(string status) 
        {
            string apiUrl = "http://localhost:3000/api/Item/get_items_admin";
            var items = await _httpClient.GetFromJsonAsync<IEnumerable<API_Pharmacy.Model.Item>>(apiUrl);
            if(status != "все товары")
                items = items.Where(x => x.ItemStatusOn == status);

            SortComboBox.ItemsSource = StatusOptionsItem;
            type = "item";

            ItemsContainer.Children.Clear();

            foreach (var item in items)
            {
                ItemsContainer.Children.Add(new ItemAdminControl(item));
            }
        }

        private async void LoadClients(string status)
        {
            string apiUrl = "http://localhost:3000/api/Client/get_clients";
            var clients = await _httpClient.GetFromJsonAsync<IEnumerable<Client>>(apiUrl);

            if (status != "все клинеты")
                clients = clients.Where(x => x.ClientStatus == status);

            SortComboBox.ItemsSource = StatusOptionsClient;
            type = "client";

            ItemsContainer.Children.Clear();

            foreach (var client in clients)
            {
                ItemsContainer.Children.Add(new ClientAdminControl(client));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadItem("все товары");
            type = "item";
            SortComboBox.SelectedItem = "все товары";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadClients("все клинеты");
            type = "client";
            SortComboBox.SelectedItem = "все клинеты";
        }


        private void SortComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (SortComboBox.SelectedItem == "заблокирован" || SortComboBox.SelectedItem == "активен" || SortComboBox.SelectedItem == "все клинеты")
            {
                LoadClients(SortComboBox.SelectedItem.ToString());
            }
            else if (SortComboBox.SelectedItem == "да" || SortComboBox.SelectedItem == "нет" || SortComboBox.SelectedItem == "все товары")
            {
                LoadItem(SortComboBox.SelectedItem.ToString());
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}
