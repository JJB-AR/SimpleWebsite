<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SimpleWebsite.Dashboard" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>
</head>
<body>
<form id="form1" runat="server">
    <table border="0" cellpadding="6" cellspacing="0" width="100%">
        <tr>
            <td><h1>Personal Dashboard</h1></td>
            <td align="right">Welcome, <asp:Label ID="lblUsername" runat="server" />&nbsp; <asp:Button ID="btnLogout" runat="server" Text="Log out" OnClick="btnLogout_Click" /></td>
        </tr>
    </table>
    <hr />

    <table border="1" cellpadding="8" cellspacing="0">
        <tr>
            <th>Total Users</th>
            <th>Active Users</th>
            <th>New This Week</th>
        </tr>
        <tr align="center">
            <td><asp:Label ID="litTotalUsers" runat="server" /></td>
            <td><asp:Label ID="litActiveUsers" runat="server" /></td>
            <td><asp:Label ID="litNewThisWeek" runat="server" /></td>
        </tr>
    </table>

    <h2>Personal Information</h2>
    <table border="0" cellpadding="5" cellspacing="0">
        <tr><td>Username:</td><td><asp:TextBox ID="tbUsername" runat="server" /></td></tr>
        <tr><td>Email:</td><td><asp:TextBox ID="tbEmail" runat="server" /></td></tr>
        <tr><td>New Password:</td><td><asp:TextBox ID="tbPassword" runat="server" TextMode="Password" /></td></tr>
        <tr><td>Educational Attainment:</td><td><asp:TextBox ID="tbEducationalAttainment" runat="server" TextMode="MultiLine" Rows="3" Columns="40" /></td></tr>
        <tr><td>Hobbies and Interests:</td><td><asp:TextBox ID="tbHobbiesAndInterests" runat="server" TextMode="MultiLine" Rows="3" Columns="40" /></td></tr>
        <tr><td>Skills:</td><td><asp:TextBox ID="tbSkills" runat="server" TextMode="MultiLine" Rows="3" Columns="40" /></td></tr>
        <tr><td></td><td><asp:Button ID="btnSaveProfile" runat="server" Text="Save Information" OnClick="btnSaveProfile_Click" /></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblProfileMessage" runat="server" /></td></tr>
    </table>

    <h2>Website Announcement</h2>
    <p><asp:Label ID="lblAnnouncement" runat="server" Text="No announcement has been published." /></p>

    <h2>Recently Registered Users</h2>
    <asp:GridView ID="gvRecentUsers" runat="server" AutoGenerateColumns="false" EmptyDataText="No users found.">
        <Columns>
            <asp:BoundField DataField="Username" HeaderText="Username" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:BoundField DataField="CreatedAt" HeaderText="Registered" DataFormatString="{0:g}" />
        </Columns>
    </asp:GridView>
    <p><asp:Label ID="lblError" runat="server" ForeColor="Red" /></p>
</form>
</body>
</html>
