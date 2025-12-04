using API_Pharmacy.Model;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Wpf_Pharmacy.Card_Control;
using Wpf_Pharmacy.View;
using Wpf_Pharmacy.Service;

namespace Wpf_Pharmacy
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private List<Item> _allItems = new List<Item>();

        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private int _totalPages = 1;
        private List<Item> _lastFilteredItems = new List<Item>();

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            LoadDataAsync();

            CartButton.Visibility = Visibility.Collapsed;
            ProfileButton.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
        }

        public MainWindow(Client client_get)
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            LoadDataAsync();
            ClientService.client = client_get;
            LoginButton.Visibility = Visibility.Collapsed;
            CartButton.Visibility = Visibility.Visible;
            ProfileButton.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;

        }

        private async void LoadDataAsync()
        {
            try
            {
                string apiUrl = "http://localhost:3000/api/Item/get_items";
                var items = await _httpClient.GetFromJsonAsync<List<Item>>(apiUrl);

                if (items != null)
                {
                    _allItems = items;
                    LoadBrandsToComboBox();
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadBrandsToComboBox()
        {
            var brands = _allItems
                .Select(x => x.ItemBrand.BrandName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            brands.Insert(0, "Все");
            BrandFilterComboBox.ItemsSource = brands;
            BrandFilterComboBox.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            ItemsContainer.Children.Clear();

            var filteredItems = _allItems.AsEnumerable();
            
            if (BrandFilterComboBox.SelectedIndex > 0)
            {
                var selectedBrand = BrandFilterComboBox.SelectedItem as string;
                filteredItems = filteredItems.Where(x => x.ItemBrand.BrandName == selectedBrand);
            }

            if (double.TryParse(MinPriceTextBox.Text, out double minPrice))
            {
                filteredItems = filteredItems.Where(x => x.ItemPrice >= minPrice);
            }

            if (double.TryParse(MaxPriceTextBox.Text, out double maxPrice))
            {
                filteredItems = filteredItems.Where(x => x.ItemPrice <= maxPrice);
            }

            _lastFilteredItems = filteredItems.ToList();

            _totalPages = Math.Max(1, (_lastFilteredItems.Count + _itemsPerPage - 1) / _itemsPerPage);
            if (_currentPage > _totalPages) _currentPage = _totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageItems = _lastFilteredItems
                .Skip((_currentPage - 1) * _itemsPerPage)
                .Take(_itemsPerPage);

            foreach (var item in pageItems)
            {
                ItemsContainer.Children.Add(new ItemControl(item));
            }

            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            PageInfoTextBlock.Text = $"Страница {_currentPage} из {_totalPages} — всего {_lastFilteredItems.Count}";

            PrevPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentPage < _totalPages;
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            _currentPage = 1;
            ApplyFilters();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            BrandFilterComboBox.SelectedIndex = 0;
            MinPriceTextBox.Text = "";
            MaxPriceTextBox.Text = "";

            _currentPage = 1;
            ApplyFilters();
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFilters();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                ApplyFilters();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) { }
        private void CartButton_Click(object sender, RoutedEventArgs e) 
        {
            new BasketWindow().Show();
            Close();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e) 
        {
            new LoginWindow().Show();
            Close();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditProfileWindow();
            editWindow.ShowDialog();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            ClientService.client = null;
            Close();
        }
    }
}
