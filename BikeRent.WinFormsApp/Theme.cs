namespace BikeRent.WinFormsApp;

using System.Drawing;
using System.Windows.Forms;

public static class Theme
{
    public static readonly Color Header = ColorTranslator.FromHtml("#12444D");
    public static readonly Color Accent = ColorTranslator.FromHtml("#2BA39A");
    public static readonly Color AccentHover = ColorTranslator.FromHtml("#23908A");
    public static readonly Color Background = ColorTranslator.FromHtml("#ECEFF1");
    public static readonly Color Card = Color.White;
    public static readonly Color TextColor = ColorTranslator.FromHtml("#1F3138");
    public static readonly Color Muted = ColorTranslator.FromHtml("#5E7278");
    public static readonly Color Border = ColorTranslator.FromHtml("#CFD8DC");
    public static readonly Color GridAltRow = ColorTranslator.FromHtml("#F5F8FA");

    public static readonly Font Default = new("Segoe UI", 10F);
    public static readonly Font Bold = new("Segoe UI", 10F, FontStyle.Bold);
    public static readonly Font Title = new("Segoe UI", 15F, FontStyle.Bold);
    public static readonly Font Subtitle = new("Segoe UI", 9F);
    public static readonly Font HeaderGrid = new("Segoe UI", 10F, FontStyle.Bold);

    public static Button StyleButton(Button btn, bool primary = true)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = primary ? AccentHover : Header;
        btn.FlatAppearance.MouseOverBackColor = primary ? AccentHover : Header;
        btn.BackColor = primary ? Accent : Header;
        btn.ForeColor = Color.White;
        btn.Font = Bold;
        btn.Height = 36;
        btn.Cursor = Cursors.Hand;
        btn.TextAlign = ContentAlignment.MiddleCenter;
        return btn;
    }

    public static TextBox StyleTextBox(TextBox tb)
    {
        tb.BorderStyle = BorderStyle.FixedSingle;
        tb.BackColor = Color.White;
        tb.ForeColor = TextColor;
        tb.Font = Default;
        tb.Height = 30;
        return tb;
    }

    public static Label StyleLabel(Label lbl, bool bold = false)
    {
        lbl.ForeColor = TextColor;
        lbl.Font = bold ? Bold : Default;
        lbl.AutoSize = true;
        return lbl;
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Header;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = HeaderGrid;
        grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        grid.ColumnHeadersHeight = 38;
        grid.DefaultCellStyle.Font = Default;
        grid.DefaultCellStyle.ForeColor = TextColor;
        grid.DefaultCellStyle.SelectionBackColor = Accent;
        grid.DefaultCellStyle.SelectionForeColor = Color.White;
        grid.AlternatingRowsDefaultCellStyle.BackColor = GridAltRow;
        grid.RowHeadersVisible = false;
        grid.GridColor = Border;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.RowTemplate.Height = 34;
    }
}
