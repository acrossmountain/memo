namespace Memo.Helpers.Remotes
{
    public class RAdminHelper : IRemote
    {
        public RAdminHelper()
        { }

        public static void Connect()
        {
            // connect successfule
            // auto fiil
            AutoFill();
        }

        public static void Disconnect()
        { }

        private static void AutoFill()
        { }
    }
}
