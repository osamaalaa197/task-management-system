// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toastr-bottom-right",
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

function showSuccessMessage() {
    toastr.success("Saved successfully!");
}

function showErrorMessage() {
    toastr.error("Something went wrong!");
}

//Modal ajax forms
function onModalBegin() {
    $('body :submit').not('.js-exclude').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}

function onModalComplete() {
    $('body :submit').not('.js-exclude').removeAttr('disabled').removeAttr('data-kt-indicator');
}

function onModalFailure() {
    showErrorMessage();
}

function onModalSuccess() {
    closeModel();
    showSuccessMessage();
}

//Add new row to datatable
function addRow(row) {
    $('tr').removeClass(tableAnimationClass);
    var newRow = $(row).addClass(tableAnimationClass);
    table.row.add(newRow).draw();
}

//Edit datatable row
function updateRow(row) {
    $('tr').removeClass(tableAnimationClass);
    var newRow = $(row).addClass(tableAnimationClass);
    table.row(updatedRow).remove().draw();
    table.row.add(newRow).draw();
    updatedRow = undefined;
}

//Generate filters
function generateFilters() {
    for (var i = 0; i < filterCols.length; i++) {
        this.api().column(filterCols[i]).every(function () {
            var column = this;
            var select = $('<select class="form-control input-sm js-filter"><option value=""></option></select>')
                .appendTo($(column.footer()).empty())
                .on('change', function () {
                    var val = $.fn.dataTable.util.escapeRegex(
                        $(this).val()
                    );
                    column
                        .search(val ? '^' + val + '$' : '', true, false)
                        .draw();
                });
            column.data().unique().sort().each(function (d, j) {
                select.append("<option value='" + d + "'>" + d + "</option>");
                $('#TableFilter').append("<option value='" + d + "'>" + d + "</option>");
            });
        });
    }

    if (filterCols.length > 0)
        $('#TableFilter').removeClass('d-none');
}



// DataTable features
var columnDefs = [];
var filterCols = [];
var exportedColms = [];
var order = [];
var headers = $('th');


//Check special columns
$.each(headers, function (i) {
    var col = $(this);
    if (col.hasClass('js-no-sort'))
        columnDefs.push({ "targets": i, "orderable": false });
    else
        exportedColms.push(i);

    if (col.hasClass('js-dt-filter'))
        filterCols.push(i);

    if (col.hasClass('js-order'))
        order.push([i, col.data('order-dir')]);
});


//Handle custome export buttons
$('body').delegate('.js-export', 'click', function () {
    $('body .buttons-excel').trigger('click');
});

$('body').delegate('.js-copy', 'click', function () {
    $('body .buttons-copy').trigger('click');
});



//DataTable options
var dataTableOptions = {
    dom: "B<'row d-none'<'col-6'i><'col-6'f>>" + "<'row'<'col-12'tr>>" +
        "<'row'<'col-6'l><'col-6'p>>",
    buttons: [
        {
            extend: 'copy',
            className: 'btn-primary d-none',
            init: function (api, node, config) {
                $(node).removeClass('btn-secondary');
            },
            exportOptions: {
                columns: exportedColms
            }
        },
        {
            extend: 'excel',
            className: 'btn-success d-none',
            init: function (api, node, config) {
                $(node).removeClass('btn-secondary');
            },
            title: 'Data',
            exportOptions: {
                columns: exportedColms
            }
        }
    ],
    initComplete: generateFilters,
    columnDefs: columnDefs,
    scrollX: true,
    order: order
};


$(document).ready(function () {
    $('.js-logout').on('click', function () {
        $(this).parent().submit();
    });
    $('body').on('click', '.js-remove-attachment', function () {
        var parent = $(this).parents('p');
        parent.addClass('d-none');
        parent.find('input').val('');
        parent.siblings('input').removeClass('d-none');
    });
    //Datatables
    if ($(".js-dataTable").length > 0) {
        table = $(".js-dataTable").DataTable(dataTableOptions);
    }

    //Filter Data tables
    $('[data-kt-user-table-filter="search"]').on('keyup', function () {
        var value = $(this).val();
        var searchInput = $('input[type="search"]');
        searchInput.val(value);
        searchInput.trigger('keyup');
    });

    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');
        var url = btn.data('url').split('/');
        var modelSize = btn.data('modal-size');

        if (modelSize !== '')
            $('.modal-dialog').css('max-width', modelSize);

        if (btn.data('scrollable'))
            $('.modal-dialog').addClass('modal-dialog-scrollable');

        if (btn.data('update') !== undefined)
            updatedRow = btn.parents('tr');


        $.ajax({
            url: btn.data('url'),
            success: function (form) {
                modal.find('#ModalLabel').html(btn.data('title'));
                modal.find('.modal-body').html(form);
                if (!btn.data('has-noform') === true) {
                    $('[data-control="select2"]').select2();
                    $('[data-control="select2"]').select2({ dropdownParent: modal });
                    $('[data-bs-toggle="tooltip"]').tooltip();
                    //applyYearCalendar();
                    //applyPastCalendar();

                    $.validator.unobtrusive.parse(modal);
                }

                modal.modal('show');
            },
            error: function (error) {

                showErrorMessage();

            }
        });
    });


    $('body').delegate('select.js-has-child-ddl', 'change', function () {
        var parent = $(this);
        var child = $('#'.concat(parent.data('child')));
        child.empty();
        child.append('<option></option>');

        if (parent.val() !== '') {
            $.ajax({
                url: `${parent.data('url')}${parent.val()}`,
                success: function (items) {
                    console.log(items)
                    $.each(items,
                        function (i, item) {
                            child.append($("<option></option>").attr("value", item.value).text(item.text));
                        });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });

})
