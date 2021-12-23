
var createState = function() {
    return "SessionValuewwerwerwerfdsfdsfwerwerwerwerwer";
}

var createNonce = function() {
    return "NonceValuewergrthryhrhjprgjormgovrfngoefoenfouwenfiunrwiugn";
}

var signIn = function () {
    var redirectUri = "https://localhost:44342/signin";
    var responseType = "id_token token";
    var authUrl = "/connect/authorize/callback" +
"?client_id=client_id_js" +
"&redirect_uri=" + encodeURIComponent(redirectUri) +
"&response_type=" + encodeURIComponent(responseType) +
"&scope="+ encodeURIComponent("openid ApiOne") +
"&nonce=" + createNonce() +
"&state=" + createState()
;
    console.log(authUrl);
    var returnUrl = encodeURIComponent(authUrl);
    console.log(returnUrl);

    window.location.href = "https://localhost:44325/Auth/Login?returnUrl=" + returnUrl;
}