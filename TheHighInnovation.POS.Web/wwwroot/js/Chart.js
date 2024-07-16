window.initializeChart = () => {
    const xValues = ["30%", "25%", "20%", "15%", "5%"];
    const yValues = [30, 25, 20, 15, 5];
    const barColors = [
        "#14AD5A",
        "#E72929",
        "#F3C008",
        "#ABBFE0",
        "#ECECEB"
    ];

    new Chart(document.getElementById("myChart"), {
        type: "doughnut",
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
                text: "Revenue Breakdown"
            }
        }
    });
};

