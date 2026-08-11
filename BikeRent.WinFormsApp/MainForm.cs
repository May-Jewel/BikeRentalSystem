namespace BikeRent.WinFormsApp;

using System.Drawing;
using System.Windows.Forms;
using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Services;

public class MainForm : BaseForm
{
    private readonly BikeRentService _service;
    private readonly TblUser _user;
    private readonly DataGridView _dgv = new();
    private readonly StatusStrip _status = new();
    private readonly ToolStripStatusLabel _lblStatus = new();
    private Button? _activeNavButton;

    public MainForm(BikeRentService service, TblUser user) : base("Bike Rental System")
    {
        _service = service;
        _user = user;

        Text = $"Bike Rental System - {user.Name} ({user.Role})";
        MinimumSize = new Size(840, 520);
        Body.Padding = new Padding(0);

        // Build Left Side Navigation Sidebar
        var sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 240,
            BackColor = Theme.Card,
            Padding = new Padding(0)
        };

        // Border divider between sidebar and main content area
        var divider = new Panel
        {
            Dock = DockStyle.Right,
            Width = 1,
            BackColor = Theme.Border
        };
        sidebar.Controls.Add(divider);

        // User info box at top of sidebar
        var userCard = new Panel
        {
            Dock = DockStyle.Top,
            Height = 85,
            BackColor = Theme.Header,
            Padding = new Padding(16, 10, 16, 10)
        };
        var lblUserInfo = new Label
        {
            Text = $"👤 {user.Name}\nRole: {user.Role.ToUpper()}",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = Theme.Bold,
            Height = 22,
            TextAlign = ContentAlignment.MiddleLeft
        };
        userCard.Controls.Add(lblUserInfo);

