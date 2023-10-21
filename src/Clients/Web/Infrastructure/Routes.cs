namespace Spenses.Client.Web.Infrastructure;

public static class Routes
{
    public static class Homes
    {
        public static string Dashboard(Guid homeId) => $"/homes/{homeId}/dashboard";
        public static string Expenses(Guid homeId) => $"/homes/{homeId}/expenses";
        public static string Insights(Guid homeId) => $"/homes/{homeId}/insights";
        public static string Settings(Guid homeId) => $"/homes/{homeId}/settings";
    }
}
