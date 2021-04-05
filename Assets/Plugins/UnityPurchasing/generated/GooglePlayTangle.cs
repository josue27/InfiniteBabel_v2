#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("UeNgQ1FsZ2hL5ynnlmxgYGBkYWLWoNADW7r2cylFXGaxEy7YyJ72oPcUdqhHLebk7vGlWHMA78bYn+Ybs54Zu2AK7vAkgG45OdwBFjbgRttDApjwlhc5HtKHFOEE8s//7pMKjU2OiVO8WtgK2kt6TVYzy7w3fqa5wONegrfsNGCcacmLXtuzBd6ilAhnyAaapuYTr4tJIX1y3QMPo26gp67Uy0CNSJCPZkd9GACRXnVhXHV9KAgAsIpKqxi4XzJ8Odpa3x9CUZbjYG5hUeNga2PjYGBh8QBrjceGLlQG7aQBjuqKGzFgsikPb0kRsNhi7VZu5KVDGG879kw64u9Ict6yRIO6RRZ9bpZxDMTYpGvn7XeM+IFlGPS6Qlq3ryJJZGNiYGFg");
        private static int[] order = new int[] { 0,11,4,10,7,5,12,10,8,12,11,11,12,13,14 };
        private static int key = 97;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
