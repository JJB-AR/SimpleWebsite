<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="SimpleWebsite.AdminDashboard" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Administrator Dashboard</title>
</head>
<body>
<form id="form1" runat="server">
    <table border="0" cellpadding="6" cellspacing="0" width="100%">
        <tr>
            <td><h1>Administrator Dashboard</h1></td>
            <td align="right">Administrator&nbsp; <asp:Button ID="btnLogout" runat="server" Text="Log out" OnClick="btnLogout_Click" /></td>
        </tr>
    </table>
    <hr />

    <table border="1" cellpadding="8" cellspacing="0">
        <tr>
            <th>Total Users</th>
            <th>Active Users</th>
            <th>Inactive Users</th>
            <th>New This Week</th>
        </tr>
        <tr align="center">
            <td><asp:Label ID="lblTotalUsers" runat="server" /></td>
            <td><asp:Label ID="lblActiveUsers" runat="server" /></td>
            <td><asp:Label ID="lblInactiveUsers" runat="server" /></td>
            <td><asp:Label ID="lblNewUsers" runat="server" /></td>
        </tr>
    </table>

    <h2>Website Content</h2>
    <table border="0" cellpadding="5" cellspacing="0">
        <tr><td>Announcement:</td><td><asp:TextBox ID="tbAnnouncement" runat="server" TextMode="MultiLine" Rows="3" Columns="50" /></td></tr>
        <tr><td></td><td><asp:Button ID="btnSaveAnnouncement" runat="server" Text="Publish Announcement" OnClick="btnSaveAnnouncement_Click" /></td></tr>
    </table>

    <h2>User Management</h2>
    <p><asp:Label ID="lblMessage" runat="server" /></p>
    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" OnRowCommand="gvUsers_RowCommand" EmptyDataText="No users found.">
        <Columns>
            <asp:BoundField DataField="UserID" HeaderText="ID" />
            <asp:BoundField DataField="Username" HeaderText="Username" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:BoundField DataField="CreatedAt" HeaderText="Registered" DataFormatString="{0:g}" />
            <asp:CheckBoxField DataField="IsAdmin" HeaderText="Admin" />
            <asp:CheckBoxField DataField="IsActive" HeaderText="Active" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button ID="btnToggle" runat="server" Text="Toggle Active" CommandName="ToggleActive" CommandArgument='<%# Eval("UserID") + ":" + Eval("IsActive") %>' />
                        <asp:Button ID="btnToggleAdmin" runat="server" Text="Toggle Admin" CommandName="ToggleAdmin" CommandArgument='<%# Eval("UserID") + ":" + Eval("IsAdmin") %>' />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteUser" CommandArgument='<%# Eval("UserID") %>' OnClientClick="return confirm('Delete this user permanently?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</form>
</body>
</html>
