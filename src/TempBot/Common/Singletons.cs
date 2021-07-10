namespace TempBot.Common
{
    public static class Singletons
    {
        public static void Inject(DbHelper dbHelper)
        {
            DbHelper = dbHelper;
        }
        
        public static DbHelper DbHelper { get; private set; }
    }
}