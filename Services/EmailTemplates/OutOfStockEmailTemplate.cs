namespace ShoppingOnline.Services.EmailTemplates
{
    public static class OutOfStockEmailTemplate
    {
        public static string Build(string productName)
        {
            return $@"
            <h2>Product Out Of Stock</h2>

            <p>Hello Admin,</p>

            <p>The product <b>{productName}</b> is currently out of stock.</p>

            <p>Please update the inventory to avoid losing sales.</p>

            <br/>

            <p>ShoppingOnline System</p>
            ";
        }
    }
}