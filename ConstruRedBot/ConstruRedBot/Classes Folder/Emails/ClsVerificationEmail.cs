using ConstruRedBot.Classes_Folder.Random_Classes;
using ConstruRedBot.Classes_Folder.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConstruRedBot.Classes_Folder.Emails
{
    class ClsVerificationEmail
    {
        /// <summary>
        /// envia correo para verificar a la persona

        /// </summary>
        /// <param name="UserObj"></param>
        /// 

    public String FirstVerification(CUsers UserObj)
        {
            ClsGenerateToken rnd = new ClsGenerateToken();
            ClsEmail mail = new ClsEmail();
            CMParameter Par = new CMParameter();
            CUsers user = new CUsers();
            String Token = rnd.AlphaNumericToken(5);

            Par.EmailTo = UserObj.Email;
            Par.EmailSubject = "Verificacion de Identidad LGabBot";
            Par.EmailBody = "Hola,"+ user.Name + user.LastName+ " este correo es para verificar tu identidad para usar mi Telegram Bot!!!\n";
            Par.EmailBody += "Usando Telegram envia la palabra verificar seguido de "+ Token;
            Par.EmailBody += "\n Ejemplo: \n verificar 12345";
            Par.EmailBody += "\n\n Gracias por tu tiempo, Espero el bot sea de ayuda";

            UserObj.UpdateStart(UserObj, Token, false);
            return mail.SendEmail(Par);

        }
        public String CallCustomer(string fullname, string PhoneNumber)
        {
            List<CUsers> AllCustomers = new CUsers().SendsCustDatatoDB();//acutaliza los datos del cliente una vez agregado el numero de telefono
            ClsGenerateToken rnd = new ClsGenerateToken();
            ClsEmail mail = new ClsEmail();
            CMParameter Par = new CMParameter();
            CUsers user = new CUsers();
            String Token = rnd.AlphaNumericToken(5);

            Par.EmailTo = "lsamayoal1@miumg.edu.gt";//Aca elegi mi correo pero podria hacerse dinamico si hubiesen mas asesores
            Par.EmailSubject ="Nuevo Registro de Contacto LGabBot";
            Par.EmailBody = "Hola, este correo es para notificarte que tienes un nuevo cliente para contactar\n";
            Par.EmailBody += "sus datos son los siguientes: \n Nombre completo:" + fullname+ "\nNumero de telefono:"+PhoneNumber;
            Par.EmailBody += "\n\n Gracias por tu tiempo, Espero el bot sea de ayuda";

            return mail.SendEmail(Par);

        }
    }
}
