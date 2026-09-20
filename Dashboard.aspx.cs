using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace SimpleWebsite
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Convert.ToBoolean(Session["IsAdmin"] ?? false))
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadProfile();
                var announcement = Application["SiteAnnouncement"] as string;
                lblAnnouncement.Text = string.IsNullOrWhiteSpace(announcement) ? "No announcement has been published." : announcement;
                LoadStats();
            }
        }

        private int CurrentUserId { get { return Convert.ToInt32(Session["UserID"]); } }

        private void LoadProfile()
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand("SELECT u.Username, u.Email, c.EducationalAttainment, c.HobbiesAndInterests, c.Skills FROM Users u LEFT JOIN Content c ON c.USERID = u.UserID WHERE u.UserID = @UserID", conn)) //I don't understand
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return;
                    lblUsername.Text = reader["Username"].ToString();
                    tbUsername.Text = reader["Username"].ToString();
                    tbEmail.Text = reader["Email"].ToString();
                    tbEducationalAttainment.Text = reader["EducationalAttainment"] == DBNull.Value ? string.Empty : reader["EducationalAttainment"].ToString();
                    tbHobbiesAndInterests.Text = reader["HobbiesAndInterests"] == DBNull.Value ? string.Empty : reader["HobbiesAndInterests"].ToString();
                    tbSkills.Text = reader["Skills"] == DBNull.Value ? string.Empty : reader["Skills"].ToString();
                }
            }
        }

        private void LoadStats()
        {
            try
            {
                var connStr = GetConnectionString();
                if (string.IsNullOrWhiteSpace(connStr)) { lblError.Text = "Connection string missing."; return; }

                using (var conn = new SqlConnection(connStr))
                using (var cmdTotal = new SqlCommand("SELECT COUNT(*) FROM Users", conn))
                using (var cmdActive = new SqlCommand("SELECT COUNT(*) FROM Users WHERE IsActive = 1", conn))
                using (var cmdNewWeek = new SqlCommand("SELECT COUNT(*) FROM Users WHERE CreatedAt >= DATEADD(day, -7, GETUTCDATE())", conn))
                using (var cmdRecent = new SqlCommand("SELECT TOP 10 UserID, Username, Email, CreatedAt FROM Users ORDER BY CreatedAt DESC", conn))
                {
                    conn.Open();
                    litTotalUsers.Text = (cmdTotal.ExecuteScalar() ?? 0).ToString();
                    litActiveUsers.Text = (cmdActive.ExecuteScalar() ?? 0).ToString();
                    litNewThisWeek.Text = (cmdNewWeek.ExecuteScalar() ?? 0).ToString();

                    using (var adapter = new SqlDataAdapter(cmdRecent))
                    {
                        var table = new DataTable(); adapter.Fill(table);
                        gvRecentUsers.DataSource = table; gvRecentUsers.DataBind();
                    }
                }
            }
            catch (Exception ex) { lblError.Text = "Error loading dashboard: " + ex.Message; }
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            var username = tbUsername.Text.Trim();
            var email = tbEmail.Text.Trim();
            var password = tbPassword.Text;
            var message = lblProfileMessage;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                message.Text = "Username and email are required.";
                message.CssClass = "message error";
                return;
            }

            try
            {
                using (var conn = new SqlConnection(GetConnectionString()))
                using (var cmd = new SqlCommand("UPDATE Users SET Username = @Username, Email = @Email" + (string.IsNullOrEmpty(password) ? "" : ", Password = @Password") + " WHERE UserID = @UserID", conn))
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                    if (!string.IsNullOrEmpty(password)) cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = password;
                    cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserId;
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    using (var contentCommand = new SqlCommand(
                        "IF EXISTS (SELECT 1 FROM Content WHERE USERID = @UserID) " +
                        "UPDATE Content SET EducationalAttainment = @EducationalAttainment, HobbiesAndInterests = @HobbiesAndInterests, Skills = @Skills WHERE USERID = @UserID " +
                        "ELSE INSERT INTO Content (USERID, EducationalAttainment, HobbiesAndInterests, Skills) VALUES (@UserID, @EducationalAttainment, @HobbiesAndInterests, @Skills)", conn))
                    {
                        contentCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CurrentUserId;
                        contentCommand.Parameters.Add("@EducationalAttainment", SqlDbType.NVarChar, 255).Value = tbEducationalAttainment.Text.Trim();
                        contentCommand.Parameters.Add("@HobbiesAndInterests", SqlDbType.NVarChar, 255).Value = tbHobbiesAndInterests.Text.Trim();
                        contentCommand.Parameters.Add("@Skills", SqlDbType.NVarChar, 255).Value = tbSkills.Text.Trim();
                        contentCommand.ExecuteNonQuery();
                    }
                }

                Session["Username"] = username;
                message.Text = "Profile updated successfully.";
                message.CssClass = "message";
                LoadProfile();
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                message.Text = "That username or email is already in use.";
                message.CssClass = "message error";
            }
            catch (Exception ex)
            {
                message.Text = "Unable to update profile: " + ex.Message;
                message.CssClass = "message error";
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
