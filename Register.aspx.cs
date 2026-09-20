using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimpleWebsite
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearInputs(this);
            lblMessage.Text = string.Empty;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            if (!Page.IsValid)
            {
                lblMessage.Text = "Please fix validation errors.";
                return;
            }

            var username = tbUsername.Text.Trim();
            var email = tbEmail.Text.Trim();
            var password = tbPassword.Text;
            var confirm = tbConfirmPassword.Text;

            if (!string.Equals(password, confirm))
            {
                lblMessage.Text = "Passwords do not match.";
                return;
            }

            try
            {
                var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(connStr)) //I was an idiot, so I added this if your connection string is configured correctly, fix it in web.config
                {
                    lblMessage.Text = "Connection string not configured.";
                    return;
                }

                using (var conn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("usp_RegisterUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    int newId = 0;
                    if (result != null && int.TryParse(result.ToString(), out newId))
                    {
                        if (newId == -1)
                        {
                            lblMessage.Text = "Username or email already exists.";
                            return;
                        }
                        // success - use PRG pattern to avoid repost on refresh - AI to sorry na.
                        Session["RegisteredMessage"] = "Registration successful.";
                        Response.Redirect(Request.RawUrl, false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        lblMessage.Text = "Registration failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }

        private void ClearInputs(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox tb)
                {
                    tb.Text = string.Empty;
                }
                else if (ctrl.HasControls())
                {
                    ClearInputs(ctrl);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                if (Session["RegisteredMessage"] != null)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = Session["RegisteredMessage"].ToString();
                    Session.Remove("RegisteredMessage");
                    ClearInputs(this);
                }
            }
        }
    }
}