using Magazyn.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Magazyn.Services
{
    public interface IDatabaseService
    {
        IEnumerable<Product> GetProducts(); // sprawdza czy produkt isnieje w bazie
        Task<int> GetProduct(int IdProd);
        Task<int> GetWarehouse(int IdWare); // sprawdza czy produkt jest w magazynie
        Task<int> Sell(int IdProd, int IdWare, int amount); // realizuje sprzedaż
        Task<int> Buy(int IdProd, int IdWare, int amount); // dodaje istniejące produkty do tabeli Product_Warehouse
        Task<int> Add(Product p);  // dodaje produkty do tabeli product

    }
    public class DatabaseService : IDatabaseService
    {
        private readonly IPassword _pass;
        public DatabaseService(IPassword pass)
        {
            _pass = pass;
        }
        public IEnumerable<Product> GetProducts()
        {
            using var con = new SqlConnection(_pass.GetPassword());
            using var com = new SqlCommand("select * from product", con);
            con.Open();
            var dr = com.ExecuteReader();
            var result = new List<Product>();
            while (dr.Read())
            {
                result.Add(new Product
                {
                    IdProduct = (int) dr["IdProduct"],
                    Name = dr["Name"].ToString(),
                    Description = dr["Description"].ToString()
                });
            }

            return result;
        }
        public async Task<int> GetProduct(int IdProd)
        {
            //Product p1 = p;

            using var con = new SqlConnection(_pass.GetPassword());
            using var com = new SqlCommand();
            com.Connection = con;
            com.CommandText = "Select * from product Where IdProduct=@param1;";
            com.Parameters.AddWithValue("@param1", IdProd);


            await con.OpenAsync();
            using DbTransaction tran = await con.BeginTransactionAsync();
            com.Transaction = (SqlTransaction)tran;
            var result = new List<Product>();
            try
            {         
                using (var dr = await com.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        result.Add(new Product
                        {
                            Name = dr["Name"].ToString(),
                            Description = dr["Description"].ToString()
                        });
                    }
                }
                if (result.Count == 0)
                {
                    await con.CloseAsync();
                    return 2; // brak produktu zwróć "błąd"
                }
                else
                {
                    await con.CloseAsync();
                    return 1; // jest taki produkt
                }
            }
            catch (SqlException exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }
            catch (Exception exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }

            com.Parameters.Clear();
            await tran.CommitAsync();


        }

        public async Task<int> GetWarehouse(int IdWare)
        {
            //Product p1 = p;

            using var con = new SqlConnection(_pass.GetPassword());
            using var com = new SqlCommand();
            com.Connection = con;
            com.CommandText = "Select * from Warehouse Where IdWarehouse=@param1;";
            com.Parameters.AddWithValue("@param1", IdWare);


            await con.OpenAsync();
            using DbTransaction tran = await con.BeginTransactionAsync();
            com.Transaction = (SqlTransaction)tran;
            var result = new List<Warehouse>();
            try
            {
                using (var dr = await com.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        result.Add(new Warehouse
                        {
                            IdWarehouse = (int) dr["IdWarehouse"],
                            Name = dr["Name"].ToString()
                        });
                    }
                }
                if (result.Count == 0)
                {
                    await con.CloseAsync();
                    return 2; // brak magazynu zwróć "błąd"
                }
                else
                {
                    await con.CloseAsync();
                    return 1; // jest taki magazyn
                }
            }
            catch (SqlException exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }
            catch (Exception exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }

            com.Parameters.Clear();
            await tran.CommitAsync();


        }


        public async Task<int> Sell(int IdProd, int IdWare, int amount)
        {
            // notatka:
            // sprawdź czy produkt istnieje
            // sprawdź czy produkt jest na stanie w magazynie
            // sprawdź czy ilość jest wystarczająca
            // TAK - wydaj z magazynu i zapis sprzedaż
            // NIE - wystaw błąd

            // wydanie magazynu to:
            // zdejmij ze stanu o ilość
            //  utwórz zamówienie
            //


            return 1;
        }
        public async Task<int> Buy(int IdProd, int IdWare, int amount)
        // 1. sprawdź czy jest taki produkt w bazie produktów jeśli tak to pkt 2
        // 2. dodaj produkt do magazynu je

        {
            if((amount == 0) || (amount == null) || (IdProd == null) ||(IdWare== null)){
                return -3;
            } 
            using var con = new SqlConnection(_pass.GetPassword());
            using var com = new SqlCommand("AddProductToWarehouse", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@IdProduct", IdProd);
            com.Parameters.AddWithValue("@IdWarehouse", IdWare);
            com.Parameters.AddWithValue("@Amount", amount);
            com.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            //---
            

            await con.OpenAsync();
            using DbTransaction tran = await con.BeginTransactionAsync();
            com.Transaction = (SqlTransaction)tran;
            var result = new List<ProductWarehouse>();
            try
            {


                using (var dr = await com.ExecuteReaderAsync())
                {

                }
            }
            catch (SqlException exc)
            {
                //...
                await tran.RollbackAsync();
               string s = exc.Message.ToString();
                
                switch(s)
                {
                    case "Invalid parameter: Provided IdProduct does not exist":
                        return 0;
                    case "Invalid parameter: There is no order to fullfill":
                        return -1;
                    case "Invalid parameter: Provided IdWarehouse does not exist":
                        return -2;

                    default:
                        return -3;
                }
                

            }
            /// to być może w procedurze jest obsłużone
            // GetProduct(IdProd); // dorobić obsługę błędu
            // GetWarehouse(IdWare); // dorobić obsługę błędu
            com.Parameters.Clear();
            await tran.CommitAsync();
            return 1;
                // return 1;
         }
        
        public async Task<int> Add(Product p)
        {
            Product p1 = p;

            using var con = new SqlConnection(_pass.GetPassword());
            using var com = new SqlCommand();
            com.Connection = con;
            com.CommandText = "insert into product (Name, Description, Price) VALUES(@param1,@param2,@param3);";
            com.Parameters.AddWithValue("@param1", p1.Name);
            com.Parameters.AddWithValue("@param2", p1.Description);
            com.Parameters.AddWithValue("@param3", p1.Price);

            await con.OpenAsync();
            using DbTransaction tran = await con.BeginTransactionAsync();
            com.Transaction = (SqlTransaction)tran;
            try
            {
                await com.ExecuteNonQueryAsync();
                com.Parameters.Clear();
                await tran.CommitAsync();

            }
            catch (SqlException exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }
            catch (Exception exc)
            {
                //...
                await tran.RollbackAsync();
                return 0;
            }

            return 1;
        }

    }
}

