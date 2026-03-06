namespace ShoppingOnline.Services.EmailTemplates
{
    public static class FeedbackEmailTemplate
    {
        public static string Build(string customerEmail, string message)
        {
            return $@"
            <h2>New Customer Feedback</h2>

            <p>You have received a new feedback from a customer.</p>

            <p><b>Customer Email:</b> {customerEmail}</p>

            <p><b>Message:</b></p>

            <p>{message}</p>

            <br/>

            <p>Please check the admin dashboard for more details.</p>
            ";
        }
    }
}