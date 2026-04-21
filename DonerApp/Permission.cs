namespace DonerApp
{
    public static class Permission
    {
        public static bool CanAccess(string module)
        {
            string role = Session.CurrentEmployee?.Role ?? "";

            return module switch
            {
                "Tables" => role is "manager" or "cashier" or "waiter",
                "Orders" => role is "manager" or "cashier" or "waiter",
                "Stock" => role is "manager" or "kitchen",
                "Products" => role is "manager",
                "Reports" => role is "manager" or "cashier",
                "Employees" => role is "manager",
                "Suppliers" => role is "manager",
                "Settings" => true, // Herkes kendi şifresini değiştirebilir
                _ => false
            };
        }

        public static void Deny(string module)
        {
            string role = Session.CurrentEmployee?.Role ?? "unknown";
            MessageBox.Show(
                $"Access denied.\n\nYour role ({role}) does not have permission to access {module}.",
                "Access Denied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}