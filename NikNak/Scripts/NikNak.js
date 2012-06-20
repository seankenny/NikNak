var chart;
$(function () {

    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'chart-container',
            zoomType: 'x',
            spacingRight: 20
        },
        title: {
            text: 'BG Levels from 2010 through 2012'
        },
        subtitle: {
            text: document.ontouchstart === undefined ?
				'Click and drag in the plot area to zoom in' :
				'Drag your finger over the plot to zoom in'
        },
        xAxis: {
            type: 'datetime',
            maxZoom: 14 * 24 * 3600000, // fourteen days
            title: {
                text: null
            }
        },
        yAxis: {
            title: {
                text: 'BG Level (mmol/l)'
            },
            min: 2.0,
            startOnTick: false,
            showFirstLabel: false
        },
        tooltip: {
            shared: true
        },
        legend: {
            enabled: false
        },
        plotOptions: {
            area: {
                fillColor: {
                    linearGradient: [0, 0, 0, 300],
                    stops: [
						[0, Highcharts.getOptions().colors[0]],
						[1, 'rgba(2,0,0,0)']
                    ]
                },
                lineWidth: 1,
                marker: {
                    enabled: false,
                    states: {
                        hover: {
                            enabled: true,
                            radius: 5
                        }
                    }
                },
                shadow: false,
                states: {
                    hover: {
                        lineWidth: 1
                    }
                }
            }
        },

        series: [{
            type: 'area',
            name: 'BG Level (mmol/l)',
            pointInterval: 24 * 3600 * 1000,
            pointStart: Date.UTC(2010, 0, 01),
            data: getChartDataFromServer()
        }]
    });

    $('#change-chart-type').click(function() {
        var series = chart.series[0],
            newType = series.type == 'area' ? 'line' : 'area';
        changeType(series, newType);
    });

    function changeType(series, newType) {
        
        series.chart.addSeries({
            type: newType,
            name: 'BG Level (mmol/l)',
            pointInterval: 24 * 3600 * 1000,
            pointStart: Date.UTC(2010, 0, 01),
            data: series.options.data
        }, false);

        series.remove();
    }
    
    function getChartDataFromServer() {
        var data;
        $.ajax(
            {
                url: 'Data/GetChartData',
                async: false,
                success: function(response) {
                    //var data = '[' + response + ']';
                    data = response.split(',');
                    for (a in data) {
                        data[a] = parseFloat(data[a]);
                    }
                }
            });

        return data;
    }
});