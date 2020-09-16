var a = [];
var mass;
google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(DataLoad);
google.charts.setOnLoadCallback(drawChart);
google.charts.setOnLoadCallback(drawChart6);
google.charts.setOnLoadCallback(drawChart3);
google.charts.setOnLoadCallback(drawChart4);
google.charts.setOnLoadCallback(drawChart5);
google.charts.setOnLoadCallback(drawChart7);
google.charts.setOnLoadCallback(drawChart8);
google.charts.setOnLoadCallback(drawChart9);
function DataLoad() {
    a = JSON.parse(document.getElementById("jsondata").innerText);
    metro = JSON.parse(document.getElementById("jsonmetro").innerText);
    a.sort((a, b) => a.price > b.price ? 1 : -1);
 
     mass1 = new Array(a.length);
     mass1[0] = new Array(2);
     mass1[0][0] = 'Номер';
     mass1[0][1] = 'Цена';
     for (var i = 1; i < a.length; i++) {
         mass1[i] = new Array(2);
         mass1[i][0] = i;
         mass1[i][1] = a[i].price;
     }
 
    
     mass2 = new Array(a.length);
     mass2[0] = new Array(2);
     mass2[0][0] = 'Кол-во комнат';
     mass2[0][1] = 'Цена';
     for (var i = 1; i < a.length; i++) {
         mass2[i] = new Array(2);
         mass2[i][0] = a[i].num;
         mass2[i][1] = a[i].price;
     }
 
 
     mass3 = new Array(a.length);
     mass3[0] = new Array(3);
     mass3[0][0] = ' ';
     mass3[0][1] = 'Предсказание';
     mass3[0][2] = 'Цена';
     for (var i = 1; i < a.length; i++) {
         mass3[i] = new Array(3);
         mass3[i][0] = i;
         
         mass3[i][1] = a[i].prediction;
         mass3[i][2] = a[i].price;
     }
     
    metro.sort((a, b) => a.price > b.price ? 1 : -1);
   mass4 = new Array(metro.length);
    mass4[0] = new Array(2);
    mass4[0][0] = 'Метро';
    mass4[0][1] = 'Коэффициент';
    mass5 = new Array(metro.length);
    mass5[0] = new Array(3);
    mass5[0][0] = 'Метро';
    mass5[0][1] = 'Цена';
    mass5[0][2] = 'Кол-во';
    for (var i = 1; i < metro.length; i++) {
        mass4[i] = new Array(2);
        mass4[i][0] = metro[i].metro;
        mass4[i][1] = metro[i].k;

    }
    metro.sort((a, b) => a.num > b.num ? 1 : -1);
    for (var i = 1; i < metro.length; i++) {
        mass5[i] = new Array(3);
        mass5[i][0] = metro[i].metro;
        mass5[i][1] = metro[i].price/30000;
        mass5[i][2] = metro[i].num;
    }

    mass6 = new Array(a.length);
    mass6[0] = new Array(2);
    mass6[0][0] = 'Расстояние';
    mass6[0][1] = 'Цена';
    for (var i = 1; i < a.length; i++) {
        mass6[i] = new Array(2);
        mass6[i][0] = a[i].centre_distance;
        mass6[i][1] = a[i].price;
    }

    mass7 = new Array(31);
    mass7[0] = new Array(2);
    mass7[0][0] = 'Расстояние';
    mass7[0][1] = 'Цена';
    for (var i = 1; i < a.length; i++) {
        var str = "";
        for (var j = 0; a[i].date[j] != ' '; j++)
            str += a[i].date[j];
        var n = Number(str);
        
            if (mass7[n] != null) {

                mass7[n][0]++;
                mass7[n][1] += a[i].price;
            } else {
                mass7[n] = new Array(2);
                mass7[n][0] = 1;
                mass7[n][1] = a[i].price;
            }
        
    }

    for (var i = 1; i <= 31; i++) {
        if (mass7[i] != null) {
            mass7[i][1] /= mass7[i][0];
            mass7[i][0] = i;
        }
    }

    mass8 = new Array(31);
    mass8[0] = new Array(2);
    mass8[0][0] = 'Расстояние';
    mass8[0][1] = 'Цена';
    for (var i = 1; i <= 31; i++) {
        
        mass8[i] = new Array(2);
        if (mass7[i] != null)
            mass8[i][1] = mass7[i][1];
        else
            mass8[i][1] = null;
        mass8[i][0] = (i);
    }

}

function drawChart() {

 
    
    var data = google.visualization.arrayToDataTable(mass1);

    var options = {
        title: 'График цен',
        curveType: 'function',
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.LineChart(document.getElementById('curve_chart'));

    chart.draw(data, options);
}


function drawChart6() {



    var data = google.visualization.arrayToDataTable(mass4);

    var options = {
        title: 'График коэффициентов станций метро',
        legend: { position: 'none' },
    };

    var chart = new google.visualization.Histogram(document.getElementById('pidor'));
    chart.draw(data, options);
}




function drawChart3() {
    var data = google.visualization.arrayToDataTable([
        ['Task', 'Hours per Day'],
        ['Work', 11],
        ['Eat', 2],
        ['Commute', 2],
        ['Watch TV', 2],
        ['Sleep', 7]
    ]);

    var options = {
        title: 'My Daily Activities',
        pieHole: 0.4,
    };

    var chart = new google.visualization.PieChart(document.getElementById('donutchart'));
    chart.draw(data, options);
}

function drawChart4() {
    var data = google.visualization.arrayToDataTable([
        ['Task', 'Hours per Day'],
        [0, 0],
        [5, 0],
        [10, 0.723],
        [15, 0.717],
        [20, 0.712],
        [25, 0.722],
        [30, 0.728],
        [35, 0.72],
        [40, 0.718],
        [45, 0.716]
    ]);

    var options = {
        title: 'График зависимости цены от кол-ва комнат',
        hAxis: { title: 'Age', minValue: 0, maxValue: 15 },
        vAxis: { title: 'Weight', minValue: 0, maxValue: 15 },
        legend: 'none'
    };

    var chart = new google.visualization.ScatterChart(document.getElementById('chart_div'));

    chart.draw(data, options);
}
function drawChart5() {



    var data = google.visualization.arrayToDataTable(mass3);

    var options = {
        title: 'График цены и рыночной стоимости',
        curveType: 'function',
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.LineChart(document.getElementById('curve_chart5'));

    chart.draw(data, options);
}

function drawChart7() {
    var data = google.visualization.arrayToDataTable(mass4);

    var view = new google.visualization.DataView(data);


    var options = {
        title: "Density of Precious Metals, in g/cm^3",
     
        bar: { groupWidth: "95%" },
        legend: { position: "none" },
    };
    var chart = new google.visualization.BarChart(document.getElementById("barchart_values"));
    chart.draw(view, options);
}

function drawChart8() {
    var data = google.visualization.arrayToDataTable(mass5);

    var view = new google.visualization.DataView(data);


    var options = {
        title: "График средней цены и кол-ва предложений",

        bar: { groupWidth: "95%" },
        legend: { position: "none" },
    };
    var chart = new google.visualization.BarChart(document.getElementById("barchart_values2"));
    chart.draw(view, options);
}

function drawChart9() {

    var data = google.visualization.arrayToDataTable(mass8);

    var options = {
        title: 'График зависимости цены от расстояния до центра',
        hAxis: { title: 'Age', minValue: 0, maxValue: 15 },
        vAxis: { title: 'Weight', minValue: 0, maxValue: 15 },
        legend: 'none'
    };

    var chart = new google.visualization.ScatterChart(document.getElementById('curve_chart6'));

    chart.draw(data, options);
}