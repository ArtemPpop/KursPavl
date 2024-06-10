using Microsoft.Data.Sqlite;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System;

namespace KursPavl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            comboBoxAuction = new ComboBox();
            comboBoxSeller = new ComboBox();
            LoadComboBoxData(); // Добавляем вызов метода для загрузки данных в комбобоксы
        }
        public ComboBox comboBoxAuction;
        public ComboBox comboBoxSeller;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //mainFrame.Navigate(new Authorization());
            Authorization authorization = new Authorization();
            authorization.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Create_lot create_Lot = new Create_lot();
            create_Lot.Show();
        }

        private void LoadComboBoxData()
        {
            using (var connection = new SqliteConnection("Data Source=auction.db"))
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
        public List<Auction> GetAuctionsByDateRange(DateTime startDate, DateTime endDate)
        {
            var auctions = new List<Auction>();
            using (var connection = new SqliteConnection("DataSource=auction.db"))
            {
                connection.Open();
                string query = "SELECT * FROM Auction WHERE Date BETWEEN @startDate AND @endDate";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            auctions.Add(new Auction
                            {
                                AuctionID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Date = DateTime.Parse(reader.GetString(2)),
                                Location = reader.GetString(3),
                                Description = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return auctions;
        }

        public List<AuctionIncome> GetAuctionsWithIncome()
        {
            var auctionIncomes = new List<AuctionIncome>();
            using (var connection = new SqliteConnection("DataSource=auction.db"))
            {
                connection.Open();
                string query = "SELECT Auction.Name, Auction.Date, Auction.Location, SUM(Lot.FinalPrice) as TotalIncome FROM Auction " +
                               "JOIN Lot ON Auction.AuctionID = Lot.AuctionID " +
                               "WHERE Lot.FinalPrice IS NOT NULL " +
                               "GROUP BY Auction.AuctionID " +
                               "ORDER BY TotalIncome DESC";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            auctionIncomes.Add(new AuctionIncome
                            {
                                Name = reader.GetString(0),
                                Date = DateTime.Parse(reader.GetString(1)),
                                Location = reader.GetString(2),
                                TotalIncome = reader.GetDouble(3)
                            });
                        }
                    }
                }
            }
            return auctionIncomes;
        }

        public List<Lot> GetSoldLotsByDateRange(DateTime startDate, DateTime endDate)
        {
            var soldLots = new List<Lot>();
            using (var connection = new SqliteConnection("DataSource=auction.db"))
            {
                connection.Open();
                string query = "SELECT * FROM Lot WHERE FinalPrice IS NOT NULL AND AuctionID IN (SELECT AuctionID FROM Auction WHERE Date BETWEEN @startDate AND @endDate)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            soldLots.Add(new Lot
                            {
                                LotID = reader.GetInt32(0),
                                AuctionID = reader.GetInt32(1),
                                SellerID = reader.GetInt32(2),
                                BuyerID = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                                StartPrice = reader.GetDouble(4),
                                FinalPrice = reader.GetDouble(5),
                                Description = reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return soldLots;
        }
        public class AuctionIncome
        {
            public string? Name { get; set; }
            public DateTime Date { get; set; }
            public string? Location { get; set; }
            public double TotalIncome { get; set; }
        }




        public class Account
        {
            public int AccountID { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? Role { get; set; }
        }

        public class Auction
        {
            public int AuctionID { get; set; }
            public string? Name { get; set; }
            public DateTime Date { get; set; }
            public string? Location { get; set; }
            public string? Description { get; set; }
        }

        public class Lot
        {
            public int LotID { get; set; }
            public int AuctionID { get; set; }
            public int SellerID { get; set; }
            public int? BuyerID { get; set; }
            public double StartPrice { get; set; }
            public double? FinalPrice { get; set; }
            public string? Description { get; set; }
        }

        public class Seller
        {
            public int SellerID { get; set; }
            public string? Name { get; set; }
            public string? ContactInfo { get; set; }
            public int AccountID { get; set; }
        }

        public class Buyer
        {
            public int BuyerID { get; set; }
            public string? Name { get; set; }
            public string? ContactInfo { get; set; }
            public int AccountID { get; set; }
        }
    }
}





         

    







       



