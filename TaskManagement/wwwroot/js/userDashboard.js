
$(document).ready(function () {
    var connectionToHub = new signalR.HubConnectionBuilder()
        .withUrl("/hub/TaskCountHub")
        .build();
    $.get({
        url: '/Dashboard/GetAssignedTask',
        data: {},
        dataType: 'json',
        success: function (result) {
            var ctx = document.getElementById('kt_chartjs_3');
            var primaryColor = KTUtil.getCssVariableValue('--kt-primary');
            var dangerColor = KTUtil.getCssVariableValue('--kt-danger');
            var successColor = KTUtil.getCssVariableValue('--kt-success');
            var warningColor = KTUtil.getCssVariableValue('--kt-warning');
            var infoColor = KTUtil.getCssVariableValue('--kt-info');
            var fontFamily = KTUtil.getCssVariableValue('--bs-font-sans-serif');
            const labels = ['Task NotStarted', 'Task InProgress', 'Task Completed'];
            const data = {
                labels: labels,
                datasets: [{
                    label: 'Dataset 1',
                    data: [result.data.taskNotStarted, result.data.taskInProgress, result.data.taskCompleted],
                    borderWidth: 1,
                }]
            };
            const config = {
                type: 'pie',
                data: data,
                options: {
                    plugins: {
                        title: {
                            display: false,
                        },
                    },
                    responsive: true,
                },
                defaults: {
                    global: {
                        defaultFont: fontFamily,
                    },
                },
            };

            var myChart = new Chart(ctx, config);
        },
    });

    $.get({
        url: '/Dashboard/GetCompletedTask',
        dataType: 'json',
        success: function (result) {
            console.log(result);
            var ctx = document.getElementById('chart-holder').getContext('2d');
            var chart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Total Tasks', 'Completed Tasks'],
                    datasets: [{
                        label: 'Task Completion Rate',
                        data: [result.totalTask, result.completedTasks],
                        backgroundColor: [
                            'rgb(255, 99, 132)',
                            'rgb(54, 162, 235)',
                            'rgb(255, 205, 86)'
                        ],
                        hoverOffset: 4,
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    legend: {
                        display: true,
                        position: 'bottom'
                    },
                    title: {
                        display: true,
                        text: 'Task Completion Rate'
                    }
                }
            });
        }
    });

    $.get({
        url: '/Dashboard/GetTaskByPriorityLevel',
        dataType: 'json',
        success: function (result) {
            console.log("dsadsad",result);
            var ctx = document.getElementById('kt_chartjs_2');
                    var primaryColor = KTUtil.getCssVariableValue('--kt-primary');
                    var dangerColor = KTUtil.getCssVariableValue('--kt-danger');
            // Define fonts
            var fontFamily = KTUtil.getCssVariableValue('--bs-font-sans-serif');
            // Chart labels
            const labels = ['PriorityLevel Low', 'PriorityLevel Medium', 'PriorityLevel High'];

            // Chart data
            const data = {
                labels: labels,
                datasets: [
                    {
                        label: 'Tasks',
                        data: [result.taskLow, result.taskMedium, result.taskHigh],
                        borderColor: primaryColor,  
                        backgroundColor: primaryColor
                    }
                ]
            };

            // Chart config
            const config = {
                type: 'bar',
                data: data,
                options: {
                    plugins: {
                        title: {
                            display: false,
                        }
                    },
                    responsive: true,
                },
                defaults: {
                    global: {
                        defaultFont: fontFamily
                    }
                }
            };

            // Init ChartJS -- for more info, please visit: https://www.chartjs.org/docs/latest/
            var myChart = new Chart(ctx, config);
        }
    });

    $.get({
        url: '/Dashboard/GetTaskByDueDate',
        dataType: 'json',
        success: function (result) {
            console.log(result);
            var ctx = document.getElementById('kt_chartjs_1');
            // Define fonts
            var fontFamily = KTUtil.getCssVariableValue('--bs-font-sans-serif');
            // Chart labels
            const labels = result.dueDates;

            // Chart data
            const data = {
                labels: labels,
                datasets: [
                    {
                        label: 'Tasks',
                        data: result.taskCounts,
                        borderColor: 'rgba(255, 99, 132, 1)',
                        borderWidth: 1
                    }
                ]
            };

            // Chart config
            const config = {
                type: 'line',
                data: data,
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                },
                defaults: {
                    global: {
                        defaultFont: fontFamily
                    }
                }
            };
            var myChart = new Chart(ctx, config);
        }
    });
    $.get({
        url: '/Dashboard/GetAssignments',
        dataType: 'json',
        success: function (assignments) {
            var dueDateCounts = {};
            $.each(assignments, function (index, assignment) {
                var dueDate = moment(assignment.dueDate).format('YYYY-MM-DD');
                if (!dueDateCounts[dueDate]) {
                    dueDateCounts[dueDate] = 0;
                }
                dueDateCounts[dueDate]++;
            });
            const data = {
                labels: Object.keys(dueDateCounts),
                datasets: [
                    {
                        label: 'Tasks',
                        data: Object.values(dueDateCounts),
                        fill: false,
                        tension: 0.1,
                    }
                ]
            };

            // Create the chart
            const config = {
                type: 'line',
                data: data,
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            };
            const ctx = document.getElementById('kt_chartjs_1');
            new Chart(ctx, config);
        }
    });

    connectionToHub.start().then(function () {
        console.log("SignalR connected");
    }).catch(function (err) {
        return console.error(err.toString());
    });
    connectionToHub.on("ReceiveTaskUpdate", function (totalTaskCount) {
        console.log(totalTaskCount)
        document.getElementById("total-tasks-count").innerText = totalTaskCount;
    });

   
})
