$(document).ready(function () {
    var connectionToHub = new signalR.HubConnectionBuilder()
        .withUrl("/hub/TaskCountHub")
        .build();

    //Admin
    //$.get({
    //    url: '/Dashboard/GetAssignedTaskForTeam',
    //    data: {},
    //    dataType: 'json',
    //    success: function (result) {
    //        console.log(result)
    //        var ctx = document.getElementById('kt_chartjs_3');
    //        var primaryColor = KTUtil.getCssVariableValue('--kt-primary');
    //        var dangerColor = KTUtil.getCssVariableValue('--kt-danger');
    //        var successColor = KTUtil.getCssVariableValue('--kt-success');
    //        var warningColor = KTUtil.getCssVariableValue('--kt-warning');
    //        var infoColor = KTUtil.getCssVariableValue('--kt-info');
    //        var fontFamily = KTUtil.getCssVariableValue('--bs-font-sans-serif');
    //        const labels = ['Task NotStarted', 'Task InProgress', 'Task Completed'];
    //        const data = {
    //            labels: result.teamName,
    //            datasets: [{
    //                label: 'Dataset 1',
    //                data: result.teamTask,
    //                borderWidth: 1,
    //            }]
    //        };
    //        const config = {
    //            type: 'pie',
    //            data: data,
    //            options: {
    //                plugins: {
    //                    title: {
    //                        display: false,
    //                    },
    //                },
    //                responsive: true,
    //            },
    //            defaults: {
    //                global: {
    //                    defaultFont: fontFamily,
    //                },
    //            },
    //        };

    //        var myChart = new Chart(ctx, config);
    //    },
    //});

    $.get({
        url: '/Dashboard/GetAssignedTaskForTeamByStatus',
        data: {},
        dataType: 'json',
        success: function (result) {
            console.log("dasdasd",result)
            var KTJKanbanDemoColor = function () {
                // Private functions
                var exampleColor = function () {
                    var kanban = new jKanban({
                        element: '#kt_docs_jkanban_color',
                        gutter: '0',
                        widthBoard: '250px',
                        boards: [{
                            'id': '_inprocess',
                            'title': 'Not Started',
                            'class': 'primary',
                            'item':
                                result.data.taskNotStarted.map(e => {
                                    return {
                                        'title': '<span class="fw-bold">' + e.title==null?"":e.title + '</span>',
                                        'class': 'light-primary'
                                    };
                                })
                        }, {
                            'id': '_working',
                            'title': 'Working',
                            'class': 'success',
                            'item': result.data.taskInProgress.map(e => {
                                return {
                                    'title': '<span class="fw-bold">' + e.title + '</span>',
                                    'class': 'light-primary'
                                };
                            })
                        }, {
                            'id': '_done',
                            'title': 'Done',
                            'class': 'danger',
                            'item': result.data.taskCompleted.map(e => {
                                return {
                                    'title': '<span class="fw-bold">' + e.title + '</span>',
                                    'class': 'light-primary'
                                };
                            })
                        }
                        ]
                    });
                }

                return {
                    // Public Functions
                    init: function () {
                        exampleColor();
                    }
                };
            }();

            // On document ready
            KTUtil.onDOMContentLoaded(function () {
                KTJKanbanDemoColor.init();
            });
        },
    });

    $.get({
        url: '/Dashboard/GetTasksTeamByPriorityLevel',
        dataType: 'json',
        success: function (result) {
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
        url: '/Dashboard/GetTaskCompletedForMembers',
        dataType: 'json',
        success: function (result) {
            console.log("result", result);
            var ctx = document.getElementById('kt_chartjs_3');
            var primaryColor = KTUtil.getCssVariableValue('--kt-primary');
            var dangerColor = KTUtil.getCssVariableValue('--kt-danger');
            // Define fonts
            var fontFamily = KTUtil.getCssVariableValue('--bs-font-sans-serif');
            // Chart labels
            const labels = result.members.map((e) => e.memberName);
            // Chart data
            const data = {
                labels: labels,
                datasets: [
                    {
                        label: 'Tasks',
                        data: result.members.map((e) => e.completedTaskCount),
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

    //dounts
    $.get({
        url: '/Dashboard/GetCompletedTaskForTeam',
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