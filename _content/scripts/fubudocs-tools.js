$(function () {
    var template = $('#request-diagnostics-template');
    var msg = template.html();
    var title = template.data('title');

    toastr.options.timeOut = 0;
    toastr.options.positionClass = 'toast-bottom-right';
    toastr.info(msg, title);
});