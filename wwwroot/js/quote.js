$(function () {
    var table;
    $('.table').DataTable();
});

$('#form-model').on('hidden.bs.modal', function (e) {
    let id = $.trim($("#quoteId").val());
    $.ajax({
        type: "POST",
        url: `Quotation/RemoveCache/${id}`,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $('#pploader').removeClass('display-none')
        },
        success: function (res) {
        },
        complete: function () {
            $('#pploader').addClass('display-none');
        },
        error: function () {
            $('#pploader').addClass('display-none');
        }
    });
});

const showInPopup = (url, title, isTbl = false) => {
    $.ajax({
        type: "GET",
        url: url,
        beforeSend: function () {
            $('#loader').removeClass('display-none')
        },
        success: function (res) {
            //$("#form-model").addClass(cls);
            $("#form-model .modal-body").html(res);
            $("#form-model .modal-title").html(title);
            $("#form-model").modal({
                show: true,
                keyboard: false,
                backdrop: 'static'
            });

            $('.selectpicker').select2({
                placeholder: "Select",
                allowClear: true
            });
        },
        complete: function () {
            $('#loader').addClass('display-none');
        },
        error: function () {
            $('#loader').addClass('display-none');
        }
    })
}

//Add Multiple Order.
$(document).on('click', '#addToList', function (e) {
    e.preventDefault();
    if ($.trim($("#modelDesc").val()) == ""
        || $.trim($("#source").val()) == ""
        || $.trim($("#destination").val()) == ""
        || $.trim($("#totalKms").val()) == ""
        || $.trim($("#qty").val()) == ""
        || $.trim($("#tmlRate").val()) == ""
        || $.trim($("#sblRate").val()) == ""
        || $.trim($("#basicFreight").val()) == ""
        || $.trim($("#enroute").val()) == ""
        || $.trim($("#totalFreight").val()) == ""
    ) {
        warning("Please provide all values");
        return;
    }


    var vcNo = $.trim($("#modelDesc").val()),
        modelDesc = $.trim($('#modelDesc :selected').text()),
        source = $.trim($("#source").val()),
        id = $.trim($("#destination").val()),
        destination = $.trim($('#destination :selected').text()),
        totalKms = $.trim($("#totalKms").val()),
        qty = $.trim($("#qty").val()),
        tmlRate = $.trim($("#tmlRate").val()),
        sblRate = $.trim($("#sblRate").val()),
        basicFreight = $.trim($("#basicFreight").val()),
        enroute = $.trim($("#enroute").val()),
        totalFreight = $.trim($("#totalFreight").val()),
        quoteId = $.trim($("#quoteId").val()),
        detailsTableBody = $("#detailsTable tbody");

    modelDesc = modelDesc.replace(`${vcNo}-`, '');

    const formData = new FormData();

    formData.append("quoteId", quoteId);
    formData.append("modelDesc", vcNo);
    formData.append("source", source);
    formData.append("destination", id);
    formData.append("totalKms", totalKms);
    formData.append("qty", qty);
    formData.append("tmlRate", tmlRate);
    formData.append("sblRate", sblRate);
    formData.append("basicFreight", basicFreight);
    formData.append("enroute", enroute);
    formData.append("totalFreight", totalFreight);

    var url = "Quotation/AddOrEditDetail";
    $.when(saveDetails(url, formData)).then(function (response) {
        if (response.success) {
            var srNo = response.srNo;
            var productItem = `<tr>
                <td  class="count"></td>
                <td>${source}</td>
                <td>${destination}</td>
                <td>${totalKms}</td>
                <td>${qty}</td>
                <td>${modelDesc}</td>
                <td>${tmlRate}</td>
                <td>${sblRate}</td>
                <td>${basicFreight}</td>
                <td>${enroute}</td>
                <td>${totalFreight}</td>
                <td>
                    <a data-itemId="0" data-value="${srNo}" href="javascript:()" class="deleteItem text-danger"><span class="fa fa-trash"></span></a>
                </td>
            </tr>`;
            detailsTableBody.append(productItem);
            clearItem();
            //<a href="javascript:()" id="${srNo}" onclick="return QuoteDetailDelete('${srNo}');" class="text-danger"><span class="fa fa-trash"></span></a>
        }
    }).fail(function (err) {
        console.log(err);
        error(err);
    });
});

//After Add A New Order In The List, Clear Clean The Form For Add More Order.
const clearItem = () => {
    $("#modelDesc").val('');
    $("#source").val('');
    $("#destination").val('-1');
    $("#totalKms").val('');
    $("#qty").val('');
    $("#tmlRate").val('');
    $("#sblRate").val('');
    $("#basicFreight").val('');
    $("#enroute").val('');
    $("#totalFreight").val('');
}
// After Add A New Order In The List, If You Want, You Can Remove It.
$(document).on('click', 'a.deleteItem', function (e) {
    e.preventDefault();
    let id = $(this).attr('data-value');
    var $self = $(this);
    if ($(this).attr('data-itemId') == "0") {
        $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
            $(this).remove();
            deleteQuoteDetail(id);
        });
    }
});
//After Click Save Button Pass All Data View To Controller For Save Database
const saveOrder = (url, data) => {
    return $.ajax({
        type: 'POST',
        contentType: false,
        processData: false,
        url: url,
        data: data,
        beforeSend: function () {
            $('#pploader').removeClass('display-none')
        },
        success: function (res) {

        },
        complete: function () {
            $('#pploader').addClass('display-none');
        },
        error: function () {
            $('#pploader').addClass('display-none');
        }
    });
}

