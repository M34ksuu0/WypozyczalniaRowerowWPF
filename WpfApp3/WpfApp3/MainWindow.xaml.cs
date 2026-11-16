
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace WypozyczalniaRowerow
{
    public class Rower
    {
        public int Id { get; set; }
        public string Opis { get; set; } = "";
        public string Rodzaj { get; set; } = "";
        public double Cena { get; set; }
        public bool Wypozyczony { get; set; }
        public string Obrazek { get; set; } = "";

        public string ObrazekSciezka =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         "Obrazki", Obrazek);

        public string Status =>
            Wypozyczony ? "Wypożyczony" : "Dostępny";
    }

    public partial class MainWindow : Window
    {
        ObservableCollection<Rower> rowery =
            new ObservableCollection<Rower>();

        public MainWindow()
        {
            InitializeComponent();
            listaRowerow.ItemsSource = rowery;
        }

        int ZnajdzRower(int id)
        {
            return rowery.IndexOf(
                rowery.FirstOrDefault(r => r.Id == id)
            );
        }

        // DODAWANIE ROWERU
        private void DodajRower_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Nieprawidłowe ID!");
                return;
            }

            if (ZnajdzRower(id) != -1)
            {
                MessageBox.Show("Rower o takim ID już istnieje!");
                return;
            }

            if (!double.TryParse(txtCena.Text, out double cena) || cena <= 0)
            {
                MessageBox.Show("Cena musi być większa od 0!");
                return;
            }

            string nazwa = Path.GetFileName(txtObrazek.Text.Trim());

            string sciezka = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Obrazki",
                nazwa
            );

            if (!File.Exists(sciezka))
            {
                MessageBox.Show("Nie znaleziono obrazka!");
                return;
            }

            Rower r = new Rower
            {
                Id = id,
                Opis = txtOpis.Text,
                Rodzaj = txtRodzaj.Text,
                Cena = cena,
                Obrazek = nazwa,
                Wypozyczony = false
            };

            rowery.Add(r);

            MessageBox.Show("Dodano rower!");

            txtId.Clear();
            txtOpis.Clear();
            txtRodzaj.Clear();
            txtCena.Clear();
            txtObrazek.Text = "rower1.jpg";
        }

        // WYPOŻYCZANIE ROWERU
        private void WypozyczRower_Click(object sender, RoutedEventArgs e)
        {
            if (listaRowerow.SelectedItem is not Rower r)
            {
                MessageBox.Show("Wybierz rower z listy!");
                return;
            }

            if (r.Wypozyczony)
            {
                MessageBox.Show("Rower jest już wypożyczony!");
                return;
            }

            r.Wypozyczony = true;
            listaRowerow.Items.Refresh();

            MessageBox.Show("Rower został wypożyczony!");
        }

        // ZWROT ROWERU
        private void ZwrocRower_Click(object sender, RoutedEventArgs e)
        {
            if (listaRowerow.SelectedItem is not Rower r)
            {
                MessageBox.Show("Wybierz rower z listy!");
                return;
            }

            if (!r.Wypozyczony)
            {
                MessageBox.Show("Ten rower nie jest wypożyczony!");
                return;
            }

            // Tworzenie okna do wpisania godzin
            Window okno = new Window
            {
                Title = "Zwrot roweru",
                Width = 300,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new StackPanel
            {
                Margin = new Thickness(20)
            };

            TextBlock napis = new TextBlock
            {
                Text = "Podaj liczbę godzin:",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 10)
            };

            TextBox poleGodzin = new TextBox
            {
                Height = 30,
                Margin = new Thickness(0, 0, 0, 15)
            };

            Button przycisk = new Button
            {
                Content = "Zatwierdź zwrot",
                Height = 35
            };

            przycisk.Click += (s, args) =>
            {
                if (!double.TryParse(poleGodzin.Text, out double godziny)
                    || godziny <= 0)
                {
                    MessageBox.Show("Podaj prawidłową liczbę godzin!");
                    return;
                }

                double koszt = godziny * r.Cena;

                r.Wypozyczony = false;

                listaRowerow.Items.Refresh();

                MessageBox.Show(
                    $"Rower został zwrócony!\nDo zapłaty: {koszt:F2} zł"
                );

                okno.DialogResult = true;
                okno.Close();
            };

            panel.Children.Add(napis);
            panel.Children.Add(poleGodzin);
            panel.Children.Add(przycisk);

            okno.Content = panel;
            okno.ShowDialog();
        }

        // ODSWIEŻANIE LISTY
        private void Odswiez_Click(object sender, RoutedEventArgs e)
        {
            listaRowerow.Items.Refresh();
        }
    }
}
