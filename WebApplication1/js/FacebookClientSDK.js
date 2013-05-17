var FBClient = function (settings, window, onready) {

    var settings = settings || {};
    var appAccessToken = "https://graph.facebook.com/oauth/access_token?client_id=" + settings.appId + "&client_secret=58673b8c967065b799b7b9e95aef899f&grant_type=client_credentials";
    
    this.user = {};
    this.baseUri = "http://localhost:14636/api/";
    //"http://nicolasbotto.cloudapp.net/api/";;
    this.appToken = null;

    execPost = function (uri, callback, data, method) {
        method = method || "POST";
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open(method, uri, true);
        xmlHttp.setRequestHeader("Content-Type", "application/json");
        var client = this;
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState === 4) {
                if (xmlHttp.status >= 200 && xmlHttp.status < 300) {
                    var result = {};

                    try {
                        result = JSON.parse(xmlHttp.responseText);
                    }
                    catch (e) {
                        // swallow it
                    }

                    if (callback) {
                        callback.call(client, result);
                    }
                }
            }
        };

        xmlHttp.onerror = function (e,s) {
            var x = e.description;
            return;
        };

        xmlHttp.send(data);
    },

    setAppToken = function (r) {
        this.appToken = r;
    },

    execUri = function (uri, callback) {
        var xmlHttp = new XMLHttpRequest();
        // check from CORS enabled
        if ("withCredentials" in xmlHttp) {
            xmlHttp.open("GET", uri, true);
            var client = this;
            xmlHttp.onreadystatechange = function () {
                if (xmlHttp.readyState === 4) {
                    if (xmlHttp.status >= 200 && xmlHttp.status < 300) {
                        var result = {};

                        try {
                            result = JSON.parse(xmlHttp.responseText);
                        }
                        catch (e) {
                            // try get access token
                            var prefix = "access_token=";

                            result = xmlHttp.responseText.substr(prefix.length);
                        }

                        if (callback) {
                            callback.call(client, result);
                        }
                    }
                }
            };

            xmlHttp.send();
        }
    };

    // Load the SDK Asynchronously
    (function (d) {
        var fbRoot = d.createElement("div");
        fbRoot.id = "fb-root";
        document.body.appendChild(fbRoot);

        var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
        if (d.getElementById(id)) { return; }
        js = d.createElement('script'); js.id = id; js.async = true;
        js.src = "//connect.facebook.net/en_US/all.js";
        ref.parentNode.insertBefore(js, ref);
    }(document));

    var me = this;

    window.fbAsyncInit = function () {
        // init the FB JS SDK
        FB.init({
            appId: settings.appId, // App ID from the App Dashboard
            status: true, // check the login status upon init?
            cookie: true, // set sessions cookies to allow your server to access the session?
            xfbml: true  // parse XFBML tags on this page?
        });

        FB.getLoginStatus(function (response) {
            if (response.status === 'connected') {
                execUri.call(me, appAccessToken, setAppToken);
                // connected
                if (onready) {
                    loadMe(response.authResponse, onready);
                }
            }
            else if (response.status === 'not_authorized') {
                // not_authorized
                FBClient.prototype.login(onready);
            } else {
                // not_logged_in
                FBClient.prototype.login(onready);
            }
        });

       
    };

};

FBClient.prototype = {
    login: doLogin,
    getFriends: function (f) {
        doGetFriends.call(this, f);
    },
    readWall: doReadWall,
    logout: function (f) {
  
        FB.logout.call(this, function (response) {
            // user is now logged out
            this.client.user = {};

            if (f) {
                f();
            }
        });
    },
    getPlayers: function (f) {
        // Check user is logged in
        var uri = this.baseUri + "player";

        execUri.call(this, uri, function (response) {
            if (f) {
                f(response);
            }
        });
    },
    getMatches: function (f) {
        // Check user is logged in
        var uri = this.baseUri + "match";

        execUri.call(this, uri, function (response) {
            if (f) {
                f(response);
            }
        });
    },
    addMatch: function (data, f) {
        var uri = this.baseUri + "match";

        execPost.call(this, uri, function (response) {
            // add event to user

            if (f) {
                f();
            }
        },
        data)
    },
    addPlayer: function (data, f) {
        var uri = this.baseUri + "player";

        execPost.call(this, uri, function (response) {
            if (f) {
                f(response);
            }
        },
        data)
    },
    addMatchPlayer: function (id, data, f) {
        var uri = this.baseUri + "match?id="+id;

        execPost.call(this, uri, function (response) {
            // add event to user

            if (f) {
                f();
            }
        },
        data, "PUT");
    },
    sendRequest: function (id, message, f) {
        var uri = "https://graph.facebook.com/apprequests?message=" + message + "&ids=" + id + "&access_token=" + this.appToken + "&method=post";
        execUri.call(this, uri, function (response) {
            if (f) {
                f(response);
            }
        })
    },
    getRequests: function (f) {
        FB.api('me/apprequests', 'GET',
            function (r) {
                if (f) {
                    f(r.data);
                }
            });
    },
    deleteRequest: function (requestId, f) {
        FB.api(requestId, 'delete', function (response) {
            if (f) {
                f(response);
            }
        });
    }
}

function doReadWall(f) {
    FB.api.call(this, 'me?fields=posts', function (response)
    {
        if(f)
        {
            this.client.user.data.wall = response.posts.data;
            f();
        }
    });
}

function doGetFriends(f) {
    FB.api.call(this, '/me?fields=friends.fields(name, id, picture)', function (response) {
        if (f) {
            this.client.user.data.friends = response.friends.data;
            f();
        }
    });
}

function doLogin(loggedIn) {
    FB.login.call(this, function (response) {
        if (response.authResponse) {
            // connected
            if (loggedIn) {
                loadMe.call(this, response.authResponse, loggedIn);
            }
        }
    }, { scope: "read_friendlists, read_stream, publish_stream" });
}

function loadMe(authResponse, loggedIn) {
    //console.log('Welcome!  Fetching your information.... ');
    FB.api.call(this, '/me', function (response) {
        // set the user property
        var user = { data: response, id: authResponse.userID, authToken: authResponse.accessToken };
        this.client.user = user;
        loggedIn();
    });
}