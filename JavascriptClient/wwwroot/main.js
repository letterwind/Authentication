var config = {
    authority: "https://localhost:44325/",
    client_id: "client_id_js",
    redirect_uri: "https://localhost:44342/signin",
    response_type: "id_token token",
    scope: "openid ApiOne ApiTwo my.scope",
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage })
}

var userManager = new Oidc.UserManager(config);

userManager.getUser().then(user => {
    console.log(user);
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
        console.log(user.expires_in);
        console.log(user.expired);

    } else {
        refreshing = false;
    }
});

var signIn = function() {
    userManager.signinRedirect();
}

var callApi = function () {
    
    axios.get("https://localhost:44351/secret").then(res => {
        console.log(res);
    });
}

var refreshing = false;

axios.interceptors.response.use(function(res) { return res },
    function (error) {
        console.log(error.response);
        if (error.response.status === 401) {
            var axiosConfig = error.response.config;
            console.log(axiosConfig);
            if (!refreshing) {
                console.log("start refresh token");
                refreshing = true;

                return userManager.signinSilent().then(user => {
                    console.log("new token:", user);
                    axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
                    axiosConfig.headers["Authorization"] = "Bearer " + user.access_token;

                    return axios(axiosConfig);
                });

            }
        }

        return Promise.reject(error);
    });



