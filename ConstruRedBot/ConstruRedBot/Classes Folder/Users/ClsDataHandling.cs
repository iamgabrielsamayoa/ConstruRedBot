using ConstruRedBot.Classes_Folder.Database;
using ConstruRedBot.Classes_Folder.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConstruRedBot.Classes_Folder
{
    class ClsDataHandling
    {//Cambiar la tabla SQL

        /// <summary>
        /// Adds Records to the DB from a file or another db
        /// </summary>
        /// <returns></returns>
        public List <CUsers> AddRecordstoDb()
        {
            ClsConection cn = new ClsConection();
            CUsers User = new CUsers();
            List<CUsers> AllUsers = new List<CUsers>();

            DataTable DT = cn.Query("SELECT * FROM [tb_ConsDb]");
            foreach(DataRow DR in DT.Rows)
            {
                User.Name = DR["Name"].ToString();
                User.LastName = DR[1].ToString();
                User.Email = DR["Email"].ToString();
                User.IdConstruRed = DR["IdConstruRed"].ToString();
                User.Department = DR["Department"].ToString();
                User.IdBot = DR["IdBot"].ToString();
                AllUsers.Add(User);
                User = new CUsers();
            }
            return AllUsers;
        }
        /// <summary>
        /// Finds User through his/her IDConstruRed
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// 
        public List<CUsers> AddNewCustRecordstoDb()
        {
            ClsConection cn = new ClsConection();
            CUsers User = new CUsers();
            List<CUsers> AllUsers = new List<CUsers>();

            DataTable DT = cn.Query("SELECT * FROM [tb_newcustomers]");
            foreach (DataRow DR in DT.Rows)
            {
                User.NewCustID = DR[0].ToString();
                User.FullName = DR[1].ToString();
                User.PhoneNumber = DR[2].ToString();
                User.IdBot = DR["IdBot"].ToString();
                AllUsers.Add(User);
                User = new CUsers();
            }
            return AllUsers;
        }
        public CUsers FindUserthID(String Id)
        {
            ClsConection cn = new ClsConection();
            CUsers User = new CUsers();
            User.IdBot = "Empty";
            DataTable DT = cn.Query("SELECT *  FROM [tb_ConsDb] where IdConstruRed='" + Id + "'");

            foreach(DataRow DR in DT.Rows)
            {
                User.Name = DR["Name"].ToString();
                User.LastName = DR[1].ToString();
                User.Email = DR["Email"].ToString();
                User.IdConstruRed = DR["IdConstruRed"].ToString();
                User.Department = DR["Department"].ToString();
                User.IdBot = DR["IdBot"].ToString();
            }
            return User;

        }
        

    }
}
