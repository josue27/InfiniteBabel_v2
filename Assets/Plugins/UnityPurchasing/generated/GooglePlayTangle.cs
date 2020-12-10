#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("WvU7p5vbLpK2dBxAT+A+Mp5TnZqOoySGXTfTzRm9UwQE4TwrC9175pPp9n2wda2yW3pAJT2sY0hcYUhA3l1TXGzeXVZe3l1dXMw9VrD6uxNws7RugWflN+d2R3BrDvaBCkObhBU1PY23d5YlhWIPQQTnZ+Iif2yrh3grQFOrTDH55ZlW2tBKscW8WCXrne0+ZofLThR4YVuMLhPl9aPLnWk70Jk8s9e3JgxdjxQyUnQsjeVfbN5dfmxRWlV22hTaq1FdXV1ZXF/KKUuVehDb2dPMmGVOPdL75aLbJv3eY7+K0QldoVT0tmPmjjjjn6k1fj+lzasqBCPvuincOc/ywtOuN7DQa1PZmH4lUgbLcQff0nVP4495vsmHf2eKkh90WV5fXVxd");
        private static int[] order = new int[] { 3,10,8,10,5,6,13,11,8,10,13,12,12,13,14 };
        private static int key = 92;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
