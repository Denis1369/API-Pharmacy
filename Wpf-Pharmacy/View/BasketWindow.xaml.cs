using API_Pharmacy.DTO;
using API_Pharmacy.Model;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Wpf_Pharmacy.Card_Control;
using Wpf_Pharmacy.Service;

namespace Wpf_Pharmacy.View
{
    /// <summary>
    /// Логика взаимодействия для BasketWindow.xaml
    /// </summary>
    public partial class BasketWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private List<BasketItemDto> list = new List<BasketItemDto>();
        private decimal total;
        private int id;

        public BasketWindow()
        {
            InitializeComponent();

            LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                ItemsContainer.Children.Clear();

                if (ClientService.client == null)
                {
                    MessageBox.Show("Пользователь не авторизован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string apiUrl = "http://localhost:3000/api/BasketItem/basket";

                var user = new User { ClientId = ClientService.client.ClientId };

                var response = await _httpClient.PostAsJsonAsync(apiUrl, user);

                if (response.IsSuccessStatusCode)
                {
                    var basketResult = await response.Content.ReadFromJsonAsync<BasketWithTotalDto>();

                    if (basketResult != null)
                    {
                        foreach (var item in basketResult.Items)
                        {
                            ItemsContainer.Children.Add(new BasketControl(item));
                        }

                        list = basketResult.Items;
                        total = basketResult.TotalSum;
                        id = basketResult.Basket;
                    }
                }
                else
                {
                    MessageBox.Show($"Ошибка сервера: {response.StatusCode}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке корзины: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(ClientService.client).Show();
            Close();
        }

        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            User user = new User { ClientId = Service.ClientService.client.ClientId };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/api/Basket/checkout", user);

            if (response.IsSuccessStatusCode) 
            {
                MessageBox.Show("Товары успешно купленны", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Docs();

                new MainWindow(ClientService.client).Show();
                Close();
            }
        }

        private void Docs()
        {
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add(new SizeF(842, 595), new PdfMargins(40));

            PdfTable table = new PdfTable();
            table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            table.Style.DefaultStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);


            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Название");
            dataTable.Columns.Add("Количество");
            dataTable.Columns.Add("Цена");


            foreach (var item in list)
            {
                dataTable.Rows.Add(new string[]
                {item.ItemTitle, item.BasketItemCount.ToString(), $"{item.ItemPrice} р."});
            }

            PdfTrueTypeFont font = new PdfTrueTypeFont(new Font("Arial", 10), true);
            PdfTrueTypeFont font_total = new PdfTrueTypeFont(new Font("Arial", 14), true);

            page.Canvas.DrawString($"Номер {id}", font_total, new PdfSolidBrush(Color.Black), 380, 0);

            table.DataSource = dataTable;
            table.Style.ShowHeader = true;
            table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.Gray;
            table.Style.HeaderStyle.TextBrush = PdfBrushes.White;

            table.Style.DefaultStyle.Font = font;
            table.Style.HeaderStyle.Font = font;
            PdfLayoutResult result = table.Draw(page, new PointF(40, 50));

            float x = 40;
            float y = result.Bounds.Bottom + 15;

            page.Canvas.DrawString($"Общая сумма: {Math.Round(total, 2)}", font_total, new PdfSolidBrush(Color.Black), x, y);

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, "Chek.pdf");
            doc.SaveToFile(filePath);
        }
    }
}
