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
<canvas id="myChart" height="150"></canvas>

<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.5.0/Chart.min.js"></script> 

<script>
    var ctx = document.getElementById("myChart");

    var myChart = new Chart(ctx, {
        type: "line",
        data: {
            labels: [],
            datasets: [{
                    label: '',
                    data: [],
					borderColor: "blue",
					fill: false,         
                },
                {
                    label: '',
                    data: [],
                    borderColor: "purple",
                    fill: false,         
                },
                {
                    label: '',
                    data: [],
                    borderColor: "orange",
                    fill: false,         
                },
                {
                    label: '',
                    data: [],
                    borderColor: "yellow",
                    fill: false,         
                },
                {
                    label: '',
                    data: [],
                    borderColor: "red",
                    fill: false,         
                }
            ]
        },
        options: {
            legend: { display: false },
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
    pm.getData(function (err, value) {
        myChart.data.datasets[0].label = value.labels
        myChart.data.datasets[0].data = value.actualResults;
        myChart.data.datasets[1].label = value.labels
        myChart.data.datasets[1].data = value.forecastedResults;
        myChart.data.datasets[2].label = value.predictedLabels;
        myChart.data.datasets[2].data = value.predictedEstimate;
        myChart.data.datasets[3].label = value.predictedLabels;
        myChart.data.datasets[3].data = value.lowerEstimate;
        myChart.data.datasets[4].label = value.predictedLabels;
        myChart.data.datasets[4].data = value.upperEstimate;
        myChart.data.labels = value.labels;
        myChart.update();
    });
</script>`;

pm.visualizer.set(template, vizData);
