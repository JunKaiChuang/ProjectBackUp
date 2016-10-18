$(document).ready(function () {
    
    $("#grid").kendoGrid({
        dataSource: {
            transport: {
                read: "./GetAllPersonInfo",
                dataType: "json",
                update: {
                    url: "#test",
                    type: "POST"
                }
            }
        },
        height: 550,
        groupable: true,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: true,
            buttonCount: 5
        },
        columns: [{
            field: "userId",
            title: "ID",
            editable: false
        }, {
            field: "userName",
            title: "姓名",
            editable: false
        }, {
            field: "gender",
            title: "性別",
            editable: false
        }, {
            command: { text: "修改", click: updatePerson },
            title: " ",
            width: "150px"
        }, {
            command: { text: "刪除", click: deletePerson },
            title: " ",
            width: "150px"
        }
        ]
    });


    $("#TrainingButton").click(function () {
        $.blockUI({ message: '<h1><img src="/images/busy.gif" /> 訓練中請稍後</h1>' });

        $.ajax({
            type: "POST",
            url: "/FaceService/TrainFace",
            success: function (response) {
                if (response.success) {
                    alert("訓練成功!");
                }
                else {
                    alert(response.message);
                }
                $.unblockUI();
            },
        });
    });
    
});

//轉向修改頁面
function updatePerson(e) {
    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    location.href = "/FaceService/EditPerson?userId=" + dataItem.userId;
    //alert("ID : " + dataItem.userId);
}

//刪除employee
function deletePerson(e) {
    e.preventDefault();
    var tr = $(e.currentTarget).closest("tr");
    var dataItem = this.dataItem(tr);
    var grid = $("#grid").data("kendoGrid");
    if (confirm("是否刪除")) {
        $.blockUI({ message: '<h1><img src="/images/busy.gif" /> 刪除中請稍後</h1>' });
        $.ajax({
            type: "POST",
            url: "/FaceService/DeletePerson",
            data: "userId=" + dataItem.userId,
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    //$(tr).remove();
                    grid.dataSource.remove(dataItem);
                    grid.dataSource.sync();
                    grid.dataSource.page(1);
                    alert("刪除成功!");
                    
                }
                else {
                    alert(response.message);
                }
                $.unblockUI();
            },
        });
        return false;
    }
}