var dataTable;
$(document).ready(function () {
    LodeDataTable();
});


function LodeDataTable() {

    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/User/GetAll'
        },
        "columns": [
            { "data": 'name', "width": "%15" },
            { "data": 'email', "width": "15%" },
            { "data": 'phoneNumber', "width": "15%" },
            { "data": 'company.name', "width": "15%" },
            { "data": 'role', "width": "15%" },
            {
                data: { id: 'id', lockoutEnd:'lockoutEnd' },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockoutEnd = new Date(data.lockoutEnd).getTime();
                    if (lockoutEnd > today) {
                        return ` <div class="text-center" >
                                        <a  onclick=LockUnlock('${data.id}') class="btn btn-danger text-white " style="cursor:pointer;width:100px; ">
                                               <i class="bi bi-lock-fill"></i> lock
                                          </a>
                                           <a  href="/admin/User/RoleManagment?id=${data.id}" class="btn btn-danger text-white " style="cursor:pointer;width:150px; ">
                                               <i class="bi bi-pencil-square"></i> Permission
                                          </a>
                                                                      
                                            </div>`
                    }
                    else {
                        return ` <div class="text-center" >
                                        <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white " style="cursor:pointer;width:100px; ">
                                               <i class="bi bi-unlock-fill"></i> Unlock
                                          </a>
                                           <a href="/admin/User/RoleManagment?id=${data.id}"class="btn btn-danger text-white " style="cursor:pointer;width:150px; ">
                                               <i class="bi bi-pencil-square"></i> Permission
                                          </a>
                                                                      
                                            </div>`
                    }
                    


           
                },
                "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {

    $.ajax({
        type: "POST",
        url: "/admin/User/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
               
                dataTable.ajax.reload();
                toastr.success(data.message);
            }
        }

    });

}



