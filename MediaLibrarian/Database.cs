﻿using System;
using System.Data;
using System.Data.SQLite;

namespace MediaLibrarian
{
    #region TOUPPER
    [SQLiteFunction(Name = "TOUPPER", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class TOUPPER : SQLiteFunction
    {
        public override object Invoke(object[] args)        //characters for the growth of
        {
            return args[0].ToString().ToUpper();
        }
    }

    [SQLiteFunction(Name = "COLLATION_CASE_INSENSITIVE", FuncType = FunctionType.Collation)]
    class CollationCaseInsensitive : SQLiteFunction
    {
        public override int Compare(string param1, string param2)       //According to Turkish character sorting to patch
        {
            return String.Compare(param1, param2, true);
        }
    }
    #endregion
    public static class Database
    {
        public static string DatabaseName = "baza.db";
        public static SQLiteConnection Connection = new SQLiteConnection(string.Format("Data Source={0};", DatabaseName));

        public static SQLiteDataReader GetReader(string query)
        {
            var readReader = new SQLiteCommand(query, Connection);
            SQLiteDataReader readerData;
            try
            {
                var reader = readReader.ExecuteReader();
                readerData = reader;
            }
            catch (System.Exception)
            {
                throw;
            }
            return readerData;
        }
        public static DataTable GetTable(string query)
        {
            var readTable = new SQLiteCommand(query, Connection);
            var dataTable = new DataTable();
            try
            {
                var reader = readTable.ExecuteReader();
                dataTable.Load(reader);
            }
            catch (System.Exception)
            {                
                throw;
            }
            return dataTable;
        }
        public static object GetScalar(string query)
        {
            var command = new SQLiteCommand(query, Connection);
            object data;
            try
            {
                data = command.ExecuteScalar();
            }
            catch (System.Exception)
            {
                throw;
            }
            return data;
        }
        public static bool Execute(string query)
        {
            var command = new SQLiteCommand(query, Connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }
    }
}