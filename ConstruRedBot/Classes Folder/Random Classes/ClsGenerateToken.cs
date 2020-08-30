using System;
using System.Collections.Generic;
using System.Text;

namespace ConstruRedBot.Classes_Folder.Random_Classes
{
    class ClsGenerateToken
    {
        
        public string AlphaNumericToken(int longitudToken)
        {
            Random AlphaNumericToken = new Random();
            String Caracters = "abcdefghijklmnñopqrstuvwxyz0123456789";
            String token = "";
            for (int i = 0; i < longitudToken; i++)
            {
                int a = AlphaNumericToken.Next(Caracters.Length);
                token = token + Caracters[a];
            }
            
            return (token);
        }
        public string NumericToken(int longitudToken)
        {
            Random AlphaNumericToken = new Random();
            String Caracters = "0123456789";
            String token = "";
            for (int i = 0; i < longitudToken; i++)
            {
                int a = AlphaNumericToken.Next(Caracters.Length);
                token = token + Caracters[a];
            }

            return (token);
        }

    }
}
