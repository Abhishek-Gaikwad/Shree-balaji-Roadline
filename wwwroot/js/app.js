$(function () {
    // table initialization
    var dtTable;
    $('.table').DataTable();
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

            if (url.indexOf('Driver') > -1 || url.indexOf('Marching') > -1) {
                showImagePreview();
            }

            if (isTbl) {
                dtTable = $('#table-billing').DataTable({
                    'columnDefs': [{
                        'targets': 0,
                        'checkboxes': {
                            'selectRow': true
                        }
                    }]
                });
            }

            switch (title.toLowerCase().replace("new ", "").replace("update ", "")) {
                case "shipment":
                case "diesel payment":
                    $('.selectpicker').select2({
                        placeholder: "Select"
                    });
                    break;
                case "roles":
                    $('.selectpicker').selectpicker();
                    $(".dropdown-toggle").addClass(" btn-sm");
                    break;
                case "irn & received amount":
                    $('.selectpicker').selectpicker();
                    $(".dropdown-toggle").addClass(" btn-sm");
                    break;
                case "irn":
                    $('.selectpicker').selectpicker();
                    $(".dropdown-toggle").addClass(" btn-sm");
                    break;
                case "marching":
                    $('.selectpicker').selectpicker();
                    $(".dropdown-toggle").addClass(" btn-sm");
                    calculateExpense();
                    break;
                /*case "billing":
                    $('.selectpicker').selectpicker();
                    $(".dropdown-toggle").addClass(" btn-sm");
                    calculateExpense();
                    break;*/
                default:
            }

            let msg = $("#error").val();
            if (typeof msg !== 'undefined') {
                if (msg.length > 0) {
                    warning(msg);
                }
            }
        },
        complete: function () {
            $('#loader').addClass('display-none');
        },
        error: function () {
            $('#loader').addClass('display-none');
        }
    })
}

const showNestedPopup = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        beforeSend: function () {
            $('#pploader').removeClass('display-none')
        },
        success: function (res) {
            $("#extra-model .modal-body").html(res);
            $("#extra-model .modal-title").html(title);
            $("#extra-model").modal({
                show: true,
                keyboard: false,
                backdrop: 'static'
            });
        },
        complete: function () {
            $('#pploader').addClass('display-none');
        },
        error: function () {
            $('#pploader').addClass('display-none');
        }
    })
}

const jQueryAjaxPost = form => {
    try {
        if (confirm("Do You Want To Complete Your Task.. ?")) {
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

                        $("#form-model .modal-body").html("");
                        $("#form-model .modal-title").html("");
                        $("#form-model").modal('hide');

                        $('.table').DataTable();
                    } else {
                        let msg = res.message !== null ? res.message : "Something Went Wrong...!";
                        $("#form-model .modal-body").html(res.html);

                        switch (res.source.toLowerCase().replace("new ", "").replace("update ", "")) {
                            case "shipment":
                                $('.selectpicker').select2({
                                    placeholder: "Select",
                                    allowClear: true
                                });
                                break;
                            case "roles":
                                $('.selectpicker').selectpicker();
                                $(".bs-placeholder").addClass(" btn-sm");
                                break;
                            case "marching":
                                $('.selectpicker').selectpicker();
                                $(".bs-placeholder").addClass(" btn-sm");
                                break;
                            /*case "billing":
                                $('.selectpicker').selectpicker();
                                $(".dropdown-toggle").addClass(" btn-sm");
                                calculateExpense();
                                break;*/
                            default:
                        }
                        error(msg);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        }
    } catch (e) {
        error();
        console.log(e);
    }
    return false;
}

