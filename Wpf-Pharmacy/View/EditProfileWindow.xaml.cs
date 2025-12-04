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
using Wpf_Pharmacy.Service;

namespace Wpf_Pharmacy.View
{
    /// <summary>
    /// Логика взаимодействия для EditProfileWindow.xaml
    /// </summary>
    public partial class EditProfileWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public EditProfileWindow()
        {
            InitializeComponent();

            var client = ClientService.client;
            if (client != null)
            {
                EmailTextBox.Text = client.ClientEmail ?? "";
                LastNameTextBox.Text = client.ClientLastName ?? "";
                NameTextBox.Text = client.ClientName ?? "";
            }
            else
            {
                MessageBox.Show("Пользователь не авторизован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var client = ClientService.client;

            // Сохраняем старые значения
            var oldEmail = client.ClientEmail;
            var oldLastName = client.ClientLastName;
            var oldName = client.ClientName;

            // Применяем новые значения
            client.ClientEmail = EmailTextBox.Text;
            client.ClientLastName = LastNameTextBox.Text;
            client.ClientName = NameTextBox.Text;

            try
            {
                var response = await _httpClient.PutAsJsonAsync("http://localhost:3000/api/Client/updateProfile", client);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Профиль успешно обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    client.ClientEmail = oldEmail;
                    client.ClientLastName = oldLastName;
                    client.ClientName = oldName;

                    MessageBox.Show($"Ошибка: {response.StatusCode} - {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                client.ClientEmail = oldEmail;
                client.ClientLastName = oldLastName;
                client.ClientName = oldName;

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
