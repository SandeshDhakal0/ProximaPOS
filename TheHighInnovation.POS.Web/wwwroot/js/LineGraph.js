window.initializeLineGraph = () => {
    const xValues = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    const ctx = document.getElementById('myLineGraph').getContext('2d');

    const redGradient = ctx.createLinearGradient(0, 0, 0, 400);
    redGradient.addColorStop(0, '#FF00004D');
    redGradient.addColorStop(1, '#FF000000');

    const blueGradient = ctx.createLinearGradient(0, 0, 0, 400);
    blueGradient.addColorStop(0, '#0000FF4D');
    blueGradient.addColorStop(1, '#FFFFFF00');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: xValues,
            datasets: [{
                data: [1860, 1140, 1060, 60, 70, 110, 330, 210, 830, 278],
                borderColor: '#FF0000',
                backgroundColor: redGradient,
                fill: true,
                tension: 0.4
            }, {
                data: [300, 700, 2000, 5200, 600, 4400, 900, 1000, 2000, 100],
                borderColor: '#0000FF',
                backgroundColor: blueGradient,
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            plugins: {
                legend: { display: false }
            },
            scales: {
                x: {
                    grid: { display: false }  // Disable vertical grid lines
                },
                y: {
                    grid: { display: true }  // Enable horizontal grid lines
                }
            }
        }
    });
};
