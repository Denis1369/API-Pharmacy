using API_Pharmacy.DTO;
using API_Pharmacy.Model;
using System;
using System.Collections.Generic;
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

namespace Wpf_Pharmacy.Card_Control
{
    /// <summary>
    /// Логика взаимодействия для BasketControl.xaml
    /// </summary>
    public partial class BasketControl : UserControl
    {
        private BasketItemDto _basketItemDto;
        private readonly HttpClient _httpClient = new HttpClient();

        public BasketControl(BasketItemDto basketItemDto)
        {
            InitializeComponent();
            DataContext = basketItemDto;
            _basketItemDto = basketItemDto;
        }

        private async void DelButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveItem removeItem = new RemoveItem() 
            {
                ClientId = Service.ClientService.client.ClientId,
                ItemId = _basketItemDto.ItemId
            };

            var response = await _httpClient.PutAsJsonAsync("http://localhost:3000/api/BasketItem/remove", removeItem);

            if (response.IsSuccessStatusCode)
            {
                var window = Window.GetWindow(this) as BasketWindow;
                await window.LoadDataAsync();
            }
        }

        private async void MinButton_Click(object sender, RoutedEventArgs e)
        {
            await MinItemToBasketAsync(Service.ClientService.client.ClientId, _basketItemDto.ItemId);
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            await AddItemToBasketAsync(Service.ClientService.client.ClientId, _basketItemDto.ItemId);
        }

        private async Task AddItemToBasketAsync(int clientId, int itemId)
        {
            try
            {
                var addItemData = new { ClientId = clientId, ItemId = itemId };

                using (var httpClient = new HttpClient())
                {
                    string apiUrl = "http://localhost:3000/api/BasketItem/add";

                    string jsonPayload = JsonSerializer.Serialize(addItemData);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var window = Window.GetWindow(this) as BasketWindow;
                        await window.LoadDataAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка при добавлении в корзину: {response.StatusCode} - {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при попытке добавить товар в корзину: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task MinItemToBasketAsync(int clientId, int itemId)
        {
            try
            {
                var addItemData = new { ClientId = clientId, ItemId = itemId };

                using (var httpClient = new HttpClient())
                {
                    string apiUrl = "http://localhost:3000/api/BasketItem/min";

                    string jsonPayload = JsonSerializer.Serialize(addItemData);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var window = Window.GetWindow(this) as BasketWindow;
                        await window.LoadDataAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка при добавлении в корзину: {response.StatusCode} - {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при попытке добавить товар в корзину: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