const jQueryIncidencePost = form => {
    try {
        if (confirm("Do You Want To Complete Your Task.. ?")) {
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
                        var msg = "";
                        if (res.message && res.message.includes("Billing/export")) {
                            msg = res.message.substring(res.message.lastIndexOf("/") + 1);
                            msg = "Your Transaction Completed successfully...\r\n Reference No. #" + msg;
                        }
                        success(msg);
                        $("#view-all-transaction").html(res.html);

                        $("#form-model .modal-body").html("");
                        $("#form-model .modal-title").html("");
                        $("#form-model").modal('hide');

                        if (res.message) {
                            window.open(res.message, "_blank");
                        }
                        if (res.message && res.message.includes("Billing/print-invoice-1")) {
                            window.open(res.message.replace('print-invoice-1', 'print-invoice-2'), "_blank");
                        }

                        $('.table').DataTable();
                    } else {
                        let msg = (res.message) ? res.message : "Something Went Wrong...!";
                        $("#form-model .modal-body").html(res.html);

                        switch (res.source.toLowerCase().replace("new ", "").replace("update ", "")) {
                            case "shipment":
                                $('.selectpicker').selectpicker();
                                $(".bs-placeholder").addClass(" btn-sm");
                                break;
                            case "roles":
                                $('.selectpicker').selectpicker();
                                $(".bs-placeholder").addClass(" btn-sm");
                                break;
                            case "marching":
                                $('.selectpicker').selectpicker();
                                $(".bs-placeholder").addClass(" btn-sm");
                                break;
                            default:
                        }
                        error(msg);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        }
    } catch (e) {
        error();
        console.log(e);
    }
    return false;
}

const jQueryBulkPost = form => {
    try {
        if (confirm("Do You Want To Complete Your Task.. ?")) {
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
                        console.log(res);
                        console.log(res.data);
                        $("#view-all-transaction").html(res.html);

                        $("#form-model .modal-body").html("");
                        $("#form-model .modal-title").html("");
                        $("#form-model").modal('hide');

                        $('.table').DataTable();

                        if (res.data !== null) {
                            $("#dvjson").excelexportjs({
                                containerid: "dvjson",
                                datatype: 'json',
                                dataset: res.data,
                                columns: getColumns(res.data)
                            });
                        }
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
            })
        }
    } catch (e) {
        error();
    }
    return false;
}

const jQueryDataPost = form => {
    var url = form.action;
    var referenceNo = parseInt($("#referenceNo").val());
    if (referenceNo > 0) {
        window.open(url + '/' + referenceNo, "_blank");
    }
    return false;
}

const jQueryDataPost1 = form => {
    var url = form.action;
    var referenceNo = parseInt($("#referenceNo").val());
    if (referenceNo > 0) {
        window.open(url + '/' + referenceNo, "_blank");
    }
    return false;
}

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

const printChoice = (id) => {
    swal({
        title: "Which print you want.?",
        text: "choose your printing choice",
        icon: "warning",
        buttons: {
            cancel: "cancel",
            performa: {
                text: 'Performa Invoice',
                value: "performa",
            },
            invoice: {
                text: 'Invoice',
                value: "invoice",
            }
        },
    }).then((res) => {
        if (res != null) {
            var url = `print-invoice-1?id=${id}&type=${res}`;
            window.open(url, "_blank");
            var url = `print-invoice-2?id=${id}&type=${res}`;
            window.open(url, "_blank");
        }
    });
    return false;
}

