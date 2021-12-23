var config = {
    authority: "https://localhost:44325/",
    client_id: "client_id_js",
    redirect_uri: "https://localhost:44342/signin",
    response_type: "id_token token",
    scope:"openid ApiOne"
}

var userManager = new Oidc.UserManager(config);

userManager.getUser().then(user => {
    console.log(user);
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
        axios.get("https://localhost:44351/secret").then(res => {
            console.log(res);
        });
    } else {
        userManager.signinRedirect();
    }
});