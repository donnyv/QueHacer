/// <reference path="app.Tasks.js" />

$().ready(function () {
    // globals
    var duedateText = "Add due date";

    // setup
    $("#ntTaskDesc").watermark("Add your task here!", { className: "watermark" });
    $("#ntCategory").watermark("add category", { className: "watermark" });

    // render
    app.Tasks.render(function () {
        
    });

    // page events
    $("#AddTask").on("click", function () {
        $("#Overlay").fadeIn(200);
        $("#NewTask").show("drop", { direction: "up" }, 400);
    });

    $("#StatusFilter").on("click", "li", function () {
        alert("hey"); //:not('.selected')
    });


    // tasks events
    $("#TasksContainer").on("mouseenter", "tr", function () {
        $(this).find(".TaskControls").show();
    }).on("mouseleave", "tr", function () {
        $(".TaskControls").hide();
    });

    $("#TasksContainer").on("click", ".DeleteTask", function () {
        var row = $(this).parents("tr");
        var id = row.attr("data-taskid");
        app.Tasks.Delete(id, function (result) {
            row.fadeOut(300, function () {
                $(this).remove();
            });
        });
    });

    $("#TasksContainer").on("click", ".CompleteTask", function () {
        var row = $(this).parents("tr");
        var id = row.attr("data-taskid");
        var task = app.Tasks.getTask(id);
        task.status = app.Tasks.status.complete;
        app.Tasks.update(task, function (result) {
            row.fadeOut(300, function () {
                $(this).remove();
            });
        });
    });


    // dialog events
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
        ResetDueDate();
    });

    $("#ntSaveClose").on("click", function () {
        SaveTask();
    });

    $("#ntSaveAnother").on("click", function () {
        SaveTask(true);
        ResetDialog();
    });

    $("#ntClose").on("click", CloseDialog);


    // methods
    function SaveTask(NotCloseWhenDone) {
        dueDate = $("#ntDueDate").attr("data-duedate") != "" ? app.Util.DatetoUTC(new Date($("#ntDueDate").attr("data-duedate"))) : 0;

        var taskItem = {
            _id: new ObjectId().toString(),
            task: $.trim($("#ntTaskDesc").val()),
            duedate: dueDate,
            category: $.trim($("#ntCategory").val())
        };

        if (taskItem.task == "")
            return;

        app.Tasks.add(taskItem, function (result) {
            if (!NotCloseWhenDone)
                CloseDialog();
        });
    }
    function ResetDueDate() {
        $("#ntDueDateLabel,#ntRemoveDate").hide();
        $("#ntDueDate").html(duedateText).attr("data-duedate", "");
    }
    function ResetDialog() {
        $("#ntTaskDesc").val("");
        $("#ntCategory").val("")
        ResetDueDate();
    }
    function CloseDialog() {
        $("#Overlay").fadeOut(200);
        $("#NewTask").hide("drop", { direction: "up" }, 300);
        ResetDialog();
    }

    
});