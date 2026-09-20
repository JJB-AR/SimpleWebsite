using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimpleWebsite
{
    public partial class AdminDashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || !Convert.ToBoolean(Session["IsAdmin"] ?? false))
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack) LoadDashboard();
        }

        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString; }
        }

        private void LoadDashboard()
        {
            try
            {
                tbAnnouncement.Text = Application["SiteAnnouncement"] as string ?? string.Empty;
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    lblTotalUsers.Text = Scalar(conn, "SELECT COUNT(*) FROM Users").ToString();
                    lblActiveUsers.Text = Scalar(conn, "SELECT COUNT(*) FROM Users WHERE IsActive = 1").ToString();
                    lblInactiveUsers.Text = Scalar(conn, "SELECT COUNT(*) FROM Users WHERE IsActive = 0").ToString();
                    lblNewUsers.Text = Scalar(conn, "SELECT COUNT(*) FROM Users WHERE CreatedAt >= DATEADD(day, -7, GETUTCDATE())").ToString();
                }

                BindUsers();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Unable to load administrator data: " + ex.Message;
                lblMessage.CssClass = "message error";
            }
        }

        protected void btnSaveAnnouncement_Click(object sender, EventArgs e)
        {
            Application["SiteAnnouncement"] = tbAnnouncement.Text.Trim();
            lblMessage.Text = "Website announcement published.";
            lblMessage.CssClass = "message";
        }

        private object Scalar(SqlConnection conn, string sql)
        {
            using (var cmd = new SqlCommand(sql, conn)) return cmd.ExecuteScalar() ?? 0;
        }

        private void BindUsers()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT UserID, Username, Email, CreatedAt, IsAdmin, IsActive FROM Users ORDER BY CreatedAt DESC", conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                var table = new DataTable();
                adapter.Fill(table);
                gvUsers.DataSource = table;
                gvUsers.DataBind();
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int userId;
            if (!int.TryParse(e.CommandArgument.ToString().Split(':')[0], out userId)) return;
            if (userId == Convert.ToInt32(Session["UserID"]))
            {
                lblMessage.Text = "The current administrator cannot be changed or deleted here.";
                lblMessage.CssClass = "message error";
                return;
            }

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.Connection.Open();
                    if (e.CommandName == "ToggleActive")
                    {
                        bool currentlyActive;
                        if (!bool.TryParse(e.CommandArgument.ToString().Split(':')[1], out currentlyActive)) return;
                        cmd.CommandText = "UPDATE Users SET IsActive = @IsActive WHERE UserID = @UserID";
                        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = !currentlyActive;
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                        cmd.ExecuteNonQuery();
                        lblMessage.Text = "User status updated.";
                    }
                    else if (e.CommandName == "ToggleAdmin")
                    {
                        bool currentlyAdmin;
                        if (!bool.TryParse(e.CommandArgument.ToString().Split(':')[1], out currentlyAdmin)) return;
                        cmd.CommandText = "UPDATE Users SET IsAdmin = @IsAdmin WHERE UserID = @UserID";
                        cmd.Parameters.Add("@IsAdmin", SqlDbType.Bit).Value = !currentlyAdmin;
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                        cmd.ExecuteNonQuery();
                        lblMessage.Text = "Administrator status updated.";
                    }
                    else if (e.CommandName == "DeleteUser")
                    {
                        cmd.CommandText = "DELETE FROM Users WHERE UserID = @UserID";
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
                        cmd.ExecuteNonQuery();
                        lblMessage.Text = "User deleted.";
                    }
                }
                lblMessage.CssClass = "message";
                LoadDashboard();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Unable to change the user: " + ex.Message;
                lblMessage.CssClass = "message error";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