        // Sidebar scrollable items container
        var navList = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Theme.Card,
            Padding = new Padding(6, 8, 6, 8)
        };

        // Logout button at bottom of sidebar
        var btnLogout = new Button
        {
            Text = "🚪  Logout",
            Dock = DockStyle.Bottom,
            Height = 44,
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Header,
            ForeColor = Color.White,
            Font = Theme.Bold,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0)
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Click += (_, _) => Close();

        List<Control> navItems = new();
        Button? defaultActiveBtn = null;

        void SetActiveNav(Button btn)
        {
            if (_activeNavButton != null)
            {
                _activeNavButton.BackColor = Theme.Card;
                _activeNavButton.ForeColor = Theme.TextColor;
                _activeNavButton.Font = Theme.Default;
            }
            _activeNavButton = btn;
            _activeNavButton.BackColor = Theme.Accent;
            _activeNavButton.ForeColor = Color.White;
            _activeNavButton.Font = Theme.Bold;
        }

        Button AddButton(string title, Action action)
        {
            Button btn = null!;
            btn = new Button
            {
                Text = "  " + title,
                Height = 40,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card,
                ForeColor = Theme.TextColor,
                Font = Theme.Default,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Margin = new Padding(0, 1, 0, 1),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#E0F2F1");
            btn.Click += (_, _) =>
            {
                SetActiveNav(btn);
                action();
            };
            navItems.Add(btn);
            return btn;
        }

        void AddHeader(string title)
        {
            var lbl = new Label
            {
                Text = title.ToUpper(),
                Height = 30,
                Dock = DockStyle.Top,
                ForeColor = Theme.Muted,
                Font = new Font(Theme.Default.FontFamily, 8.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(14, 0, 0, 4)
            };
            navItems.Add(lbl);
        }

        if (user.Role == "admin")
        {
            //AddHeader("Admin Actions");
            defaultActiveBtn = AddButton("All Bikes", ShowAllBikes);
            AddButton("Add New Bike", AddBike);
            AddButton("Update Bike", EditBike);
            AddButton("View Past Rentals", ViewPastRentals);
            AddButton("Daily Rental Totals", ViewDailyTotals);
            AddButton("Remove Bikes", RemoveBikes);
        }
        else
        {
            //AddHeader("Customer Actions");
            defaultActiveBtn = AddButton("Browse Available Bikes", BrowseAvailableBikes);
            AddButton("View Bike Details", ViewBikeDetails);
            AddButton("Rent a Bike", RentBike);
            AddButton("Calculate Rental Fee", CalculateFee);
            AddButton("Checkout", Checkout);
            AddButton("Return a Bike", ReturnBike);
            AddButton("My Rentals", MyRentals);
        }

        // Add in reverse order so DockStyle.Top stacks top-to-bottom
        navItems.Reverse();
        foreach (var item in navItems)
        {
            navList.Controls.Add(item);
        }

        sidebar.Controls.Add(navList);
        sidebar.Controls.Add(userCard);
        sidebar.Controls.Add(btnLogout);

        // Main Content Area framing
        var mainContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Background,
            Padding = new Padding(16, 16, 16, 12)
        };

        var gridCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Card,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(1)
        };

        // Setup Main DataGridView and StatusStrip
        _dgv.Dock = DockStyle.Fill;
        _dgv.ReadOnly = true;
        _dgv.AllowUserToAddRows = false;
        _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Theme.StyleGrid(_dgv);

        gridCard.Controls.Add(_dgv);

        _lblStatus.Text = "Ready";
        _lblStatus.Font = Theme.Default;
        _lblStatus.ForeColor = Theme.TextColor;
        _status.Items.Add(_lblStatus);
        _status.BackColor = Theme.Card;
        _status.Dock = DockStyle.Bottom;
        _status.SizingGrip = false;
        _status.Padding = new Padding(6, 4, 6, 4);

        mainContent.Controls.Add(gridCard);
        mainContent.Controls.Add(_status);

        Body.Controls.Add(mainContent);
        Body.Controls.Add(sidebar);

        Shown += (_, _) =>
        {
            if (defaultActiveBtn != null) SetActiveNav(defaultActiveBtn);
            if (user.Role == "admin") ShowAllBikes();
            else BrowseAvailableBikes();
        };
    }

    private void ShowResult(OperationResult result)
    {
        MessageBox.Show(this, result.Message, result.Success ? "Success" : "Failed",
            MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        _lblStatus.Text = result.Message;
    }

    private void ShowBikes(List<TblBike> bikes)
    {
        _dgv.DataSource = bikes
            .Select(b => new BikeRow(b.BikeId, b.Name, b.Type, b.PricePerHour, BikeRentService.AvailabilityLabel(b), b.Condition))
            .ToList();
    }

    private void ShowRentals(List<TblRental> rentals)
    {
        _dgv.DataSource = rentals
            .Select(r => new RentalRow(
                r.RentalId,
                r.User?.Name ?? r.UserId.ToString(),
                r.Bike?.Name ?? r.BikeId.ToString(),
                r.RentDatetime,
                r.ExpectedReturn,
                r.ActualReturn,
                r.LateFee,
                r.TotalPrice))
            .ToList();
    }

    // ---------- Customer actions ----------

    private void BrowseAvailableBikes()
    {
        var bikes = _service.GetAvailableBikes();
        if (bikes.Count == 0)
            MessageBox.Show(this, "No bikes are available right now.", "Browse", MessageBoxButtons.OK, MessageBoxIcon.Information);
        ShowBikes(bikes);
        _lblStatus.Text = $"{bikes.Count} available bike(s)";
    }

    private void ViewBikeDetails()
    {
        var values = InputDialog.Ask("View Bike Details", ("Bike ID", ""));
        if (values is null || !int.TryParse(values[0], out var id)) return;

        var bike = _service.GetBike(id);
        if (bike is null)
        {
            MessageBox.Show(this, "Bike not found.", "Details", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _dgv.DataSource = new List<BikeRow> { new(bike.BikeId, bike.Name, bike.Type, bike.PricePerHour, BikeRentService.AvailabilityLabel(bike), bike.Condition) };
        _lblStatus.Text = $"Showing details for bike #{id}";
    }

    private void RentBike()
    {
        var available = _service.GetAvailableBikes();
        if (available.Count == 0)
        {
            MessageBox.Show(this, "No bikes are available right now.", "Rent a Bike", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ShowBikes(available);
        _lblStatus.Text = "Select a bike from the list, then enter ID and hours.";

        var values = InputDialog.Ask("Rent a Bike", ("Bike ID", ""), ("Duration(hr)", ""));
        if (values is null) return;
        if (!int.TryParse(values[0], out var bikeId) || !int.TryParse(values[1], out var hours) || hours <= 0)
        {
            MessageBox.Show(this, "Invalid bike ID or duration.", "Rent a Bike", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var bike = _service.GetBike(bikeId);
        if (bike is null || bike.Status != "available")
        {
            MessageBox.Show(this, "Bike not found or not available.", "Rent a Bike", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var fee = _service.CalculateFee(bike, hours);
        var confirm = MessageBox.Show(this,
            $"Bike: {bike.Name} ({bike.Type})\nPrice/hr: {bike.PricePerHour:N0}\nDuration: {hours} hour(s)\n\nTotal (hourly rate): {fee:N0}\n\nConfirm rental?",
            "Rental Summary", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        var result = _service.RentBike(_user.UserId, bikeId, hours);
        ShowResult(result);
        ShowBikes(_service.GetAvailableBikes());
    }

    private void CalculateFee()
    {
        var values = InputDialog.Ask("Calculate Rental Fee", ("Bike ID", ""), ("Hours", "1"));
        if (values is null) return;
        if (!int.TryParse(values[0], out var id) || !int.TryParse(values[1], out var hours) || hours <= 0) return;

        var bike = _service.GetBike(id);
        if (bike is null)
        {
            MessageBox.Show(this, "Bike not found.", "Calculate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var fee = _service.CalculateFee(bike, hours);
        MessageBox.Show(this,
            $"Bike: {bike.Name} ({bike.Type})\nPrice/hr: {bike.PricePerHour:N0}\nDuration: {hours} hour(s)\nTotal: {fee:N0}",
            "Rental Fee", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void Checkout()
    {
        var rentals = _service.GetActiveRentals(_user.UserId);
        if (rentals.Count == 0)
        {
            MessageBox.Show(this, "You have no active rentals to check out.", "Checkout", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ShowRentals(rentals);
        _lblStatus.Text = "Select the rental to complete payment.";

        var values = InputDialog.Ask("Checkout - Rental ID", ("Rental ID", ""));
        if (values is null || !int.TryParse(values[0], out var id)) return;

        var rental = rentals.FirstOrDefault(r => r.RentalId == id);
        if (rental is null)
        {
            MessageBox.Show(this, "Invalid rental ID.", "Checkout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(this,
            $"Rental ID: {rental.RentalId}\nBike: {rental.Bike?.Name ?? "?"}\nRented: {rental.RentDatetime:yyyy-MM-dd HH:mm}\nExpected back: {rental.ExpectedReturn:yyyy-MM-dd HH:mm}\nTotal price: {rental.TotalPrice:N0}\n\nConfirm payment and complete checkout?",
            "Checkout Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        MessageBox.Show(this, $"Checkout complete. Amount paid: {rental.TotalPrice:N0}", "Checkout", MessageBoxButtons.OK, MessageBoxIcon.Information);
        ShowRentals(_service.GetActiveRentals(_user.UserId));
        _lblStatus.Text = $"Checkout complete for rental #{rental.RentalId}";
    }

    private void ReturnBike()
    {
        var rentals = _service.GetActiveRentals(_user.UserId);
        if (rentals.Count == 0)
        {
            MessageBox.Show(this, "You have no active rentals to return.", "Return a Bike", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ShowRentals(rentals);
        _lblStatus.Text = "Select the rental to return.";

        var values = InputDialog.Ask("Return a Bike - Rental ID", ("Rental ID", ""));
        if (values is null || !int.TryParse(values[0], out var id)) return;

        var rental = rentals.FirstOrDefault(r => r.RentalId == id);
        if (rental is null)
        {
            MessageBox.Show(this, "Invalid rental ID.", "Return a Bike", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(this,
            $"Bike: {rental.Bike?.Name ?? "?"}\nExpected back: {rental.ExpectedReturn:yyyy-MM-dd HH:mm}\nNow: {DateTime.Now:yyyy-MM-dd HH:mm}\n\nConfirm return?",
            "Return a Bike", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        var result = _service.ReturnBike(id);
        ShowResult(result);
        ShowRentals(_service.GetRentalsByUser(_user.UserId));
    }

    private void MyRentals()
    {
        var rentals = _service.GetRentalsByUser(_user.UserId);
        if (rentals.Count == 0)
            MessageBox.Show(this, "You have no rentals yet.", "My Rentals", MessageBoxButtons.OK, MessageBoxIcon.Information);
        ShowRentals(rentals);
        _lblStatus.Text = $"{rentals.Count} rental(s)";
    }

    // ---------- Admin actions ----------

    private void ShowAllBikes()
    {
        ShowBikes(_service.GetAllBikes());
        _lblStatus.Text = $"All bikes ({_service.GetAllBikes().Count})";
    }

    private void AddBike()
    {
        var values = InputDialog.Ask("Add New Bike", ("Name", ""), ("Type", ""), ("Price per hour", ""));
        if (values is null) return;
        if (!decimal.TryParse(values[2], out var price) || price <= 0)
        {
            MessageBox.Show(this, "Enter a valid price per hour.", "Add Bike", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = _service.AddBike(values[0], values[1], price);
        ShowResult(result);
        ShowBikes(_service.GetAllBikes());
    }

    private void EditBike()
    {
        var bikes = _service.GetAllBikes();
        if (bikes.Count == 0)
        {
            MessageBox.Show(this, "No bikes found.", "Update Bike", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ShowBikes(bikes);
        _lblStatus.Text = "Enter the bike ID to update.";

        var values = InputDialog.Ask("Update Bike - ID", ("Bike ID", ""));
        if (values is null || !int.TryParse(values[0], out var id)) return;

        var bike = bikes.FirstOrDefault(b => b.BikeId == id);
        if (bike is null)
        {
            MessageBox.Show(this, "Bike not found.", "Update Bike", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var edits = InputDialog.Ask("Update Bike",
            ("Status", /*bike.Status*/ null),
            ("Price per hour", /*bike.PricePerHour.ToString()*/ null),
            ("Condition", /*bike.Condition*/ null));
        if (edits is null) return;

        string? newStatus = string.IsNullOrWhiteSpace(edits[0]) ? null : edits[0].Trim();
        decimal? newPrice = decimal.TryParse(edits[1], out var p) && p > 0 ? p : null;
        string? newCondition = string.IsNullOrWhiteSpace(edits[2]) ? null : edits[2].Trim();

        var result = _service.UpdateBike(id, newStatus, newPrice, newCondition);
        ShowResult(result);
        ShowBikes(_service.GetAllBikes());
    }

    private void ViewPastRentals()
    {
        var rentals = _service.GetPastRentals();
        if (rentals.Count == 0)
            MessageBox.Show(this, "No past (completed) rentals found.", "Past Rentals", MessageBoxButtons.OK, MessageBoxIcon.Information);
        ShowRentals(rentals);
        _lblStatus.Text = $"{rentals.Count} past rental(s)";
    }

    private void ViewDailyTotals()
    {
        var totals = _service.GetDailyTotals();
        if (totals.Count == 0)
        {
            MessageBox.Show(this, "No rentals found.", "Daily Totals", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _dgv.DataSource = totals.Select(t => new DailyTotalRow(t.Date, t.Total)).ToList();
        _lblStatus.Text = $"Grand total: {totals.Sum(t => t.Total):N0}";
    }

    private void RemoveBikes()
    {
        var values = InputDialog.Ask("Remove Bikes by Condition", ("Condition", ""));
        if (values is null || string.IsNullOrWhiteSpace(values[0])) return;

        var condition = values[0].Trim().ToLower();
        var matches = _service.GetAllBikes().Where(b => b.Condition == condition).ToList();
        if (matches.Count == 0)
        {
            MessageBox.Show(this, $"No bikes found with condition '{condition}'.", "Remove Bikes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ShowBikes(matches);
        var confirm = MessageBox.Show(this,
            $"Remove these {matches.Count} bike(s) with condition '{condition}' (and their rental records)?",
            "Remove Bikes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        var result = _service.RemoveBikesByCondition(condition);
        ShowResult(result);
        ShowBikes(_service.GetAllBikes());
    }
}
