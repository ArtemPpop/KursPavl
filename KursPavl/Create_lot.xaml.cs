using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static KursPavl.MainWindow;

namespace KursPavl
{
    /// <summary>
    /// Interaction logic for Create_lot.xaml
    /// </summary>
    public partial class Create_lot : Window
    {
        public Create_lot()
        {
            InitializeComponent();
            SQLitePCL.raw.SetProvider(new SQLite3Provider_sqlite3());
            LoadComboBoxData();
        }

        private void RegisterLotButton_Click(object sender, RoutedEventArgs e)
        {
            string lotName = textBoxLotName.Text;
            double lotPrice = double.Parse(textBoxLotPrice.Text);
            DateTime eventDate = DateTime.Parse(textBoxDataEvent.Text);
            string description = textBoxDescription.Text;

            // Получаем выбранный аукцион и продавца
            if (comboBoxAuction.SelectedItem != null && comboBoxSeller.SelectedItem != null)
            {
                int auctionID = ((Auction)comboBoxAuction.SelectedItem).AuctionID;
                int sellerID = ((Seller)comboBoxSeller.SelectedItem).SellerID;

                AddLot(auctionID, sellerID, lotPrice, description);
                MessageBox.Show("Lot registered successfully!");
            }
            else
            {
                MessageBox.Show("Please select auction and seller!");
            }
        }

        private void BuyLotButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный лот
            if (dataGrid1.SelectedItem != null)
            {
                Lot selectedLot = (Lot)dataGrid1.SelectedItem;

                // Предполагаем, что покупатель уже аутентифицирован и его ID известен
                int buyerID = GetCurrentBuyerID();
                double finalPrice = double.Parse(textBoxFinalPrice.Text); // Предполагаем, что цена введена

                RecordSale(selectedLot.LotID, buyerID, finalPrice);
                MessageBox.Show("Lot purchased successfully!");
            }
            else
            {
                MessageBox.Show("Please select a lot to purchase.");
            }
        }

        // Метод для загрузки данных в ComboBox
        private void LoadComboBoxData()
        {
            using (var connection = new SqliteConnection("DataSource=auction.db"))
            {
                connection.Open();

                // Загрузка аукционов
                string queryAuction = "SELECT AuctionID, Name FROM Auction";
                using (var command = new SqliteCommand(queryAuction, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var auctions = new List<Auction>();
                        while (reader.Read())
                        {
                            auctions.Add(new Auction
                            {
                                AuctionID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                        comboBoxAuction.ItemsSource = auctions;
                    }
                }

                // Загрузка продавцов
                string querySeller = "SELECT SellerID, Name FROM Seller";
                using (var command = new SqliteCommand(querySeller, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var sellers = new List<Seller>();
                        while (reader.Read())
                        {
                            sellers.Add(new Seller
                            {
                                SellerID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                        comboBoxSeller.ItemsSource = sellers;
                    }
                }
            }
        }

        // Метод для добавления лота
        private void AddLot(int auctionID, int sellerID, double startPrice, string description)
        {
            using (var connection = new SqliteConnection("Data Source=auction.db"))
            {
                connection.Open();
                string query = "INSERT INTO Lot (AuctionID, SellerID, StartPrice, Description) VALUES (@auctionID, @sellerID, @startPrice, @description)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@auctionID", auctionID);
                    command.Parameters.AddWithValue("@sellerID", sellerID);
                    command.Parameters.AddWithValue("@startPrice", startPrice);
                    command.Parameters.AddWithValue("@description", description);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void RecordSale(int lotID, int buyerID, double finalPrice)
        {
            // Запись информации о продаже лота в базу данных
            // Ваш код для выполнения этой операции
        }

        // Метод для получения идентификатора текущего покупателя
        private int GetCurrentBuyerID()
        {
            // Ваша логика для получения идентификатора текущего покупателя
            // Например, если у вас есть сеанс аутентификации, можете получить идентификатор текущего пользователя из него
            // Здесь предполагается, что у вас есть переменная currentUserID, которая содержит идентификатор текущего покупателя
            int currentUserID = 0; // Инициализируйте переменную соответствующим образом
            return currentUserID;
        }
    }
}