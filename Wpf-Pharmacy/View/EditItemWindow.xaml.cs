using API_Pharmacy.Model;
using System;
using System.Collections.Generic;
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

namespace Wpf_Pharmacy.View
{
    /// <summary>
    /// Логика взаимодействия для EditItemWindow.xaml
    /// </summary>
    public partial class EditItemWindow : Window
    {
        public EditItemWindow()
        {
            InitializeComponent();
        }

        private readonly HttpClient _httpClient = new HttpClient();
        private Item _item;

        public EditItemWindow(Item item)
        {
            InitializeComponent();
            _item = item;

            LoadBrandsAsync();
            PopulateFields();
        }

        private async void LoadBrandsAsync()
        {
            try
            {
                var brands = await _httpClient.GetFromJsonAsync<List<Brand>>("http://localhost:3000/api/Brand/get_brands");
                BrandComboBox.ItemsSource = brands;
                BrandComboBox.SelectedValuePath = "BrandId";
                BrandComboBox.DisplayMemberPath = "BrandName";
                BrandComboBox.SelectedValue = _item.ItemBrandId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки брендов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateFields()
        {
            TitleTextBox.Text = _item.ItemTitle ?? "";
            DescTextBox.Text = _item.ItemDesc ?? "";
            ImgTextBox.Text = _item.ItemImg ?? "";
            CountTextBox.Text = _item.ItemCount?.ToString() ?? "";
            PriceTextBox.Text = _item.ItemPrice?.ToString() ?? "";

            BrandComboBox.SelectedValue = _item.ItemBrandId;

            if (_item.ItemStatus == "1")
                StatusComboBox.SelectedIndex = 0;
            else
                StatusComboBox.SelectedIndex = 1;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ImgTextBox.Text)) 
                {
                    MessageBox.Show($"Ошибка: Путь для изображения не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _item.ItemTitle = TitleTextBox.Text;
                _item.ItemDesc = DescTextBox.Text;
                _item.ItemImg = ImgTextBox.Text;

                if (int.TryParse(CountTextBox.Text, out int count))
                    _item.ItemCount = count;
                else
                    _item.ItemCount = null;

                if (int.TryParse(PriceTextBox.Text, out int price))
                    _item.ItemPrice = price;
                else
                    _item.ItemPrice = null;

                _item.ItemBrandId = (int?)BrandComboBox.SelectedValue;

                var selectedStatus = StatusComboBox.SelectedItem as ComboBoxItem;
                if (selectedStatus?.Content.ToString() == "без рецепта")
                    _item.ItemStatus = "1";
                else
                    _item.ItemStatus = "0";

                var response = await _httpClient.PutAsJsonAsync("http://localhost:3000/api/Item/update", _item);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Товар успешно обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка: {response.StatusCode} - {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
