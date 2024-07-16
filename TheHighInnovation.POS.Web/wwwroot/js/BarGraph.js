window.initializeBarGraph = () => {

    const xValues = ["jan", "Feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"];
    const yValues = [55, 45, 35, 25, 15, 5, 90, 100, 99, 87, 58, 79];
    const barColors = ["#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0", "#ABBFE0",];

    new Chart("myBarGraph", {
        type: "bar",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: barColors,
                data: yValues
            }]
        },
        options: {
            legend: { display: false },
            title: {
                display: true,
                text: "Sales Trends"
            }
        }
    });

}