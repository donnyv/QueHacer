$().ready(function () {

    // setup
    $("#ntTaskDesc").watermark("Add your task here!", { className: "watermark" });
    $("#ntCategory").watermark("add category", { className: "watermark" });

    // events
    $("#AddTask").on("click", function () {
        $("#Overlay").fadeIn(200);
        $("#NewTask").show("drop", { direction: "up" }, 400);
    });

    $("#ntDueDate").on("click", function () {
        $("#datepicker").show().datepicker({
            onSelect: function (date) {
                $("#ntDueDateLabel,#ntRemoveDate").show();
                $("#ntDueDate").html(date).attr("data-duedate", date);
                $("#datepicker").hide();
            }
        });
    });

    $("#ntRemoveDate").on("click", function () {
        $("#ntDueDateLabel,#ntRemoveDate").hide();
        $("#ntDueDate").html("Add due date").attr("data-duedate", "");;
    });

    $("#ntSaveClose").on("click", function () {
        
        var taskItem = {
            _id: new ObjectId().toString(),
            task: $.trim($("#ntTaskDesc").val()),
            duedate: $("#ntDueDate").attr("data-duedate") ? 0 : 1,
            category: $.trim($("#ntCategory").val())
        };

        if (taskItem.task == "")
            return;

        AddTask(taskItem, function (result) {
            ResetDialog();
            CloseDialog();
        });
    });

    $("#ntSaveAnother").on("click", function () {

    });

    $("#ntClose").on("click", CloseDialog);

    // methods
    function ResetDialog() {

    }
    function CloseDialog() {
        $("#Overlay").fadeOut(200);
        $("#NewTask").hide("drop", { direction: "up" }, 300);
    }

    // actions
    function AddTask(NewTask, callback) {
        var errorMsg = "Missing a parameter!";
        if (NewTask == null) { throw errorMsg; return; }
        if (callback == null) { throw errorMsg; return; }

        var packet = {
            NewTask: NewTask
        };

        $.ajax({
            type: "POST",
            url: "/App/AddTask/",
            dataType: "json",
            data: JSON.stringify(packet),
            contentType: 'application/json; charset=utf-8',
            processData: false,
            cache: false,
            success: function (result) {
                callback(result);
            }
        });
    }
});