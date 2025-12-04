using API_Pharmacy.DTO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_Pharmacy.Card_Control
{
    /// <summary>
    /// Логика взаимодействия для ClientAdminControl.xaml
    /// </summary>
    public partial class ClientAdminControl : UserControl
    {
        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string> { "активен", "заблокирован" };

        private readonly HttpClient _httpClient = new HttpClient();
        private Client _client;

        public ClientAdminControl()
        {
            InitializeComponent();
        }

        public ClientAdminControl(Client client) : this()
        {
            _client = client;
            DataContext = _client;
            StatusComboBox.ItemsSource = StatusOptions;
        }

        private async void StatusComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var status = StatusComboBox.SelectedItem.ToString();

            try
            {
                var update = new UpdateClientStatusRequest
                {
                    ClientId = _client.ClientId,
                    ClientStatus = status
                };

                var response = await _httpClient.PutAsJsonAsync("http://localhost:3000/api/Client/updateStatus", update);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Статус изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка: {response.StatusCode} - {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
