namespace ShoppingOnline.Services.EmailTemplates
{
    public static class MonthlyReportEmailTemplate
    {
        public static string Build(int totalProductsSold, decimal totalRevenue)
        {
            return $@"
            <h2>Monthly Revenue Report</h2>

            <p>Hello Admin,</p>

            <p>Here is the sales report for this month:</p>

            <ul>
                <li><b>Total Products Sold:</b> {totalProductsSold}</li>
                <li><b>Total Revenue:</b> ${totalRevenue}</li>
            </ul>

            <br/>

            <p>Please check the admin dashboard for detailed analytics.</p>

            <p>ShoppingOnline System</p>
            ";
        }
    }
}