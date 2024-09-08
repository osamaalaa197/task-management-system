$(document).ready(function () {
    var connectionToHub = new signalR.HubConnectionBuilder()
        .withUrl("/hub/SendMessageHub")
        .build();
    connectionToHub.start().then(function () {
        console.log("SignalR connected Message");
    }).catch(function (err) {
        return console.error(err.toString());
    });
    const $assignToUserRadio = $('#assignToUserRadio');
    const $assignToTeamRadio = $('#assignToTeamRadio');
    const $userDropdownContainer = $('#userDropdownContainer');
    const $teamDropdownContainer = $('#teamDropdownContainer');
    const $memberDropdownContainer = $('#memberDropdownContainer');

    function toggleDropdowns() {
        if ($assignToUserRadio.is(':checked')) {
            $userDropdownContainer.removeClass('d-none');
            $teamDropdownContainer.addClass('d-none');
            $memberDropdownContainer.addClass('d-none')
        } else if ($assignToTeamRadio.is(':checked')) {
            $teamDropdownContainer.removeClass('d-none');
            $memberDropdownContainer.removeClass('d-none')
            $userDropdownContainer.addClass('d-none');
        } else {
            $userDropdownContainer.addClass('d-none');
            $teamDropdownContainer.addClass('d-none');
            $memberDropdownContainer.addClass('d-none')

        }
    }

    $assignToUserRadio.on('change', toggleDropdowns);
    $assignToTeamRadio.on('change', toggleDropdowns);
    toggleDropdowns();

            connectionToHub.on("SendMessage", function (message) {
                console.log(message)
                alert("New task assigned: " + message);
            });

});
