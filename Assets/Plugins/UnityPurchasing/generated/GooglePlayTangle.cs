#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Qr3uhZZuifQ8IFyTHxWPdAB5neAVrpYcXbvgl8MOtMIaF7CKJkq8exuYlpmpG5iTmxuYmJkJ+JN1P37W0PD4SHKyU+BAp8qEwSKiJ+e6qW44G6Z6TxTMmGSRMXOmI0v9Jlps8Lv6YAhu78HmKn/sGfwKNwcWa/J1tXZxq0SiIPIis4K1rsszRM+GXkFWLDO4dbBod56/heD4aaaNmaSNhQ/sjlC/1R4cFgldoIv4Fz4gZx7jS2bhQ5jyFgjceJbBwST57s4YviOpG5i7qZSfkLMf0R9ulJiYmJyZmi5YKPujQg6L0b2knknr1iAwZg5YrP4VXPl2EnLjyZhK0feXselIIJqfMP5iXh7rV3Ox2YWKJfv3W5ZYXwxCuqJPV9qxnJuamJmY");
        private static int[] order = new int[] { 13,9,9,6,12,7,7,8,12,10,13,11,13,13,14 };
        private static int key = 153;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
