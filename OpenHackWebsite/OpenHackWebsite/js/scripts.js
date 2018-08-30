var serverIP = "137.117.88.118";
function getServerStatus(ip) {

    MinecraftAPI.getServerStatus(serverIP, { port: 25565 },

        function (err, status) {
            if (err) {
                $("#txtServerStatus").html('Connection Failed!');
                return;
            }

            // you can change these to your own message!

            $("#txtServerStatus").html(status.online ? 'Online' : 'Stoped');

            if (status.online) {
                $("#txtUsers").html(status.players.now + "/" + status.players.max);
                $("#txtUltimoUpdate").html(getDateUnixStr(status.last_updated));
                $("#txtUltimoOnline").html(getDateUnixStr(status.last_online));
                gerarGrafico(status.players.now, status.players.max - status.players.now);
            }
        });
}

function getDateUnixStr(unixStr) {
    var date = new Date(unixStr * 1000);
    var hours = date.getHours();
    var minutes = "0" + date.getMinutes();
    var seconds = "0" + date.getSeconds();
    return hours + ":" + minutes + ":" + seconds
}

function gerarGrafico(logados, disponivel) {
    var ctx = document.getElementById("myChart");

    data = {
        datasets: [{
            data: [logados, disponivel],
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)'
            ],
            borderColor: [
                'rgba(255,99,132,1)',
                'rgba(54, 162, 235, 1)'
            ],
            borderWidth: 1
        }],

        // These labels appear in the legend and in the tooltips when hovering different arcs
        labels: [
            'Online',
            'Slots available'
        ]
    };

    var myPieChart = new Chart(ctx, {
        type: 'pie',
        data: data,
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
}

function showPopUpNewInstace() {
    $("#newInstancePopUp").toggle();
}

function closePopUpNewInstance() {
    $("#txtInstanceName").val('');
    $("#newInstancePopUp").toggle();
}

function showGraphicPopUp(ip) {
    $("#graphicPopUp").toggle();
    getServerStatus(ip);
}

function closeGraphicPopUp() {
    $("#txtInstanceName").val('');
    $("#graphicPopUp").toggle();
}

function addInstance() {
    var name = $("#txtInstanceName").val();

    loadServerList();
    closePopUpNewInstance();
}

function loadServerList() {
    var x = "";
    for (i = 0; i <= 5; i++) {

        x += "<tr>";
        x += "<td class='fillTable'>tenant" + i + "</td>";
        x += "<td class='ipTable'>" + i + "</td>";
        x += "<td class='ipTable'>" + i + "</td>";
        x += "<td class='icoTable' title='Status'><i class='icoStatus' onclick='showGraphicPopUp(" + i + ")' ></i></td>";
        x += "<td class='icoTable' title='Remove'><i class='icoDelete' onclick='deleteInstance(" + i + ")'></i></td>";
        x += "</tr>";
    }
    $("#lstServers").html(x);
}

function deleteInstance(id) {
    alert("deleted: " + id);

    loadServerList();
}

