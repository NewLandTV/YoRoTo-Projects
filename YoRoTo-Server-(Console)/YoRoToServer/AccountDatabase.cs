using System;
using MySql.Data.MySqlClient;

namespace YoRoToServer
{
    class AccountDatabase
    {
        public static string connectionString = $"Server=;Port=;Database=yoroto;Uid=;Pwd=";

        public static bool CreateAccount(string id, string password)
        {
            try
            {
                using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
                {
                    mySqlConnection.Open();

                    string query = $"INSERT INTO account(id, password) VALUES(\'{id}\', \'{password}\');";

                    MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);

                    if (mySqlCommand.ExecuteNonQuery() == 1)
                    {
                        Console.WriteLine("[Log] Account has been successfully created.");
                    }
                    else
                    {
                        Console.WriteLine("[Log] Failed to create account.");

                        return false;
                    }

                    mySqlConnection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Database error! : {ex}");
            }

            return false;
        }

        public static bool CheckAccount(string id, string password)
        {
            try
            {
                using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
                {
                    mySqlConnection.Open();

                    string query = $"SELECT * FROM account WHERE id = \'{id}\' AND password = \'{password}\';";

                    MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
                    MySqlDataReader checkAccountTable = (MySqlDataReader)mySqlCommand.ExecuteReader();

                    if (!checkAccountTable.HasRows)
                    {
                        Console.WriteLine("[Log] Failed to login.");

                        return false;
                    }

                    mySqlConnection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Database error! : {ex}");
            }

            return false;
        }
    }
}