const saveDetails = (url, data) => {
    return $.ajax({
        type: 'POST',
        contentType: false,
        processData: false,
        url: url,
        data: data,
        beforeSend: function () {
            $('#pploader').removeClass('display-none')
        },
        success: function (result) {
        },
        complete: function () {
            $('#pploader').addClass('display-none');
        },
        error: function () {
            $('#pploader').addClass('display-none');
        }
    });
}

const deleteQuoteDetail = (id) => {
    return $.ajax({
        type: "POST",
        url: `Quotation/DeleteDetail/${id}`,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $('#pploader').removeClass('display-none')
        },
        success: function (res) {
        },
        complete: function () {
            $('#pploader').addClass('display-none');
        },
        error: function () {
            $('#pploader').addClass('display-none');
        }
    });
}
//Collect Multiple Order List For Pass To Controller
$(document).on('click', '#saveOrder', function (e) {
    e.preventDefault();
    var cnt = 0;
    $.each($("#detailsTable tbody tr"), function () {
        cnt++;
    });
    const formData = new FormData();

    formData.append("quoteId", $("#quoteId").val());
    formData.append("quoteNo", $("#quoteNo").val());
    formData.append("quoteFor", $("#quoteFor").val());
    formData.append("quoteDate", $("#quoteDate").val());

    if (cnt > 0) {
        let url = $("#quote-form").attr("action");
        $.when(saveDetails(url, formData)).then(function (resp) {
            if (resp.isValid) {
                success();
                $("#view-all-transaction").html(resp.html);

                $("#form-model .modal-body").html("");
                $("#form-model .modal-title").html("");
                $("#form-model").modal('hide');

                $('.table').DataTable();
            } else {
                let msg = resp.message !== null ? resp.message : "Something Went Wrong...!";
                $("#form-model .modal-body").html(resp.html);
                error(msg);
            }
        }).fail(function (err) {
            console.log(err);
            error(err);
        });
    }
});

const jQueryAjaxDelete = form => {
    try {
        swal({
            title: "Are you sure?",
            text: "Once deleted, you will not be able to recover it again!",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                $.ajax({
                    type: "POST",
                    url: form.action,
                    data: new FormData(form),
                    contentType: false,
                    processData: false,
                    beforeSend: function () {
                        $('#pploader').removeClass('display-none')
                    },
                    success: function (res) {
                        if (res.isValid) {
                            success();
                            $("#view-all-transaction").html(res.html);
                            $("#form-model .modal-body").html('');
                            $("#form-model .modal-title").html('');
                            $("#form-model").modal('hide');
                            $('.table').DataTable();
                        } else {
                            let msg = res.message !== null ? res.message : "Something Went Wrong...!";
                            error(msg);
                            $("#form-model .modal-body").html(res.html);
                        }
                    },
                    complete: function () {
                        $('#pploader').addClass('display-none');
                    },
                    error: function () {
                        $('#pploader').addClass('display-none');
                    }
                });
            }
        });
    } catch (e) {
        error();
        cosole.log(e);
    }
    return false;
}

const calculateTotalFreight = () => {
    let basicFreight = parseFloat($("#basicFreight").val());
    let enroute = parseFloat($("#enroute").val());
    totalFreight = (isNaN(basicFreight) ? 0 : basicFreight) + (isNaN(enroute) ? 0 : enroute);
    $("#totalFreight").val(totalFreight.toFixed(2));

    return false;
}

const calculateBasicFreight = () => {
    let totalKms = parseFloat($("#totalKms").val());
    let sblRate = parseFloat($("#sblRate").val());
    basicFreight = (isNaN(totalKms) ? 0 : totalKms) * (isNaN(sblRate) ? 0 : sblRate);
    $("#basicFreight").val(basicFreight.toFixed(2));

    calculateTotalFreight();
    return false;
}

const success = (msg) => {
    msg = (typeof msg === 'undefined' || msg === null) ? "Your Transaction Completed successfully.." : msg;
    swal("Good job!", msg, "success");
    return false;
}

const warning = (msg) => {
    swal("Ohh!", msg, "warning");
    return false;
}

const info = (msg) => {
    swal("Ohh!", msg, "info");
    return false;
}

const error = (msg) => {
    msg = msg === null ? "Your Transaction has been failed.." : msg;
    swal("Oh noes!", msg, "error");
    return false;
}

const confirmDelete = () => {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover it again!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((res) => {
        return res;
    });
}
