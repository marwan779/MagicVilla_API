namespace MagicVilla_Utility
{
    public static class StaticData
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public static string AccessToken = "JWTToken";
        public static string VillaAPIVersion = "v2";
        public static string AdminRole = "admin";
        public static string CustomerRole = "customer";

        public enum ContentType
        {
            Json,
            MultipartFormData
        }

    }
}
