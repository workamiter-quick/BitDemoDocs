using Crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;


namespace Crud.Controllers
{
    public class DBHelper
    {

        private readonly IConfiguration _configuration;
        public DBHelper(IConfiguration _conf)
        {
            _configuration = _conf;
        }

        public DataSet ExecuteSQL(string SQL)
        {

            DataSet gpDataSet = new DataSet();
            NpgsqlConnection conn = null;
            NpgsqlDataAdapter Oda = null;
            try
            {

                string oradb = _configuration.GetConnectionString("PostgressSpatialCon");
                conn = new NpgsqlConnection(oradb);
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;

                Oda = new NpgsqlDataAdapter(cmd);
                gpDataSet = new DataSet();
                Oda.Fill(gpDataSet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }

            return gpDataSet;

        }

        public bool ExecuteNonQuery(string ExecuteQuery)
        {
            NpgsqlConnection conn = null;
            bool isExecute = false;
            try
            {
                string oradb = _configuration.GetConnectionString("PostgressSpatialCon");
                using (conn = new NpgsqlConnection(oradb))
                using (NpgsqlCommand cmd = new NpgsqlCommand(ExecuteQuery, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    isExecute = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            return isExecute;
        }

        public bool ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            bool isExecute = false;
            try
            {
                string connectionString = _configuration.GetConnectionString("PostgressSpatialCon");

                using (var conn = new NpgsqlConnection(connectionString))
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    isExecute = true;
                }
            }
            catch
            {
                throw; // Preserve original stack trace
            }
            return isExecute;
        }

        public int ExecuteScalar(string ExecuteQuery)
        {
            NpgsqlConnection conn = null;
            int generatedId = 0;
            try
            {
                string oradb = _configuration.GetConnectionString("PostgressSpatialCon");
                using (conn = new NpgsqlConnection(oradb))
                using (NpgsqlCommand cmd = new NpgsqlCommand(ExecuteQuery, conn))
                {
                    conn.Open();
                    generatedId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            return generatedId;
        }

        public int ExecuteScalar(string query, params NpgsqlParameter[] parameters)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("PostgressSpatialCon");

                using (var conn = new NpgsqlConnection(connectionString))
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch
            {
                throw;
            }
        }

    }

    public class EmailService
    {

        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration _conf)
        {
            _configuration = _conf;
        }
        /// <summary>
        /// Sends an email using the SMTP settings provided in the image.
        /// This is a synchronous method.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="body">The body content of the email (can be HTML).</param>
        /// <returns>True if the email was sent successfully, otherwise false.</returns>

        private bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                // The sender's email address and name from the image.
                string fromEmail = _configuration["EmailDetails:fromEmail"];
                string fromName = _configuration["EmailDetails:fromName"];

                // IMPORTANT: You should never hard-code your password in the code.
                // Use a secure method like a secret manager or environment variables.
                string fromPassword = _configuration["EmailDetails:fromPassword"];

                // SMTP server settings from the provided image.
                string smtpServer = _configuration["EmailDetails:smtpServer"];
                int smtpPort = int.Parse(_configuration["EmailDetails:smtpPort"]);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                string[] arrEmails = toEmail.Split(';');
                foreach (string email in arrEmails)
                {
                    message.To.Add(MailboxAddress.Parse(email));
                }
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    client.Authenticate(fromEmail, fromPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MailKit error: {ex.Message}");
                return false;
            }
        }
        public void SendEmailToAdmin(ApiKeyInput AKI)
        {
            // The sender's email address and name from the image.
            string AppURL = _configuration["AppURL"];

            string subject = "API Key Request";
            string body = "You have received a new API key request from a company. " +
                "Click the link below to approve or reject:" +
                " " + AppURL + "Approve.html?em=" + AKI.EmailAddress;

            bool isEMailed = SendEmail(_configuration["EmailDetails:adminEmail"], subject, body);
        }

        public void SendEmailToUserAfterApproval(string APIKey, string email)
        {
            // The sender's email address and name from the image.
            string AppURL = _configuration["AppURL"];

            string subject = "API Key Request Approval";
                string body = @"
                Dear User,<br/><br/>
                Your request for an API key has been approved.<br/><br/>
                Here are your details:<br/>
                <strong>API Key:</strong> "+ APIKey + "<br/><br/>Thank you for using our services.<br/><br/>Best regards,<br/>Link Africa Support Team";

            bool isEMailed = SendEmail(email, subject, body);
        }
    }

}
