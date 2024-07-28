// Write your JavaScript code.
/*$(function () {
    $('#billing-table').DataTable();
});*/

/*$(document).ready(function () {
    $('#shipmentTable').on('change', '.tblChk', function () {
        debugger;
        if ($('.tblChk:checked').length == $('.tblChk').length) {
            $('#chkAll').prop('checked', true);
        } else {
            $('#chkAll').prop('checked', false);
        }
        getCheckRecords();
    });

    $("#chkAll").change(function () {
        debugger;
        if ($(this).prop('checked')) {
            $('.tblChk').not(this).prop('checked', true);
        } else {
            $('.tblChk').not(this).prop('checked', false);
        }
        getCheckRecords();
    })
});
function getCheckRecords() {
    debugger;
    $(".selectedDiv").html("");
    $('.tblChk:checked').each(function () {
        debugger;
        if ($(this).prop('checked')) {
            console.log($(this).attr("data-id"));
        }
        console.log(this.value);
    });
}*/