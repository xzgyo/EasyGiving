using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGiving.Model.HomePage
{
    public class Enchantments
    {
        private SQLiteConnection sqlConn;
        public Enchantments()
        {
            this.sqlConn = new SQLiteConnection($"Data Source={Settings.Config.EnchantmentsListPath}");
            this.sqlConn.Open();
        }

        public List<string> GetEnchsNames()
        {
            var cmd = this.sqlConn.CreateCommand();
            cmd.CommandText = "SELECT Name FROM Enchantments ORDER BY ROWID";
            List<string> result = new List<string>();
            using (var reader = cmd.ExecuteReader()) while (reader.Read()) result.Add(reader.GetString(0));
            return result;
        }

        public string GetEnchIDFromEnchName(string enchName)
        {
            var cmd = this.sqlConn.CreateCommand();
            cmd.CommandText = $"SELECT ID FROM Enchantments WHERE Name=\"{enchName}\" ORDER BY ROWID";
            List<string> result = new List<string>();
            using (var reader = cmd.ExecuteReader()) while (reader.Read()) result.Add(reader.GetString(0));
            return result[0];
        }

        public Dictionary<string/*ID*/, string/*Name*/> GetAllEnchsAndID()
        {
            var cmd = this.sqlConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Enchantments ORDER BY ROWID";
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (var reader = cmd.ExecuteReader()) while (reader.Read()) result.Add(reader.GetString(0), reader.GetString(1));
            return result;
        }
    }
}
