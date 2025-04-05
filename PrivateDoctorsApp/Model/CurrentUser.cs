using System.Collections.Generic;

namespace PrivateDoctorsApp.Model
{
    public static class CurrentUser
    {
        public static int? ID {  get; set; }
        public static Dictionary<string, string> Status = new Dictionary<string, string>
        {
            { "pending", "Заплановано"},
            { "cancelled", "Скасовано"},
            { "confirmed", "Завершено"}

        };
    }
}
