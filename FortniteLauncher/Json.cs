using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FortniteLauncher
{
    public partial class Token
    {
        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("messageVars")]
        public List<object> MessageVars { get; set; }

        [JsonPropertyName("numericErrorCode")]
        public int NumericErrorCode { get; set; }

        [JsonPropertyName("originatingService")]
        public string OriginatingService { get; set; }

        [JsonPropertyName("intent")]
        public string Intent { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("refresh_expires")]
        public int RefreshExpires { get; set; }

        [JsonPropertyName("refresh_expires_at")]
        public DateTime RefreshExpiresAt { get; set; }

        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("internal_client")]
        public bool InternalClient { get; set; }

        [JsonPropertyName("client_service")]
        public string ClientService { get; set; }

        [JsonPropertyName("scope")]
        public List<object> Scope { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("app")]
        public string App { get; set; }

        [JsonPropertyName("in_app_id")]
        public string InAppId { get; set; }
    }

    public partial class Exchange
    {
        [JsonPropertyName("expiresInSeconds")]
        public int ExpiresInSeconds { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("creatingClientId")]
        public string CreatingClientId { get; set; }
    }
}
