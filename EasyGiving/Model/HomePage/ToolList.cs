using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace EasyGiving.Model.HomePage
{
    public class ToolList
    {
        private SQLiteConnection sqlConn;
        private readonly string OtherMaterialName = "其他";
        public ToolList()
        {
            this.sqlConn = new SQLiteConnection($"Data Source={Settings.Config.ToolListPath}");
            this.sqlConn.Open();
        }

        public List<string> GetAllMaterialsDisplayName()
        {
            var cmd = this.sqlConn.CreateCommand();
            cmd.CommandText = "SELECT MaterialDisplayName FROM Tools ORDER BY ROWID";
            List<string> result = new List<string>();
            using (var reader = cmd.ExecuteReader()) while (reader.Read()) result.Add(reader.GetString(0));
            return result;
        }

        public List<string> GetAllToolsDisplayNameFromMaterialDisplayName(string MaterialsDisplayName) => this.GetToolsDisplayNameFromToolIDs(this.GetAllToolIDsFromMaterialDisplayName(MaterialsDisplayName));

        public List<string> GetAllToolIDsFromMaterialDisplayName(string MaterialsDisplayName)
        {
            var cmd = this.sqlConn.CreateCommand();
            if (MaterialsDisplayName != OtherMaterialName)
                cmd.CommandText = $"""
                    SELECT j.key AS tool_type
                    FROM Tools
                    CROSS JOIN json_each(
                      json_object(
                        'sword', sword,
                        'shovel', shovel,
                        'pickaxe', pickaxe,
                        'axe', axe,
                        'hoe', hoe,
                        'helmet', helmet,
                        'chestplate', chestplate,
                        'leggings', leggings,
                        'boots', boots,
                        'horse_armor', horse_armor
                      )
                    ) AS j
                    WHERE MaterialDisplayName = '{MaterialsDisplayName}' 
                      AND j.value = 1;
                    """;
            else cmd.CommandText = "SELECT ToolItemID FROM OtherTools ORDER BY ROWID";
            List<string> result = new List<string>();
            using (var reader = cmd.ExecuteReader()) while (reader.Read()) result.Add(reader.GetString(0));
            return result;
        }

        public List<string> GetToolsDisplayNameFromToolIDs(List<string> ToolIDs)
        {
            var cmd = this.sqlConn.CreateCommand();
            string cmdText = "SELECT DisplayName FROM MaterialProductDisplayName WHERE";
            foreach (var toolID in ToolIDs)
            {
                if (ToolIDs.IndexOf(toolID) != 0) cmdText += " OR";
                cmdText += $" Item=\"{toolID}\"";
            }
            cmdText += " ORDER BY ROWID";
            cmd.CommandText = cmdText;
            List<string> result = new List<string>();
            using (var reader = cmd.ExecuteReader()) while (reader.Read()) result.Add(reader.GetString(0));
            return result;
        }

        public string GetItemIDFromSelectedItem(string SelectedMaterial, string ToolDisplayName)
        {
            string ItemID = string.Empty;
            if (SelectedMaterial != OtherMaterialName)
            {
                var _cmd = this.sqlConn.CreateCommand();
                _cmd.CommandText = $"SELECT Item FROM MaterialProductDisplayName WHERE DisplayName=\"{SelectedMaterial}\" ORDER BY ROWID";
                List<string> _result = new List<string>();
                using (var reader = _cmd.ExecuteReader()) while (reader.Read()) _result.Add(reader.GetString(0));
                ItemID += _result[0];
                ItemID += "_";
            }
            var cmd = this.sqlConn.CreateCommand();
            cmd.CommandText = $"SELECT Item FROM MaterialProductDisplayName WHERE DisplayName=\"{ToolDisplayName}\" ORDER BY ROWID";
            List<string> result = new List<string>();
            using (var reader = cmd.ExecuteReader()) while (reader.Read()) result.Add(reader.GetString(0));
            ItemID += result[0];
            return ItemID;
        }
    }
}
