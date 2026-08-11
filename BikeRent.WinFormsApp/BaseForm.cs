namespace BikeRent.WinFormsApp;

using System.Drawing;
using System.Windows.Forms;

public class BaseForm : Form
{
    public const int StandardWidth = 460;
    public const int StandardHeight = 492;
    public const int HeaderHeight = 62;
    public const int LabelX = 24;
    public const int BoxX = 174;
    public const int BoxWidth = 222;
    public const int RowHeight = 38;
    public const int ButtonHeight = 36;
    public const int ButtonGap = 8;

    protected readonly Panel Header;
    protected readonly Panel Body;
    protected readonly Label TitleLabel;

    public BaseForm(string title)
    {
        AutoScaleMode = AutoScaleMode.None;
        Font = Theme.Default;
        BackColor = Theme.Background;
        ForeColor = Theme.TextColor;
        StartPosition = FormStartPosition.CenterScreen;

        // Set full size window screen across all forms
        MinimumSize = new Size(600, 450);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = true;
        WindowState = FormWindowState.Maximized;

        Header = new Panel { Dock = DockStyle.Top, Height = HeaderHeight, BackColor = Theme.Header };
        TitleLabel = new Label
        {
            Text = title,
            ForeColor = Color.White,
            Font = Theme.Title,
            AutoSize = true,
            Location = new Point(20, 18)
        };
        Header.Controls.Add(TitleLabel);
        Header.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 4, BackColor = Theme.Accent });

        Body = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background, Padding = new Padding(0) };

        Controls.Add(Body);
        Controls.Add(Header);
    }

    protected void SetTitle(string title) => TitleLabel.Text = title;

    protected static Button Primary(string text, int x, int y, int w) =>
        Theme.StyleButton(new Button { Text = text, Location = new Point(x, y), Width = w });

    protected static Button Secondary(string text, int x, int y, int w) =>
        Theme.StyleButton(new Button { Text = text, Location = new Point(x, y), Width = w }, primary: false);

    protected static Label FieldLabel(string text, int x, int y) =>
        Theme.StyleLabel(new Label { Text = text, Location = new Point(x, y) }, bold: true);

    protected static TextBox FieldBox(int x, int y, int w) =>
        Theme.StyleTextBox(new TextBox { Location = new Point(x, y), Width = w });

    protected Panel StandardCard()
    {
        var wrapper = new Panel
        {
            Size = new Size(StandardWidth + 2, StandardHeight - HeaderHeight + 2),
            BackColor = Theme.Border,
            Padding = new Padding(1)
        };

        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Card,
            Padding = new Padding(24, 18, 24, 18),
            AutoScroll = true
        };
        wrapper.Controls.Add(card);

        void CenterCard()
        {
            if (Body.ClientSize.Width > 0 && Body.ClientSize.Height > 0)
            {
                var x = Math.Max(0, (Body.ClientSize.Width - wrapper.Width) / 2);
                var y = Math.Max(0, (Body.ClientSize.Height - wrapper.Height) / 2);
                wrapper.Location = new Point(x, y);
            }
        }

        Body.Resize += (_, _) => CenterCard();
        Body.ControlAdded += (_, e) => { if (e.Control == card || e.Control == wrapper) CenterCard(); };
        Body.Controls.Add(wrapper);
        CenterCard();

        return card;
    }

    protected static int ContentStartY(int fieldCount)
    {
        var bodyHeight = StandardHeight - HeaderHeight;
        var contentHeight = (fieldCount * RowHeight) + ButtonGap + ButtonHeight;
        var startY = (bodyHeight - contentHeight) / 2;
        return Math.Max(20, startY);
    }
}