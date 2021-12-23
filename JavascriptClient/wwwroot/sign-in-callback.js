var extractToken = function(address) {
    
    var hash = location.hash.split('#')[1];
    var params = hash.split('&');
    localStorage.clear();
    for (var i = 0; i < params.length; i++) {
        var hashKVs = params[i].split('=');
        localStorage.setItem(hashKVs[0], hashKVs[1]);
    }

    location.href = "/home/index";
}

extractToken(window.location.href);