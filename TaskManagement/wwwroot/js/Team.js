$(document).ready(function () {

    var relationArray = [];

    function SearchForRelatedCustomer(searchValue, isParent, key) {
        const url=`/Team/GetMembersById?memberPhone=${searchValue}`
        $.ajax({
            method: "Get",
            url: url,
            dataType: "json",
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        }).done(function (result) {
            console.log(result)
            if (result.error != "" ) {
                toastr.error(result.error)
            } else { 
            $('#MembersData').removeClass('d-none');
            const list = $('#item');
            var option = document.createElement('option')
            option.value = result.data.memberMobile;
            option.text = result.data.membersName
                list.append(option);
            }

        }).fail(function () {
            toastr.error("Something wrong!");
        });
    }

    $('#MemberNationalId').change(function () {
        $('#item').empty();
        var newValue = $(this).val();
        console.log(newValue);
        SearchForRelatedCustomer(newValue)

    })
    $('#addButton').click(function () {

        const selectedOptions = $('#item').find('option:selected');
        selectedOptions.each(function () {
            const memberNameValue = $(this).text();
            const memberMobileValue = $(this).val();
            if (relationArray.length !== 0) {
                let exists = false;
                relationArray.forEach((e) => {
                    if (e.CustomerNationalId === nationalId) {
                        exists = true;
                    }
                });

                if (exists) {
                    toastr.error("Customer already exists!");
                } else {
                    relationArray.push({
                        MemberName: memberNameValue,
                        MemberMobile: memberMobileValue
                    });
                }
            } else {
                relationArray.push({
                    MemberName: memberNameValue,
                    MemberMobile: memberMobileValue
                });
            }
        });

        displayRelations();
        $('#item').empty();
    });
    function drawTable() {
        const headers = ['MemberName', 'MemberMobile', 'Action'];

        const row = $('<tr class="fw-bold fs-6 text-gray-800"></tr>');

        headers.forEach(headerText => {
            row.append($('<th></th>').text(headerText));
        });

        return row;
    }
    function displayRelations() {
        $('#selectedMembers').empty();
        const actionText = "Remove";
        var table = $('#itemList');
        table.empty();
        const $headerRow = drawTable();
        table.append($headerRow);
        if (relationArray && relationArray.length > 0) {
            relationArray.forEach((relation, index) => {
                const $row = $('<tr></tr>').append(`  
                <td>${relation.MemberName}</td>  
                <td>${relation.MemberMobile}</td>  
                 <td>
                    <a href="javascript:;" class="js-remove-listItem" data-index="${index}">${actionText}</a>  
                </td>  
            `);
                table.append($row);
                $('#selectedMembers').append(`
                        <input type="hidden" id="teamMembers[${index}].MembersName"
                               name="teamMembers[${index}].MembersName"
                               value="${relation.MemberName}">
                        <input type="hidden" id="teamMembers[${index}].MemberMobile"
                               name="teamMembers[${index}].MemberMobile"
                               value="${relation.MemberMobile}">
                               `);
            });
        } else {
           table.append(`<tr><td colspan="3">No data found</td></tr>`)

        };
        $('#membersArrayDisplay').append(table);
    }
    // Remove relation on link click  
    $(document).on('click', '.js-remove-listItem', function () {
        const index = $(this).data('index');
        relationArray.splice(index, 1);
        displayRelations();
    });
    function ResetData() {
        $('#item').empty();
        $('#CustomerNationalId').val('')
        $('#selectedMembers').find('input').val('');
        relationArray = [];
        displayRelations();
    }
})
