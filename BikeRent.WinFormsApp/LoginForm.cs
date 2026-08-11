namespace BikeRent.WinFormsApp;

using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Services;

public class LoginForm : BaseForm
{
    private readonly BikeRentService _service;
    private readonly TextBox _txtPhone;
    private readonly TextBox _txtPassword;
    private readonly Button _btnLogin;
    private readonly Button _btnRegister;

    public LoginForm(BikeRentService service) : base("Welcome back")
    {
        _service = service;

        Text = "Login - Bike Rental System";

        Body.BackColor = Theme.Card;

        var card = StandardCard();

        card.BorderStyle = BorderStyle.None;
        card.BackColor = Theme.Card;

        var startY = ContentStartY(2);

        card.Controls.Add(FieldLabel("Phone number", LabelX, startY + 4));
        _txtPhone = FieldBox(BoxX, startY, BoxWidth);
        card.Controls.Add(_txtPhone);

        card.Controls.Add(FieldLabel("Password", LabelX, startY + RowHeight + 4));
        _txtPassword = FieldBox(BoxX, startY + RowHeight, BoxWidth);
        _txtPassword.UseSystemPasswordChar = true;
        card.Controls.Add(_txtPassword);

        var btnY = startY + 2 * RowHeight + ButtonGap;
        _btnLogin = Primary("Log In", BoxX, btnY, 108);
        _btnRegister = Secondary("Create Account", BoxX + 114, btnY, 108);
        card.Controls.Add(_btnLogin);
        card.Controls.Add(_btnRegister);

        _btnLogin.Click += BtnLogin_Click;
        _btnRegister.Click += (_, _) => new RegisterForm(_service).ShowDialog(this);
        _txtPassword.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) _btnLogin.PerformClick();
        };

        AcceptButton = _btnLogin;
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        var user = _service.Login(_txtPhone.Text.Trim(), _txtPassword.Text);
        if (user is null)
        {
            MessageBox.Show(this, "Invalid phone or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var main = new MainForm(_service, user);
        main.FormClosed += (_, _) => Show();
        main.Show();
        Hide();
    }
}
