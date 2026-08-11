namespace BikeRent.WinFormsApp;

using BikeRent.Domain.Services;

public class RegisterForm : BaseForm
{
    private readonly BikeRentService _service;
    private readonly TextBox _txtName;
    private readonly TextBox _txtPhone;
    private readonly TextBox _txtPassword;
    private readonly TextBox _txtConfirm;

    public RegisterForm(BikeRentService service) : base("Create account")
    {
        _service = service;

        Text = "Register - Bike Rental System";
        StartPosition = FormStartPosition.CenterParent;

        var card = StandardCard();

        var startY = ContentStartY(4);

        card.Controls.Add(FieldLabel("Full name", LabelX, startY + 4));
        _txtName = FieldBox(BoxX, startY, BoxWidth);
        card.Controls.Add(_txtName);

        card.Controls.Add(FieldLabel("Phone number", LabelX, startY + RowHeight + 4));
        _txtPhone = FieldBox(BoxX, startY + RowHeight, BoxWidth);
        card.Controls.Add(_txtPhone);

        card.Controls.Add(FieldLabel("Password", LabelX, startY + 2 * RowHeight + 4));
        _txtPassword = FieldBox(BoxX, startY + 2 * RowHeight, BoxWidth);
        _txtPassword.UseSystemPasswordChar = true;
        card.Controls.Add(_txtPassword);

        card.Controls.Add(FieldLabel("Confirm password", LabelX, startY + 3 * RowHeight + 4));
        _txtConfirm = FieldBox(BoxX, startY + 3 * RowHeight, BoxWidth);
        _txtConfirm.UseSystemPasswordChar = true;
        card.Controls.Add(_txtConfirm);

        var btnY = startY + 4 * RowHeight + ButtonGap;
        var btnRegister = Primary("Register", BoxX, btnY, BoxWidth);
        btnRegister.Click += BtnRegister_Click;
        card.Controls.Add(btnRegister);

        AcceptButton = btnRegister;
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        if (_txtPassword.Text != _txtConfirm.Text)
        {
            MessageBox.Show(this, "Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = _service.RegisterCustomer(_txtName.Text.Trim(), _txtPhone.Text.Trim(), _txtPassword.Text);
        if (!result.Success)
        {
            MessageBox.Show(this, result.Message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        MessageBox.Show(this, result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}
