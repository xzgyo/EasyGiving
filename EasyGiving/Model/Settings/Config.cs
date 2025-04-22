using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGiving.Model.Settings
{
    public static class Config
    {
        public static readonly string ToolForMcVersion = "1.20.1";
        public static readonly string ToolListPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), $"db\\Tools.db");
        public static readonly string EnchantmentsListPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), $"db\\Enchantments.db");
    }
}
