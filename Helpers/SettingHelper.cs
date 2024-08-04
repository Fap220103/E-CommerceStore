using ShoppingOnline.Data;

namespace ShoppingOnline.Helpers
{
    public class SettingHelper
    {
        private readonly ApplicationDbContext _context;

        public SettingHelper(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string GetValue(string key)
        {
            var item = _context.SystemSettings.SingleOrDefault(x => x.SettingKey == key);
            if (item != null)
            {
                return item.SettingValue;
            }
            return string.Empty;
        }
    }
}
