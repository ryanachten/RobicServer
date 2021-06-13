const res = pm.response.json();
const labels = res.actualResults.map((val, index) => index);
const maxIndex = Math.max(...labels);
const predictedLabels = res.predictionResults.forecastedNetValue.map((val, index) => maxIndex + index + 1);
labels.push(...predictedLabels);
const buffer = res.actualResults.map(() => null);
buffer.pop();
const vizData = {
  labels,
  predictedLabels,
  actualResults: res.actualResults,
  forecastedResults: res.forecastedResults,
  predictedEstimate: [...buffer, ...res.predictionResults.forecastedNetValue],
  lowerEstimate: [...buffer, ...res.predictionResults.lowerBoundNetValue],
  upperEstimate: [...buffer, ...res.predictionResults.upperBoundNetValue]
};

var template = `
<canvas id="chart"></canvas>
<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.5.0/Chart.min.js"></script> 
<script>
    var ctx = document.getElementById("chart");

    var myChart = new Chart(ctx, {
        type: "line",
        data: {
            labels: [],
            datasets: [{
                    label: 'Actual',
                    data: [],
					borderColor: "blue",
					fill: false,         
                },
                {
                    label: 'Forecasted',
                    data: [],
                    borderColor: "purple",
                    fill: false,         
                },
                {
                    label: 'Predicted',
                    data: [],
                    borderColor: "orange",
                    fill: false,         
                },
                {
                    label: 'Predicted (Lower band)',
                    data: [],
                    borderColor: "yellow",
                    fill: false,         
                },
                {
                    label: 'Predicted (Upper band)',
                    data: [],
                    borderColor: "red",
                    fill: false,         
                }
            ]
        },
        options: {
            legend: { display: true },
            title: {
                display: true,
                text: ''
            },
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'Session'
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: true,
                        labelString: 'NetValue'
                    }
                }]
            }
        }

    });

    // Access the data passed to pm.visualizer.set() from the JavaScript
    // code of the Visualizer template
    pm.getData((err, value) => {
        myChart.data.datasets[0].data = value.actualResults;
        myChart.data.datasets[1].data = value.forecastedResults;
        myChart.data.datasets[2].data = value.predictedEstimate;
        myChart.data.datasets[3].data = value.lowerEstimate;
        myChart.data.datasets[4].data = value.upperEstimate;
        myChart.data.labels = value.labels;
        myChart.update();
    });
</script>`;

pm.visualizer.set(template, vizData);
