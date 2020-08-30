using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConstruRedBot.Classes_Folder.Users
{
    class ClsFileStream
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OpenFile"></param>
        /// <param name="Department">sin parametro trae todas</param>
        /// <returns></returns>
        public List<CUsers> ReadAddRecordsFile(String OpenFile)
        {
            CUsers User = new CUsers();
            List<CUsers> Users = new List<CUsers>();
            String Content = String.Empty;
            String Line;

            StreamReader file = new StreamReader(OpenFile);
            while ((Line = file.ReadLine()) != null)
            {
                var Data = Line.Split(';');
                User.LastName = Data[0];
                User.Name = Data[1];
                User.Email = Data[2];
                User.IdConstruRed = Data[3];
                User.Department = Data[4];
                Users.Add(User);
                User = new CUsers();
            }
            return Users;
        }
    }
}
