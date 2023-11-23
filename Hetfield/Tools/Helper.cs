using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hetfield.Tools
{
    internal static class Helper
    {
        private const string _connectionString = "Data Source=DESKTOP-S7OLEVV\\SQLEXPRESS;Initial Catalog=HetfieldPrime;Integrated Security=True; Encrypt=False";

        private const string _saveUserInSystemPath = @"..\..\..\/Resources/SaveUser.txt";
        public static string ConnectionString { get => _connectionString; }

        public static string SaveUserInSystemPath { get => _saveUserInSystemPath; }

        public static bool DbCompare(string valueFromDb, string value)
        {
            return valueFromDb.ToLower() == value.ToLower().TrimEnd().TrimStart();
        }
    }
}
