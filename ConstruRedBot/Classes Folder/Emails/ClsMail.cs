using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Telegram.Bot.Types;

namespace ConstruRedBot.Classes_Folder.Emails
{
    class ClsEmail
    {
        public String SendEmail(CMParameter MailP)
        {
            var retorno = "OK";
            string SMTP = "smtp.gmail.com";
            string SMTPPORT = "587";
            string UserSMTP = "lsamayoal1@miumg.edu.gt";
            string PassSMPT = "$GAB197@";

            try
            {

                MailMessage o = new MailMessage(UserSMTP, MailP.EmailTo, MailP.EmailSubject, MailP.EmailBody);
                NetworkCredential netCred = new NetworkCredential(UserSMTP, PassSMPT);
                SmtpClient smtpobj = new SmtpClient(SMTP, Convert.ToInt32(SMTPPORT));
                smtpobj.EnableSsl = true;
                smtpobj.Credentials = netCred;


                smtpobj.Send(o);
                return retorno;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al Enviar el Correo:" + ex.Message);
                return "Error al Enviar el Correo:" + ex.Message;

            }
        }
    }
}
