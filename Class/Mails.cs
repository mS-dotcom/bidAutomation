using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Wantsan.Models;

namespace Wantsan.Class
{
    public class Mails
    {
        public string Gonder(string konu, string mesaj, string eposta)
        {
            vantsanEntities ent = new vantsanEntities();
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = new NetworkCredential(ent.Mail.FirstOrDefault().mail1.ToString(), ent.Mail.FirstOrDefault().password.ToString()); // Gönderici bilgilerini giriyoruz
                smtp.Port = 587; // Mail uzantınıza göre bu değişebilir
                smtp.Host = "smtp-mail.outlook.com"; // Gmail veya hotmail ise onların host bilgisini almanız gerekiyor 
                smtp.EnableSsl = true;
                
                mail.IsBodyHtml = true;// HTML tagleri kullanarak mail gönderebilmek için aktif ediyoruz
                mail.From = new MailAddress(ent.Mail.FirstOrDefault().mail1); // Gönderen mail adresini yazıyoruz
                mail.To.Add(eposta); // Gönderilecek mail adresini ekliyoruz
                mail.Subject = konu; // Mesaja konuyu ekliyoruz
                mail.Body = mesaj; // Mesajın içeriğini ekliyoruz

                smtp.Send(mail); // Mesajı gönderiyoruz
                return "basarili";
            }
            catch (Exception e)
            {
                return "basarisiz";
            }
        }
    }
}