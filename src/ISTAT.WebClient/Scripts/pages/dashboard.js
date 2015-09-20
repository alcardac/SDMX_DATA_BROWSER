window.onload = function () {
    var usRole = JSON.parse(sessionStorage.user_role);
    if (usRole.RoleId == 1) {
        $("#home-page-setting").css("display","inline");
    }
    var options_area = {
        title: {
            fontFamily: "Arial",
            fontSize: 18,
            text: "Email Analysis"
        },
        animationEnabled: true,
        axisX: {
            interval: 3
            // labelAngle : 30,
            // valueFormatString: "HHmm'hrs'"

        },
        axisY: {
            title: "Number of Messages"
        },
        legend: {
            verticalAlign: "bottom",
            horizontalAlign: "center"
        },

        data: [{
            name: "received",
            showInLegend: true,
            legendMarkerType: "square",
            type: "area",
            color: "rgba(40,175,101,0.6)",
            markerSize: 0,

            dataPoints: [
            { x: new Date(2013, 0, 1, 00, 00), label: "midnight", y: 7 },
            { x: new Date(2013, 0, 1, 01, 00), y: 8 },
            { x: new Date(2013, 0, 1, 02, 00), y: 5 },
            { x: new Date(2013, 0, 1, 03, 00), y: 7 },
            { x: new Date(2013, 0, 1, 04, 00), y: 6 },
            { x: new Date(2013, 0, 1, 05, 00), y: 8 },
            { x: new Date(2013, 0, 1, 06, 00), y: 12 },
            { x: new Date(2013, 0, 1, 07, 00), y: 24 },
            { x: new Date(2013, 0, 1, 08, 00), y: 36 },
            { x: new Date(2013, 0, 1, 09, 00), y: 35 },
            { x: new Date(2013, 0, 1, 10, 00), y: 37 },
            { x: new Date(2013, 0, 1, 11, 00), y: 29 },
            { x: new Date(2013, 0, 1, 12, 00), y: 34, label: "noon" },
            { x: new Date(2013, 0, 1, 13, 00), y: 38 },
            { x: new Date(2013, 0, 1, 14, 00), y: 23 },
            { x: new Date(2013, 0, 1, 15, 00), y: 31 },
            { x: new Date(2013, 0, 1, 16, 00), y: 34 },
            { x: new Date(2013, 0, 1, 17, 00), y: 29 },
            { x: new Date(2013, 0, 1, 18, 00), y: 14 },
            { x: new Date(2013, 0, 1, 19, 00), y: 12 },
            { x: new Date(2013, 0, 1, 20, 00), y: 10 },
            { x: new Date(2013, 0, 1, 21, 00), y: 8 },
            { x: new Date(2013, 0, 1, 22, 00), y: 13 },
            { x: new Date(2013, 0, 1, 23, 00), y: 11 }
            ]
        },
        {
            name: "sent",
            showInLegend: true,
            legendMarkerType: "square",
            type: "area",
            color: "rgba(0,75,141,0.7)",
            markerSize: 0,
            label: "",
            dataPoints: [

            { x: new Date(2013, 0, 1, 00, 00), label: "midnight", y: 12 },
            { x: new Date(2013, 0, 1, 01, 00), y: 10 },
            { x: new Date(2013, 0, 1, 02, 00), y: 3 },
            { x: new Date(2013, 0, 1, 03, 00), y: 5 },
            { x: new Date(2013, 0, 1, 04, 00), y: 2 },
            { x: new Date(2013, 0, 1, 05, 00), y: 1 },
            { x: new Date(2013, 0, 1, 06, 00), y: 3 },
            { x: new Date(2013, 0, 1, 07, 00), y: 6 },
            { x: new Date(2013, 0, 1, 08, 00), y: 14 },
            { x: new Date(2013, 0, 1, 09, 00), y: 15 },
            { x: new Date(2013, 0, 1, 10, 00), y: 21 },
            { x: new Date(2013, 0, 1, 11, 00), y: 24 },
            { x: new Date(2013, 0, 1, 12, 00), y: 28, label: "noon" },
            { x: new Date(2013, 0, 1, 13, 00), y: 26 },
            { x: new Date(2013, 0, 1, 14, 00), y: 17 },
            { x: new Date(2013, 0, 1, 15, 00), y: 23 },
            { x: new Date(2013, 0, 1, 16, 00), y: 28 },
            { x: new Date(2013, 0, 1, 17, 00), y: 22 },
            { x: new Date(2013, 0, 1, 18, 00), y: 10 },
            { x: new Date(2013, 0, 1, 19, 00), y: 9 },
            { x: new Date(2013, 0, 1, 20, 00), y: 6 },
            { x: new Date(2013, 0, 1, 21, 00), y: 4 },
            { x: new Date(2013, 0, 1, 22, 00), y: 12 },
            { x: new Date(2013, 0, 1, 23, 00), y: 14 }
            ]
        }
        ]
    };

    var options_bar = {
        title: {
            fontFamily: "Arial",
            fontSize: 18,
            text: "Column Chart using jQuery Plugin"
        },
        animationEnabled: true,
        data: [
        {
            type: "column", //change it to line, area, bar, pie, etc
            dataPoints: [
                { x: 10, y: 10 },
                { x: 20, y: 11 },
                { x: 30, y: 14 },
                { x: 40, y: 16 },
                { x: 50, y: 19 },
                { x: 60, y: 15 },
                { x: 70, y: 12 },
                { x: 80, y: 10 }
            ]
        }
        ]
    };

    var options_pie = {
        title: {
            fontFamily: "Arial",
            fontSize: 18,
            text: "Top Categoires of New Year's Resolution"
        },
        exportFileName: "Pie Chart",
        animationEnabled: true,
        legend: {
            verticalAlign: "bottom",
            horizontalAlign: "center"
        },
        data: [
        {
            type: "pie",
            showInLegend: true,
            toolTipContent: "{legendText}: <strong>{y}%</strong>",
            indexLabel: "{label} {y}%",
            dataPoints: [
                { y: 35, legendText: "Health", exploded: true, label: "Health" },
                { y: 20, legendText: "Finance", label: "Finance" },
                { y: 18, legendText: "Career", label: "Career" },
                { y: 15, legendText: "Education", label: "Education" },
                { y: 5, legendText: "Family", label: "Family" },
                { y: 7, legendText: "Real Estate", label: "Real Estate" }
            ]
        }
        ]
    };

    $("#D1-chart-left").CanvasJSChart(options_area);
    $("#D1-chart-left").CanvasJSChart().render();
    $("#D1-chart-right").CanvasJSChart(options_bar);
    $("#D1-chart-right").CanvasJSChart().render();
    $("#D2-chart-left").CanvasJSChart(options_pie);
    $("#D2-chart-left").CanvasJSChart().render();

    $('.Maincontainer').resize(function () {
        $("#D1-chart-left").CanvasJSChart().render();
        $("#D1-chart-right").CanvasJSChart().render();
        $("#D2-chart-left").CanvasJSChart().render();
    });

    $("#D2-Container-right").show("blind", { "direction": "down" });

}