using API_Pharmacy.DTO;
using API_Pharmacy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
using Wpf_Pharmacy.View;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Wpf_Pharmacy.Card_Control
{
    /// <summary>
    /// Логика взаимодействия для ItemAdminControl.xaml
    /// </summary>
    public partial class ItemAdminControl : UserControl
    {
        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string> { "да", "нет" };
        private API_Pharmacy.Model.Item _item;
        private Client _client;
        private readonly HttpClient _httpClient = new HttpClient();

        public ItemAdminControl()
        {
            InitializeComponent();
        }

        public ItemAdminControl(API_Pharmacy.Model.Item item) : this()
        {
            _item = item;
            _client = Service.ClientService.client;
            DataContext = _item;
            StatusOnComboBox.ItemsSource = StatusOptions;
        }

        private async void StatusOnComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var selectedValue = StatusOnComboBox.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(selectedValue)) return;

            var update = new UpdateStatusOnRequest
            {
                ItemId = _item.ItemId,
                ItemStatusOn = selectedValue
            };

            try
            {
                var response = await _httpClient.PutAsJsonAsync("http://localhost:3000/api/Item/updateStatusOn", update);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Статус изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка : {response.StatusCode} - {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditItemWindow(_item);
            editWindow.ShowDialog();
        }
    }
}
