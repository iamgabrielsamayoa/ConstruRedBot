using ConstruRedBot.Classes_Folder.Database;
using ConstruRedBot.Classes_Folder.Random_Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ConstruRedBot.Classes_Folder.Users
{
    class CUsers
    {
        public String LastName { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String IdConstruRed { get; set; }
        public String Department { get; set; }
        public String IdBot { get; set; }
        public String NewCustID { get; set; }
        public String FullName { get; set; }
        public String PhoneNumber { get; set; }
        public String UniqueToken { get; set; }
        public List<CUsers> AllUsers { get; set; }

        public CUsers()
        {
            AllUsers = new List<CUsers>();
        }

        public List<CUsers> AddData(String File)
        {

            return new ClsFileStream().ReadAddRecordsFile(File);
        }
        /// <summary>
        /// carga todos los alumnos de la tabla de alumnos
        /// los regresa en una estructura de datos tipo List
        /// </summary>
        /// <returns></returns>
        public List<CUsers> SendsDatatoDB()
        {
            return new ClsDataHandling().AddRecordstoDb();
        }

        public List<CUsers> SendsCustDatatoDB()
        {
            return new ClsDataHandling().AddNewCustRecordstoDb();
        }

        public CUsers SendsUsertoDB(String Id)
        {
            ClsDataHandling UQuery = new ClsDataHandling();
            return UQuery.FindUserthID(Id);
        }

        public bool VerifiesUserRegistry(String Id)
        {
            CUsers USearch = SendsUsertoDB(Id);
            if (USearch.IdConstruRed.Equals("Empty"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void UpdateStart(CUsers user, string keyword, bool verified)//Keyword == Telegram ID if the process goes properly
        {
            ClsConection cn = new ClsConection();
            cn.StartSQL();
            String Query;
            if (verified)
            {
                Query = keyword.ToString();
            }
            else
            {
                Query = "P" + keyword.ToString();
            }
            String sql;
            if (VerifiesUserRegistry(user.IdConstruRed))
            {
                sql = "update tb_ConsDb  set  idbot='" + Query + "' where IdConstruRed='" + user.IdConstruRed + "'";
            }
            else
            {
                sql = "update tb_ConsDb  set  idbot='" + Query + "' where IdConstruRed='" + user.IdConstruRed + "'";
            }
            SqlCommand Command = new SqlCommand(sql, cn.Connection);
            Command.ExecuteNonQuery();
        }
        public void NewContactCreation(string keyword)
        {
            ClsConection cn = new ClsConection();
            cn.StartSQL();
            ClsGenerateToken token = new ClsGenerateToken();
            String Query;
            
         Query = keyword.ToString();
         String sql;
            
         sql = "insert into tb_newcustomers (idbot) values('"+ Query + "')";
           
            SqlCommand Command = new SqlCommand(sql, cn.Connection);
            Command.ExecuteNonQuery();
        }

        public void AddPDV(string keyword, string pdv)
        {
            ClsConection cn = new ClsConection();
            cn.StartSQL();
            ClsGenerateToken token = new ClsGenerateToken();
            String Query;
            String PdvName;

            PdvName = pdv.ToString();
            Query = keyword.ToString();
            String sql;

            sql = "update tb_locationCons set Pdv='" + PdvName + "' where botid ='"+Query+"'";

            SqlCommand Command = new SqlCommand(sql, cn.Connection);
            Command.ExecuteNonQuery();
        }

        public void NewProspecto(string customerinfo, string keyword, bool verified)
        {
            ClsConection cn = new ClsConection();
            cn.StartSQL();
            string NewUserToken;
            ClsGenerateToken token = new ClsGenerateToken();
            NewUserToken = token.AlphaNumericToken(5);
            String Query;
            if (verified)
            {
                Query = keyword.ToString();
            }
            else
            {
                Query = keyword.ToString();
            }
            String sql;
            if (verified == true)//verified agregara el nombre
            {
               
                sql = "insert into tb_ProspectoCons (FullName,botid) values('" + customerinfo + "','" + Query + "'";
            }
            else //non verified agregara el numero de telefono
            {
                sql = "update tb_ProspectoCons  set  PhoneNumber='" + customerinfo + "' where idbot='" + Query + "'";
            }
            SqlCommand Command = new SqlCommand(sql, cn.Connection);

            Command.ExecuteNonQuery();

        }

        public void NewContact(string customerinfo, string keyword, bool verified)
        {
            ClsConection cn = new ClsConection();
            cn.StartSQL();
            string NewUserToken;
            ClsGenerateToken token = new ClsGenerateToken();
            NewUserToken = token.AlphaNumericToken(5);
            String Query;
            if (verified)
            {
                Query = keyword.ToString();
            }
            else
            {
                Query = keyword.ToString();
            }
            String sql;
            if (verified == true)//verified agregara el nombre
            {
                sql = "update tb_newcustomers  set  FullName='" + customerinfo + "' where idbot='" + Query + "'";
            }
            else //non verified agregara el numero de telefono
            {
                sql = "update tb_newcustomers  set  PhoneNumber='" + customerinfo + "' where idbot='" + Query + "'";
            }
            SqlCommand Command = new SqlCommand(sql, cn.Connection);

            Command.ExecuteNonQuery();
           
        }
    } 
}
