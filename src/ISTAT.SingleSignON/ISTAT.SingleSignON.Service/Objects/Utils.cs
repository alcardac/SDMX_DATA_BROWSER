using ISTAT.SingleSignON.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace ISTAT.SingleSignON.Service
{
    public class Utils
    {
        public static string SAcode = "SA";
        public static ISTATUser ConvertISTATUser(User user)
        {
            ISTATUser istatUser = new ISTATUser()
            {
                UserCode = user.UserCode == null ? null : user.UserCode.Trim(),
                Email = user.Email == null ? null : user.Email.Trim(),
                Name = user.Name == null ? null : user.Name.Trim(),
                Surname = user.Surname == null ? null : user.Surname.Trim(),
                Sex = user.Sex == null ? null : user.Sex.Trim(),
                Age = user.Age == null ? null : user.Age.Trim(),
                Country = user.Country == null ? null : user.Country.Trim(),
                Study = user.Study == null ? null : user.Study.Trim(),
                Position = user.Position == null ? null : user.Position.Trim(),
                Agency = user.Agency == null ? null : user.Agency.Trim(),
                Lang = user.Lang == null ? null : user.Lang.Trim(),
                IsSA = (user.Role != null && user.Role.Trim() == SAcode)
            };
            var themes = (from pre in user.UserPreferences select pre.PreferenceCode.Trim());
            if (themes != null)
                istatUser.Themes = themes.ToList();
            return istatUser;
        }

        internal static bool ValidatePassword(string pass)
        {
            if (string.IsNullOrEmpty(pass) || pass.Trim().Length < 5)
                return false;
            else
                return true;
        }

        internal static string CriptaPassword(string pass)
        {
            UnicodeEncoding Ue = new UnicodeEncoding();
            Byte[] ByteSourceText = Ue.GetBytes(pass.ToUpper());
            MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();
            Byte[] ByteHash = Md5.ComputeHash(ByteSourceText);
            return Convert.ToBase64String(ByteHash);
        }

        internal static string RandomPassword()
        {
            int CharNewPass = 5;
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < CharNewPass; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        internal static void SendMail(string Mail, MailObject passobj)
        {
            MailMessage message = new MailMessage(Mail, passobj.MailSender);
            message.Subject = passobj.MailSubject;
            message.Body = passobj.MailTemplate;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient(passobj.MailSMTPServer);
            client.UseDefaultCredentials = true;
            client.Send(message);
               
        }
    }
}
