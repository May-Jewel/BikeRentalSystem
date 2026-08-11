namespace BikeRent.WinFormsApp;

public class InputDialog : BaseForm
{
    private readonly TextBox[] _boxes;

    public string[] Values => _boxes.Select(b => b.Text.Trim()).ToArray();

    public InputDialog(string title, params (string Label, string Initial)[] fields) : base(title)
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;

        _boxes = new TextBox[fields.Length];

        var card = StandardCard();

        var startY = ContentStartY(fields.Length);

        for (var i = 0; i < fields.Length; i++)
        {
            var y = startY + i * RowHeight;
            card.Controls.Add(FieldLabel(fields[i].Label, LabelX, y + 4));
            var box = FieldBox(BoxX, y, BoxWidth);
            box.Text = fields[i].Initial;
            card.Controls.Add(box);
            _boxes[i] = box;
        }

        var btnY = startY + fields.Length * RowHeight + ButtonGap;
        var btnOk = Primary("OK", BoxX, btnY, 108);
        var btnCancel = Secondary("Cancel", BoxX + 114, btnY, 108);
        btnOk.DialogResult = DialogResult.OK;
        btnCancel.DialogResult = DialogResult.Cancel;
        card.Controls.Add(btnOk);
        card.Controls.Add(btnCancel);

        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    public static string[]? Ask(string title, params (string Label, string Initial)[] fields)
    {
        var dlg = new InputDialog(title, fields);
        return dlg.ShowDialog() == DialogResult.OK ? dlg.Values : null;
    }
}
