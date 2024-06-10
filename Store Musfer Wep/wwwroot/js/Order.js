var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("Inprocess")) {
        LodeDataTable("Inprocess");
    }
    else {
        if (url.includes("pending")) {
            LodeDataTable("pending");
        }
        else {
            if (url.includes("completed")) {
                LodeDataTable("completed");
            }
            else {
                if (url.includes("approved")) {
                    LodeDataTable("approved");
                }
                else {
                    LodeDataTable("all");
                }
            }
        }
    }
    
});


function LodeDataTable(status) {

    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/Order/GetAll?status='+ status
        },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "25%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id', "render": function (date) {
                    return ` <div class="w-75 btn-group" role="group">
                             <a href="/admin/Order/details?orderId=${date}" class="btn btn-primary mx-2">
                                 <i class="bi bi-pencil-square"></i> 
                              </a>
                           </div>`
                },
                "width": "10%"
            }
        ]
    });
}


