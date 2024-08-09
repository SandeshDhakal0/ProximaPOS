namespace TheHighInnovation.POS.Web.Models.Response.DashbordRestro
{
    public class DashboardRestroResponse
    {
        public decimal? total_revenue { get; set; }

        public decimal? total_petty_cash { get; set; }

        public decimal? total_sales { get; set; }

        public decimal? petty_cash { get; set; }

        public decimal? bar_sales { get; set; }

        public decimal occupancy_rate { get; set; }

        public decimal? total_inventory_rupees { get; set; }

        public decimal? total_inventory_units {  get; set; }

        public decimal? restro_overview_table_sales { get; set; }

        public decimal? restro_overview_online_sales { get; set; }

        public decimal? order_status_total {  get; set; }

        public decimal? order_status_return {  get; set; }

        public decimal? customer_status_order { get; set; }

        public decimal? customer_status_online {  get; set; }

    }
}
