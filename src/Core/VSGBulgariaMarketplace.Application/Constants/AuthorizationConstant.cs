namespace VSGBulgariaMarketplace.Application.Constants
{
    public static class AuthorizationConstant
    {
        public const string SWAGGER_GEN_OPEN_API_SECURITY_SCHEMA_DESCRIPTION = "Using the Authorization header with the Bearer scheme.";
        public const string SWAGGER_GEN_OPEN_API_SECURITY_SCHEMA_NAME = "Authorization";
        public const string OPEN_API_SECURITY_SCHEMA_SCHEME_NAME = "bearer";
        public const string OPEN_API_SECURITY_SCHEMA_REFERENCE_ID = "Bearer";
        public const string SWAGGER_GEN_SECURITY_DEFINITION_NAME = "Bearer";
        public const string SWAGGER_GEN_SECURITY_REQUIREMENT_NAME = "Bearer";
        public const string AZURE_AD_CONFIGURATION_CLIENT_ID = "AzureAd:ClientId";
        public const string AZURE_AD_CONFIGURATION_TENANT_ID = "AzureAd:TenantId";
        public const string CONFIGURATION_AZURE_AD_ADMIN_GROUP_ID = "AzureAd:AdminGroupId";
        public const string JWT_BEARER_AUTHORITY_TEMPLATE = "https://login.microsoftonline.com/{0}/v2.0";
        public const string AUTHORIZATION_ADMIN_POLICY_NAME = "Admin";
        public const string GROUPS_CLAIM_TYPE_NAME = "groups";
        public static string PREFERRED_USERNAME_CLAIM_NAME = "preferred_username";

        internal static string EMAIL_ROUTE_DATA_KEY_NAME = "email";
        internal static string EMAIL_FROM_TOKEN_DOES_NOT_MATCH_EMAIL_FROM_ROUTE_PARAMETER_ERROR_MESSAGE = "Email from token doesn't match email from route parameter.";
    }
}
