#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("O3H2HmtX4YpO/MqxXSe7fP167k2lxsS1B4SntYiDjK8DzQNyiISEhOIKjTGlck4pqaXq9TO6hLUJMsZKuKPipQ+273KIB0pbbiaqfNbv3uGwt7Sxtbaz35KItrC1t7W8t7SxtYO1ioOG0JiWhIR6gYC1hoSEerWY/7UHhPO1i4OG0JiKhIR6gYGGh4Sl5Ovhpebg9/Hs4+zm5PHs6uul9fLyq+T19engq+bq6Krk9fXp4ObkEBv/iSHCDt5Rk7K2TkGKyEuR7FSTtZGDhtCBhpaIxPX16eCl1+rq8TS13WnfgbcJ7TYKmFvg9nri2+A5trPftee0jrWMg4bQgYOWh9DWtJa1lIOG0IGPlo/E9fXp4KXM6+artIGDlofQ1rSWtZSDhtCBj5aPxPX1Mp44Fsehl69CipgzyBnb5k3OBZL16eCl1+rq8aXGxLWbkoi1s7Wxt/yl5Pb28Ojg9qXk5ubg9fHk6+bg3CKAjPmSxdOUm/FWMg6mvsImUOqCafi8Bg7WpVa9QTQ6H8qP7nqueQ6cDFt8zulwgi6ntYdtnbt91YxWihi4dq7MrZ9Ne0swPItc25lTTrgtWfunsE+gUFyKU+5RJ6GmlHIkKZoABgCeHLjCsncsHsULqVE0FZdd9+Tm8ezm4KX28eTx4Ojg6/H2q7WIg4yvA80DcoiEhICAhYYHhISF2dfg6ezk6+bgperrpfHt7Pal5uD3gIWGB4SKhbUHhI+HB4SEhWEULIwHhIWDjK8DzQNy5uGAhLUEd7Wvg0yc93DYi1D62h53oIY/0ArI2Ih0sxzJqP0yaAkeWXbyHnfzV/K1ykRcs/pEAtBcIhw8t8d+XVD0G/sk1+ngpczr5qu0o7Whg4bQgY6WmMT16+Gl5urr4ezx7Orr9qXq46Xw9uChZ25UMvVaisBkok906P1oYjCSkkXmtvJyv4Kp025fiqSLXz/2nMowrwPNA3KIhISAgIW157SOtYyDhtCNroOEgICCh4STm+3x8fX2v6qq8rUHgT61B4YmJYaHhIeHhIe1iIOMq8UjcsLI+o3btZqDhtCYpoGdtZOjtaGDhtCBjpaYxPX16eClxuD38TC/KHGKi4UXjjSkk6vxULmIXueT8e3q9+zx/LSTtZGDhtCBhpaIxPWqtQRGg42ug4SAgIKHh7UEM58ENi4m9BfC1tBEKqrENn1+ZvVIYybJmhRem8LVboBo2/wBqG6zJ9LJ0Gml6uOl8e3gpfHt4Oul5PX16ezm5Ozj7Obk8ezq66XE8PHt6vfs8fy08ezj7Obk8eCl5/yl5Ov8pfXk9/EFka5V7MIR84x7ce4Iq8UjcsLI+vrELR18VE/jGaHulFUmPmGer0aawPuaye7VE8QMQfHnjpUGxAK2DwSppebg9/Hs4+zm5PHgpfXq6ezm/PXp4KXG4Pfx7OPs5uTx7OrrpcTwCvYE5UOe3oyqFzd9wc115b0bkHCDhtCYi4GTgZGuVezCEfOMe3HuCI3btQeElIOG0JilgQeEjbUHhIG1zF3zGraR4CTyEUyoh4aEhYQmB4Tn6eCl9vHk6+Hk9+Gl8eD36Pal5OGwppDOkNyYNhFycxkbStU/RN3V1S8PUF9heVWMgrI18PCk");
        private static int[] order = new int[] { 50,13,21,56,7,29,28,45,36,18,12,26,36,18,21,59,28,40,52,51,28,22,54,49,46,47,38,54,34,51,48,58,56,43,53,57,52,57,50,58,52,49,51,57,46,59,49,50,58,56,52,52,56,59,55,58,57,58,58,59,60 };
        private static int key = 133;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
