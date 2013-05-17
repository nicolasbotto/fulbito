<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fulbito de los viernes</title>
     <script src="/js/FacebookClientSDK.js"></script>
    <link href="/css/default.css" rel="stylesheet" />
    <link rel="stylesheet" href="/Content/themes/base/jquery-ui.css" />
    <script src="/Scripts/jquery-1.9.0.js"></script>
    <script src="/Scripts/jquery-ui-1.10.0.js"></script>
    <link rel="stylesheet" href="/resources/demos/style.css" />
    <style>
        body, input {
            font-size: 12px;
            font-family: Tahoma;
        }
        div.ui-datepicker{
            font-size:12px;
            font-family: Tahoma;
        }
    </style>
</head>

<body>
     <input type="button" id="btnLogin" value="Login" onclick="doGetStatus()" style="display: none"/>
     <script>
        var isConnected = false;
        var activePlayer = null;
        var appPlayers = [];
        var matches = [];

        var client = new FBClient({ appId: "142425165908897" }, window, loggedIn);

        function showAppointment() {
            if (isConnected) {
                document.getElementById("createApp").className = "appointmentVisible";
            }
        }

        function createMatch() {
            if (isConnected) {
                // using datacontract
                
                var name = $("#txtName").val();
                var date = $("#datepicker").val();

                if (name.length == 0)
                {
                    alert("Tenes que ingresar un nombre de partido.");
                    $("#txtName").focus();
                    return;
                }

                if (date.length == 0)
                {
                    alert("Tenes que ingresar una fecha para el partido.");
                    $("#datepicker").focus();
                    return;
                }

                //if (date < Date.now) {
                //    alert("Tenes que ingresar una fecha mayor o igual a de hoy.");
                //    $("#datepicker").focus();
                //    return;
                //}

                var data = JSON.stringify({ Name: name, Date: date });
                client.addMatch(data, function (f) {
                    document.getElementById("txtName").value = "";
                    document.getElementById("datepicker").value = "";
                    matches.push(f);
                });
            }
        }

        function addMatchPlayer(id) {
            if (isConnected != null) {
                // using datacontract
                //var data = JSON.stringify({ appointment: { Name: document.getElementById("txtName").value, Description: document.getElementById("txtDescription").value, UserId: FBClient.user.id } });
                var data = JSON.stringify({ FacebookId: activePlayer.FacebookId });
                client.addMatchPlayer(id, data, function (f) {
                    getMatch();
                    doSendRequest(id);
                    $("#statusBar").html("Estas confirmado");
                });
            }
        }

        function addPlayer() {
            if (isConnected) {
                // using datacontract
                //var data = JSON.stringify({ appointment: { Name: document.getElementById("txtName").value, Description: document.getElementById("txtDescription").value, UserId: FBClient.user.id } });
                var data = JSON.stringify({ Name: client.user.data.name, FacebookId: client.user.id });
                client.addPlayer(data, function (f) {
                    activePlayer = f;
                });
            }
        }

        function doGetStatus() {
            if (!isConnected) {
                client.login(loggedIn);
            }
        }

        function loggedIn() {
            $("#btnLogin").hide();
            document.getElementById("greeting").innerHTML = "Bienvenido: " + client.user.data.name;
            isConnected = true;
            
            $("#statusBar").html("Obteniendo partido");

            getPlayers();
            getFriends();
            getRequests();
        }

        function getRequests() {
            client.getRequests(function (r) {
                var notif = "";
                $.each(r, function (i, e) {
                    notif += "<div id='"+ e.id +"'><label>" + e.message + "</label>&nbsp;&nbsp;<input value='Borrar' type='button' onclick='doDeleteRequest(\"" + e.id + "\");'/></div>";
                });

                if (notif.length > 0) {
                    $("#lblNotifications").show();
                    $("#notifications").html(notif);
                }
            });
        }

        function doDeleteRequest(id) {
            $("#" + id).hide();

            client.deleteRequest(id, function (r) {
                if (r === false) {
                    alert('Error al eliminar notificacion, volve a intentarlo');
                    $("#" + id).show();
                }
            });
        }

        function friendsLoaded() {
            var list = "";
            var friends = client.user.data.friends;
            for (var f in friends) {
                var friend = friends[f];
                list += "<div class='friendItem'>";
                list += "<img src='" + friend.picture.data.url + "' />";
                list += "<div>" + friend.name + " </div></div>";
            }

            document.getElementById("selectFriends").innerHTML = list;
        }

        function getFriends() {
            if (isConnected) {
                client.getFriends(friendsLoaded);
            }
        }

        function doConfirmar(id)
        {
            if (confirm("Vas a jugar, no?")) {
                addMatchPlayer(id);
            }
        }

        function getPlayers() {
            client.getPlayers(function (r) {
                $.each(r, function (i, p) {
                    appPlayers.push(p);

                    if (p.FacebookId == client.user.id) {
                        activePlayer = p;
                    }
                })

                if (activePlayer === null) {
                    addPlayer();
                }

                getMatch();
            });
        }

        function getMatch() {
            client.getMatches(function (r) {
                $("#statusBar").html("");
                
                var partidos = "";

                $.each(r, function (i, p) {
                    var match = p;
                    matches.push(p);
                    var content = "<H2>Partido: " + match.Name + "</H2><BR/>";
                    var matchDate = new Date(match.Date);
                    var dateContent = "Fecha: " + matchDate.getDate() + "/" + matchDate.getMonth() + 1 + "/" + matchDate.getFullYear() + " a las " + matchDate.getHours() + ":" + matchDate.getMinutes() + "<BR/><br/>";

                    var playersConfirmed = "<b>Aun no confirmo nadie</b><BR/><BR/>"

                    if (match.Players.length > 0) {
                        playersConfirmed = "Confirmados:<BR/><ul>";

                        var iamPlaying = false;

                        $.each(match.Players, function (index, item) {
                            if (item.FacebookId == activePlayer.FacebookId) {
                                playersConfirmed += "<li><b>" + item.Name + "</b></li>";
                                iamPlaying = true;
                            }
                            else {
                                playersConfirmed += "<li>" + item.Name + "</li>";
                            }
                        });

                        playersConfirmed += "</ul>";
                    }

                    var divContent = content + dateContent + playersConfirmed;

                    if (!iamPlaying) {
                        divContent += "<input type='button' value='Confirmar' onclick='doConfirmar("+ match.Id +")' /><br/><hr/>";
                    }

                    partidos += divContent;
                });

                $("#partido").html(partidos);
            });
        }

        $(function () {
            $("#datepicker").datepicker({
                showOtherMonths: true,
                selectOtherMonths: true
            });
            $("#anim").change(function () {
                $("#datepicker").datepicker("option", "showAnim", $(this).val());
            });
            $("#tabs").tabs();
        });

        function doSendRequest(matchId) {
            var ids = "";

            $.each(appPlayers, function (i, e)
            {
                ids += e.FacebookId + ",";
            });

            ids = ids.substring(0, ids.length - 1);

            var match = null;

            $.each(matches, function (i, e)
            {
                if (e.Id == matchId) {
                    match = e;
                    return;
                }
            });

            var message = activePlayer.Name + " confirmo para el partido: " + match.Name + ".";

            client.sendRequest(ids, message, function ()
            {
                getRequests();
            });
        }
    </script>
    
    <form id="form1" runat="server">
        <div id="greeting"></div>
        <div id="tabs">
          <ul>
            <li><a href="#tabs-1">Partidos</a></li>
            <li><a href="#tabs-2">Armar partido</a></li>
            <li><a href="#tabs-3">Notificaciones</a><label id="lblNotifications" style="font-size:medium; display:none;"><b>!</b></label></li>
          </ul>
          <div id="tabs-1">
            <div id="statusBar"></div>
            <div id="partido"></div>
          </div>
          <div id="tabs-2">
            <label for="txtName">Name: </label>
            <input id="txtName" />
            <p>Date: <input type="text" id="datepicker" /></p>
            <div id="selectFriends"></div>
            <input type="button" value="Crear" onclick="createMatch()" />
          </div>
            <div id="tabs-3">
            <div id="notifications"></div>
          </div>
        </div>
        <input type="button" value="request" onclick="doSendRequest(1)" />
    </form>
</body>
</html>
