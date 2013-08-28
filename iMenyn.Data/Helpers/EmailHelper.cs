using System.Configuration;
using System.IO;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

namespace iMenyn.Data.Helpers
{
    public class EmailHelper
    {
        public static MailSettingsSectionGroup MailSettingsConfig
        {
            get
            {
                var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                return config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
            }
        }


        public static void NewEnterpriseNotification()
        {
            var smtpServer = MailSettingsConfig.Smtp.Network.Host;

            var message = new MailMessage();
            message.To.Add("jesse@imenyn.se");
            message.Subject = "Ny restaurang har inkommit!";
            message.From = new MailAddress("info@imenyn.se");
            message.Body = "Ny restaurang har inkommit!";
            message.BodyEncoding = System.Text.Encoding.UTF8;
            var smtp = new SmtpClient(smtpServer);
            try
            {
                smtp.SendAsync(message,"");
            }
            catch
            {
            }
        }
    }
}