const getDriverDetails = (ele) => {
    let id = ele.value;
    if (id > 0) {
        try {
            $.ajax({
                type: "POST",
                url: "/Driver/getdriver/" + id,
                /*data: new {id:id},*/
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#pploader').removeClass('display-none')
                },
                success: function (res) {
                    console.log(res)
                    if (res.isValid) {
                        var data = res.result;
                        if (data !== null) {
                            $("#driverId").val(data.driverId);
                            $("#aadharNo").val(data.aadharNo);
                            $("#dlNo").val(data.dlNo);
                            $("#driverName").val(data.driverName);
                            $("#mobileNo").val(data.mobileNo);
                            $("#licenseExpDate").val(data.licenseExpDate);
                            let currentDate = new Date();
                            let licenseExpDate = new Date(data.licenseExpDate);
                            if (licenseExpDate <= currentDate) {
                                // Show a popup message (you can replace this with your own logic)
                                alert("The driver's license has expired; kindly renew it before proceeding.");
                            }

                            $.each(data.aadharDocs, function (index, val) {
                                showImagePreview(index, 'aadharcard', val);
                            });

                            $.each(data.licenseDocs, function (index, val) {
                                showImagePreview(index, 'licensecard', val);
                            });

                            $.each(data.photos, function (index, val) {
                                showImagePreview(index, 'photo', val);
                            });

                            $.each(data.bankDocs, function (index, val) {
                                showImagePreview(index, 'bankdetail', val);
                            });
                        }
                    } else {
                        warning(res.message);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        } catch (e) {
            cosole.log(e);
        }
    }
    return false;
}

const getDealerDetails = (ele) => {
    let id = ele.value;
    if (id !== null | id !== "") {
        try {
            $.ajax({
                type: "POST",
                url: "/Dealer/getdealer/" + id,
                /*data: new {id:id},*/
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#pploader').removeClass('display-none')
                },
                success: function (res) {
                    console.log(res)
                    if (res.isValid) {
                        var data = res.result;
                        if (data !== null) {
                            $("#location").val(data.city);
                        }
                    } else {
                        warning(res.result);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        } catch (e) {
            cosole.log(e);
        }
    }
    return false;
}

const getDestinationDetails = (ele) => {
    let id = ele.value;
    let vcNo = $('#vcNo').val();
    if (id !== null | id !== "") {
        try {
            let quote = ($("#cid").val() !== '3') ? '' : $("#quotationId").val();
            $.ajax({
                type: "POST",
                url: "/Destination/getdestination/" + id,
                /*data: new {id:id},*/
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#pploader').removeClass('display-none')
                },
                success: function (res) {
                    console.log(res)
                    if (res.isValid) {
                        var data = res.result;
                        if (data !== null) {
                            $("#region").val(data.region);
                            $("#trasitDays").val(data.trasitDays);
                            $("#routeCode").val(data.routeCode);

                            let mfgroute = $("#mfgCode").val() + data.routeCode;
                            $("#mfgRoute").val(mfgroute);

                            getFreightDetails(quote, vcNo);
                        }
                    } else {
                        warning(res.result);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        } catch (e) {
            cosole.log(e);
        }
    }
    return false;
}

const getVcDetails = (ele) => {
    let id = ele.value;
    if (id !== null | id !== "") {
        try {
            let quote = ($("#cid").val() !== '3') ? '' : $("#quotationId").val();
            let url = quote === '' ? `/VcMaster/getvc/${id}` : `/VcMaster/getvcbyquote?quoteId=${quote}&id=${id}`;
            $.ajax({
                type: "POST",
                url: url,
                /*data: new {id:id},*/
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#pploader').removeClass('display-none')
                },
                success: function (res) {
                    console.log(res)
                    if (res.isValid) {
                        var data = res.result;
                        if (data !== null) {
                            $("#modelDesc").val(data.modelDesc);
                            $("#mfgCode").val(data.mfgCode);
                            $("#plantDesc").val(data.plantDesc);
                            $("#plantCode").val(data.plantCode);

                            let mfgroute = data.mfgCode + $("#routeCode").val();
                            $("#mfgRoute").val(mfgroute);

                            if (quote !== '') {
                                $("#quotinfo").html(`QUANTITY : ${data.balance}`);
                            } else {
                                $("#quotinfo").html(``);
                            }

                            getFreightDetails(quote, id);
                        }
                    } else {
                        warning(res.result);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        } catch (e) {
            cosole.log(e);
        }
    }
    return false;
}

const getPaymentDetails = (ele) => {
    let id = ele.value;
    if (id !== null | id !== "") {
        try {
            $.ajax({
                type: "POST",
                url: "/Payment/getvoucher/" + id,
                /*data: new {id:id},*/
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#pploader').removeClass('display-none')
                },
                success: function (res) {
                    console.log(res)
                    if (res.isValid) {
                        var data = res.result;
                        if (data !== null) {
                            $("#payableAmount").val(data.balance);
                            $("#balanceAmount").val(data.balance);
                        }
                    } else {
                        warning(res.result);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        } catch (e) {
            cosole.log(e);
        }
    }
    return false;
}

const getShipmentsForQuote = (ele) => {
    let id = ele.value;
    if (id !== null | id !== "") {
        try {
            $.ajax({
                type: "POST",
                url: "/Billing/getibshipment/" + id,
                /*data: new {id:id},*/
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#pploader').removeClass('display-none')
                },
                success: function (res) {
                    if (res.isValid) {
                        if (res.html !== null) {
                            $('#shipmentNos').val(res.message);
                            $('#ib-shipment').html(res.html);
                            $('.table').DataTable();
                        }
                    } else {
                        warning(res.message);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        } catch (e) {
            cosole.log(e);
        }
    }
    return false;
}

const getPumpDetails = (ele) => {
    let id = ele.value;
    if (id !== null | id !== "") {
        try {
            $.ajax({
                type: "POST",
                url: "/PumpMaster/getpump/" + id,
                /*data: new {id:id},*/
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#pploader').removeClass('display-none')
                },
                success: function (res) {
                    console.log(res)
                    if (res.isValid) {
                        var data = res.result;
                        if (data !== null) {
                            $("#spdRate").val(data.rate);
                        }
                    } else {
                        warning(res.result);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        } catch (e) {
            cosole.log(e);
        }
    }
    return false;
}

const getFreightDetails = (quoteId, vcNo) => {
    let mgfCode = $("#mfgCode").val();
    let routeCode = $("#routeCode").val();
    let mfgroute = mgfCode + routeCode;

    if ((mgfCode !== null | mgfCode !== "") && (routeCode !== null | routeCode !== "")) {
        try {
            $.ajax({
                type: "POST",
                url: `/MfgRoute/getfreight?id=${mfgroute}&quoteId=${quoteId}&vcNo=${vcNo}`,
                /*data: new {id:id},*/
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#pploader').removeClass('display-none')
                },
                success: function (res) {
                    console.log(res)
                    if (res.isValid) {
                        var data = res.result;
                        if (data !== null) {
                            $("#basicFreight").val(data.basicFreight);
                            $("#enRoute").val(data.enRoute);
                            $("#totalFreight").val(data.totalFreight);
                        }
                    } else {
                        warning(res.result);
                    }
                },
                complete: function () {
                    $('#pploader').addClass('display-none');
                },
                error: function () {
                    $('#pploader').addClass('display-none');
                }
            })
        } catch (e) {
            cosole.log(e);
        }
    }
    return false;
}

const showPreview = (e) => {
    var files = e.files;
    filesLength = files.length;
    if (filesLength > 0) {
        for (var i = 0; i < filesLength; i++) {
            var f = files[i]
            imgData = readImageData(i, f, e.id);
        }
    }
    return false;
}

const readImageData = (i, file, container) => {
    const reader = new FileReader();
    reader.addEventListener("load", () => {
        showImagePreview(i, container, reader.result);
    }, false);

    if (file) {
        reader.readAsDataURL(file);
    }
}

const showImagePreview = (i, container, data) => {
    if (typeof container !== 'undefined') {
        var html = $('#' + container + '-preview').html();
        var count = (html.match(/href/g) || []).length;
        i = html.length == 0 ? 1 : (i + count + 1);
        html += '<a class="image-link" data-lightbox="' + container + '" data-title="' + container + ' ' + i + '" href="' + data + '">';
        if (i === 1) {
            html += '<i class="fa fa-eye"></i></a>';
        }
        html += '</a>';

        $('#' + container + '-preview').html(html);
    }
}

const checkRemark = (ele) => {
    let status = ele.value;

    $("#reason-block").css('display', 'none');
    if (status.toLowerCase() === "others") {
        $("#reason-block").css('display', 'block');
    }

    if (status != null & status != "") {
        $("#status").val(status);
        $("#status").data("status").value(status)
    } else {
        status = $("#status").data("status");
        $("#status").val(status);
    }

    return false;
}

const calculateHsd = () => {
    let totalHsd = $("#totalHsd").val();
    let spdQty = $("#spdQty").val();
    let spdRate = $("#spdRate").val();
    spdRate = parseFloat(spdRate);
    let spdAmount = spdQty * spdRate;
    $("#spdAmount").val(spdAmount.toFixed(2));

    let remainHsd = parseFloat(totalHsd) - parseFloat(spdQty);

    let rate = $("#remainhsd-prefix").data("rate");
    $("#remainhsd-prefix").html("" + remainHsd.toFixed(2) + " * " + rate);

    let total = (remainHsd.toFixed(2) * parseFloat(rate))
    $("#remainhsd").val(total.toFixed(2));

    calculateTotalExpense();

    return false;
}

const calculateExpense = () => {
    let inroute = $("#inRouteExp").val();
    let toll = $("#tollExp").val();
    totalexp = parseFloat(inroute) - parseFloat(toll);
    $("#total-inroot").val(totalexp.toFixed(2));

    calculateTotalExpense();

    return false;
}

const calculateFreight = () => {
    let basicFreight = parseFloat($("#basicFreight").val());
    let enroute = parseFloat($("#enRoute").val());
    totalFreight = (isNaN(basicFreight) ? 0 : basicFreight) + (isNaN(enroute) ? 0 : enroute);
    $("#totalFreight").val(totalFreight.toFixed(2));

    return false;
}

const calculateTotalExpense = () => {

    let inroute = $("#total-inroot").val();
    let remainHsd = $("#remainhsd").val();
    let driverPayment = $("#driverPayment").val();
    let loading = $("#loadingCharge").val();
    let extraAmt = $("#extraAmt").val();

    let total = parseFloat(inroute) + parseFloat(remainHsd) + parseFloat(driverPayment) - parseFloat(loading) + parseFloat(extraAmt);
    $("#totalExp").val(total.toFixed(2));

    return false;
}

const calculateBilling = () => {
    let shipments = new Array();
    let selected_row = dtTable.column(0).checkboxes.selected();

    $.each(selected_row, (key, shipmentId) => {
        shipments.push(shipmentId);
    })

    try {
        $("#shipmentNos").val(shipments.toString());
        $("#billing-form").submit();
    } catch (e) {
        console.log(e);
    }
    return false;
}

const getShipmentNos = (selected_row) => {
    let shipments = new Array();
    $.each(selected_row, (key, shipmentNo) => {
        shipments.push(shipmentNo);
    })
    return shipments;
}

const calculatePayAmount = (ele) => {
    let per = ele.value;
    let payment = 0;
    if (per.length > 0) {
        let total = $("#payableAmount").val();
        let paid = $("#paidAmount").val();
        
        let balance = parseFloat(total) - parseFloat(paid);
        if (paid > total) {
            alert ("PAYMENT AMOUNT MUST BE LESS THAN OR EQUAL TO PAYABLE AMOUNT")
        }

        $("#balanceAmount").val(balance);
    }

}

const getQuoteQty = (ele) => {
    return false;
}

const getDieselVoucherList = (ele) => {
    try {
        // diesel payment form submission
        const userInfo = document.querySelector('#diesel-payment-form');
        const formData = new FormData(userInfo);

        $.ajax({
            type: "POST",
            url: userInfo.action,
            data: formData,
            contentType: false,
            processData: false,
            beforeSend: function () {
                $('#pploader').removeClass('display-none')
            },
            success: function (res) {
                if (res.isValid) {
                    try {
                        $("#dieselVoucher").html('');
                        dtTable = new DataTable('#table-diesel-vouchers');
                        dtTable.clear().destroy()
                    } catch (ex) { }
                    finally {

                        $("#dieselVoucher").html(res.html);
                        dtTable = $('#table-diesel-vouchers').DataTable({
                            'columnDefs': [{
                                'targets': 0,
                                'checkboxes': {
                                    'selectRow': true
                                }
                            }],
                        });
                    }
                    $('#pploader').addClass('display-none');
                } else {
                    error();
                    $('#pploader').addClass('display-none');
                }
            }
        })
    } catch (e) {
        error();
        $('#pploader').addClass('display-none');
    }
    return false;
}

const calculatePumpBilling = (ele) => {
    try {

    let shipments = new Array();
    let selected_row = dtTable.column(0).checkboxes.selected();

    $.each(selected_row, (key, shipmentId) => {
        shipments.push(shipmentId);
    });

        var formData = {
            vouchers: shipments,
            pumpId: $('#pumpId').val()
        }

        $.ajax({
            type: "POST",
            url: '/Payment/getDieselTotalByIds',
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            processData: false,
            beforeSend: function () {
                $('#pploader').removeClass('display-none')
            },
            success: function (res) {
                if (res.isValid) {
                    let data = res.data;
                    console.log(res.data);
                    $('input#pumpId').val(formData.pumpId);
                    $('#vouchers').val(shipments);
                    $('#balanceAmount').val(data.balanceAmount);
                    $('#currentAmount').val(data.currentAmount);
                    $('#paidAmount').val(data.paidAmount);
                    $('#payableAmount').val(data.payableAmount);
                    $('#prevBalance').val(data.prevBalance);
                    $('#spdAmount').val(data.spdAmount);
                    $('#spdQty').val(data.spdQty);
                    $('#spdRate').val(data.spdRate);

                    $('#pploader').addClass('display-none');
                    $(".btn-pump").toggleClass("hidden");
                } else {
                    error();
                    $('#pploader').addClass('display-none');
                }
            }
        })
    } catch (e) {
        error();
        $('#pploader').addClass('display-none');
    }

    return false;
}

const updateInvoiceDate = (ele) => {
    let dt = ele.value;
    $("#invoiceDate").val(dt);
    return false;
}

const checkIsNewDriver = (ele) => {
    $("#driverName").css('display', 'none');
    $("#driverId").css('display', 'inline');
    $(".dropdown").css('display', 'block');
    $("#lbl-diver").text("Driver");
    if (ele.checked == true) {
        $("#driverName").css('display', 'inline');
        $("#driverId").css('display', 'none');
        $(".dropdown").css('display', 'none');
        $("#lbl-diver").text("Driver Name");
    }
    return false;
}

const toggleQuote = (ele) => {
    let cid = $("#cid").val();
    let quote = (cid !== '3') ? '' : $("#quotationId").val();
    try {
        if (cid !== '3') {
            $("#quoteblock").hide();
            $("#quotinfo").css('display', 'none');
        } else {
            $("#quoteblock").show();
            $("#quotinfo").css('display', 'block');
        }

        $.ajax({
            type: "POST",
            url: `/Shipment/getVcList?quoteId=${quote}`,
            //data: { id: cid, quoteId: quote },
            contentType: false,
            processData: false,
            beforeSend: function () {
                $('#pploader').removeClass('display-none')
            },
            success: function (res) {
                console.log(res)
                if (res.isValid) {

                    $('select#vcNo').empty();
                    $.each(res.source, function (key, value) {
                        $('select#vcNo').append(`<option value="${value.id}" data-tokens="${value.id}">$${value.name}</option>`).trigger("chosen:updated");

                    });

                    $('select#vcNo').select2({
                        placeholder: "Select",
                        allowClear: true
                    });
                }
            },
            complete: function () {
                $('#pploader').addClass('display-none');
            },
            error: function () {
                $('#pploader').addClass('display-none');
            }
        })
    } catch (e) {
        $('#pploader').addClass('display-none');
        cosole.log(e);
    }
    return false;
}

const hideStatue = (ele) => {
    if (ele.id === "ewayNo") {
        let str = ele.value;
        str.replace(" ", "");
        if (str.length > 0) {
            $("#status-block").css('display', 'none');
            $("#reason-block").css('display', 'none');
        } else {
            $("#status-block").css('display', 'block');

            $("#reason-block").css('display', 'none');
            if (status.toLowerCase() === "others") {
                $("#reason-block").css('display', 'block');
            }
        }
    } else if (ele.id === "epodNo") {
        const [file] = ele.files
        if (file) {
            $("#status-block").css('display', 'none');
            $("#reason-block").css('display', 'none');
        } else {
            $("#status-block").css('display', 'block');

            $("#reason-block").css('display', 'none');
            if (status.toLowerCase() === "others") {
                $("#reason-block").css('display', 'block');
            }
        }
    }
    return false;
}

const success = (msg) => {
    msg = !msg ? "Your Transaction Completed successfully.." : msg;
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
    msg = !msg ? "Your Transaction has been failed.." : msg;
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

const choice = () => {
    swal({
        title: "Which print you want.?",
        text: "choose your printing choice",
        icon: "warning",
        buttons: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Performa Invoice',
        cancelButtonText: 'Invoice',
        dangerMode: true,
    }).then((res) => {
        return res;
    });
}

// Diesel Payment form
const calculateBalance = (ele) => {

    let inputValue = ele.value;

    let numericValue = inputValue.replace(/\D/g, '');

    ele.value = numericValue;
    // Get the key code of the pressed key
    document.querySelector('#balanceAmount').value = document.querySelector('#payableAmount').value - numericValue;
    return false;
}