using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Threads.Data
{
    public class AccDataAccess : IDataAccess
    {
        private string ConnectionString;

        public AccDataAccess(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool InsertData(int num, string message)
        {
            string query = "INSERT INTO History (Num, Message, Created) VALUES (@ThreadNum, @Message, @Date)";

            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ThreadNum", num);
                    command.Parameters.AddWithValue("@Message", message);
                    command.Parameters.AddWithValue("@Date", DateTime.Now);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Įvyko klaida įdedant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
        }

        public ObservableCollection<ThreadInfo> GetLatest20Data()
        {
            ObservableCollection<ThreadInfo> threadInfoCollection = new ObservableCollection<ThreadInfo>();
            string query = "SELECT TOP 20 Num, Message FROM History ORDER BY Id DESC";

            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ThreadInfo threadInfo = new ThreadInfo(reader.GetInt32(0), reader.GetString(1));
                                threadInfoCollection.Add(threadInfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Įvyko klaida: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            return threadInfoCollection;
        }
    }
}

