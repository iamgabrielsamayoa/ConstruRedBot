using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ConstruRedBot.Classes_Folder.Database
{
    class ClsConection
    {
        public SqlConnection Connection;
        private String MainConnection { get; }

        /// <summary>
        /// Indicates SQL Connection Source
        /// </summary>
        public ClsConection()
        {
            //AZURE
            MainConnection= "Data Source=progra1-server.database.windows.net;Initial Catalog=prograI;Integrated Security=false;Persist Security Info=False;User ID=adminbot;Password=GABlearning2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //Local
            //MainConnection =  "Data Source=HP-ENVY-DV6\\SQLEXPRESS;Initial Catalog=prograI;Integrated Security=True";
        }


        /// <summary>
        /// Disconnects SQL Connection
        /// </summary>
        public void DisconnectDB()
        {
            Connection.Close();
        }
        /// <summary>
        /// As the name describes, it starts the connection with SQL
        /// </summary>
        public void StartSQL()
        {
            Connection = new SqlConnection(MainConnection);
            Connection.Open();
        }
        /// <summary>
        /// Runs a Query, this method handles open and closing from the DB
        /// </summary>
        /// <param name="sqll"></param>
        /// <returns></returns>
        public DataTable Query(String sqll)
        {
            StartSQL();
            SqlDataReader dr;
            SqlCommand comm = new SqlCommand(sqll, Connection);
            dr = comm.ExecuteReader();

            var dataTable = new DataTable();
            dataTable.Load(dr);
            DisconnectDB();
            return dataTable;
        }
        /// <summary>
        /// ejecuta una instrucción de insersion, eliminación y actualización,
        /// esta clase se encarga de manejar las aperturas y clausuras de la conexion.
        /// </summary>
        /// <param name="sqll"></param>
        public void RawQuery(String sqll)
        {
            StartSQL();
            try
            {

                SqlCommand Comm = new SqlCommand(sqll,Connection);
                Comm.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            finally
            {
                DisconnectDB();
            }



        }
    }
}
