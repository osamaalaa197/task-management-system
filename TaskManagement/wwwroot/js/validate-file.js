var imagesOnlyRegex = /(\.jpg|\.png|\.JPG|\.PNG|\.jpeg|\.JPEG)$/;
var filesRegex = /(\.jpg|\.png|\.JPG|\.PNG|\.jpeg|\.JPEG|\.pdf|\.PDF|\.msg|\.MSG|\.7z|\.7Z|\.zip|\.ZIP|\.rar|\.RAR|\.doc|\.DOC|\.docx|\.DOCX|\.xls|\.XLS|\.xlsx|\.XLSX|\.ppt|\.PPT|\.pptx|\.PPTX)$/;

function validate() {
    var hasNoErrors = true;

    $('body .js-validate-file').each(function (i, input) {
        var file = $(input);

        var allowedTypes = file.data('allowed-ex');
        var allowedExtensions = allowedTypes == 'images' ? imagesOnlyRegex : filesRegex;

        if (file.val() != '') {
            var error = allowedTypes == 'images' ? file.parents('.js-input-container').siblings('.field-validation-valid') : file.siblings('.js-error');
            var checkFile = file.val().toLowerCase();

            if (!checkFile.match(allowedExtensions)) {
                file.focus();
                error.text('Not allowed extension!');
                hasNoErrors = false;
                return hasNoErrors;
            }

            var inputId = file.attr('id');
            var fileInput = document.querySelector(`#${inputId}`);
            var maxAllowedSize = parseInt(file.data('max-size'));

            if (fileInput.files[0].size > maxAllowedSize * 1024 * 1024) {
                error.text(`File cannot be more than ${maxAllowedSize} MB!`);
                hasNoErrors = false;
                return hasNoErrors;
            }

            error.text('');
        }
    });

    return hasNoErrors;
}

$(document).ready(function () {
    $('body').on('click', ':submit', function (e) {
        e.preventDefault();

        if (validate()) $('form').submit();
    });
});