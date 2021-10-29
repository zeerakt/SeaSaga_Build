// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Xg9nL8zs9venfB3FUcrRWAmjIdPqAUTHgl2x6sRP7mxzSIyRCMFFAxeHTgfeHMDEccigUY92uO/o07ZrMHbpMEciBDQ5QSotSfDx5e6dd4a7J9affFEQiNFfuXeW5poTZQeFUCkQkrUOti42W/HKFR4a1jVkWXvmXuKtxVJ5KpzpSY4R+mBmBqTGJq2uHJ+8rpOYl7QY1hhpk5+fn5uenfrdZTxbmT2VrhrAdXl01JD52rC6Bax464TgtRIIlx34thA+1VjlUY3GAZhZuGgiYPNBQJQl+5WGNVja6hMoT3pdBjm+hn/Go+J3U1RVJnl9HJ+Rnq4cn5ScHJ+fniWkXLvZ1ZbRUSxxKG5vGGru0CYu7b4OPB4Amtq1wjMyshplG5ydn56f");
        private static int[] order = new int[] { 7,11,10,12,13,5,9,7,13,10,12,12,12,13,14 };
        private static int key = 158;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
