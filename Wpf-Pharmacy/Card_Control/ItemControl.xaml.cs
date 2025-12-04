using API_Pharmacy.Model;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Wpf_Pharmacy.Card_Control
{
    /// <summary>
    /// Логика взаимодействия для ItemControl.xaml
    /// </summary>
    public partial class ItemControl : UserControl
    {
        private Item _item; 
        private Client _client; 

        public ItemControl()
        {
            InitializeComponent();
        }

        public ItemControl(Item item) : this()
        {
            _item = item;
            _client = Service.ClientService.client;
            DataContext = _item;
        }

        private void InstructionButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (DescriptionTextBlock != null && InstructionButton != null)
            {
                if (DescriptionTextBlock.Tag?.ToString() == "Expanded")
                {
                    DescriptionTextBlock.TextWrapping = TextWrapping.NoWrap;
                    DescriptionTextBlock.MaxHeight = 50;
                    InstructionButton.Content = "Инструкция";
                    DescriptionTextBlock.Tag = "Collapsed";
                }
                else
                {
                    DescriptionTextBlock.TextWrapping = TextWrapping.Wrap;
                    DescriptionTextBlock.MaxHeight = double.PositiveInfinity;
                    InstructionButton.Content = "Скрыть";
                    DescriptionTextBlock.Tag = "Expanded";
                }
            }
        }


        private async void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_client?.ClientId == null)
            {
                MessageBox.Show("Пользователь не авторизован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_item?.ItemId == null)
            {
                MessageBox.Show("Неизвестный ID товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await AddItemToBasketAsync(_client.ClientId, _item.ItemId);
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
                        string responseContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Товар добавлен в корзину", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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