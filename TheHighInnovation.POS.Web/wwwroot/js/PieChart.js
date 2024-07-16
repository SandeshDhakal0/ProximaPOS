
window.initializePieChart = () => {
    const xValues = ["Money", "My Money", "Your Money",];
    const yValues = [35, 35, 30,];
    const barColors = [
        "#B0C8EF",
        "#C9EAD4",
        "#EAF6ED",

    ];

    new Chart("myPieChart", {
        type: "pie",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: barColors,
                data: yValues
            }]
        },
        options: {
            title: {
                display: true,
                text: "World Wide Wine Production 2018"
            }
        }
    });
}