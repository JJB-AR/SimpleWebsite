using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimpleWebsite
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                lblMessage.Text = "Please fix validation errors.";
                return;
            }

            if (Session["LoginLocked"] is bool locked && locked)
            {
                lblMessage.Text = "Login temporarily locked due to repeated failures.";
                return;
            }

            var login = tbLogin.Text.Trim();
            var password = tbPassword.Text;

            try
            {
                var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(connStr))
                {
                    lblMessage.Text = "Connection string not configured.";
                    return;
                }

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("usp_AuthenticateUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsernameOrEmail", login);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            var isActive = rdr["IsActive"] != DBNull.Value && Convert.ToBoolean(rdr["IsActive"]);
                            var isAdmin = rdr["IsAdmin"] != DBNull.Value && Convert.ToBoolean(rdr["IsAdmin"]);
                            var username = rdr["Username"]?.ToString();
                            var userId = rdr["UserID"] != DBNull.Value ? Convert.ToInt32(rdr["UserID"]) : 0;

                            if (!isActive)
                            {
                                lblMessage.Text = "Account is inactive.";
                                return;
                            }

                            // Authentication successful
                            FormsAuthentication.SetAuthCookie(username, false);
                            Session["UserID"] = userId;
                            Session["Username"] = username;
                            Session["IsAdmin"] = isAdmin;
                            Session.Remove("LoginFails");

                            var returnUrl = Request.QueryString["ReturnUrl"];
                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                Response.Redirect(returnUrl, false);
                                Context.ApplicationInstance.CompleteRequest();
                                return;
                            }

                            var dest = isAdmin ? "~/AdminDashboard.aspx" : "~/Dashboard.aspx";
                            Response.Redirect(dest, false);
                            Context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                        else
                        {
                            // invalid credentials
                            int fails = 0;
                            if (Session["LoginFails"] != null)
                                int.TryParse(Session["LoginFails"].ToString(), out fails);
                            fails++;
                            Session["LoginFails"] = fails;
                            if (fails >= 5)
                            {
                                Session["LoginLocked"] = true;
                                lblMessage.Text = "Too many failed attempts. Login locked.";
                            }
                            else
                            {
                                lblMessage.Text = "Invalid username or password.";
                            }
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }
    }
}