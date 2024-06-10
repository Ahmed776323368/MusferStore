var dataTable;
$(document).ready(function () {
    LodeDataTable();
});


function LodeDataTable() {

    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/Company/GetAll'
        },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'streetAdress', "width": "15%" },
            { data: 'city', "width": "15%" },
            { data: 'state', "width": "10%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id', "render": function (date) {
                    return ` <div class="w-75 btn-group" role="group">
                                                        <a href="/admin/Company/Upsert?id=${date}" class="btn btn-primary mx-2">

                                                    <i class="bi bi-pencil-square"></i> Edit
                                                </a>
                                                                        <a onclick=Delete('/admin/Company/delete?id=${date}') class="btn btn-danger mx-2">

                                                    <i class="bi bi-trash-fill"></i>   Delete
                                                  </a>
                                            </div>`
                },
                "width": "25%"
            }
        ]
    });
}


function Delete(url) {

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'Delete',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }



            });
        }
    });
}

