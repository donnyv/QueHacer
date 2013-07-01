/// <reference path="app.Tasks.js" />

$().ready(function () {
    // globals
    var duedateText = "Add due date";

    // setup
    $("#ntTaskDesc").watermark("Add your task here!", { className: "watermark" });
    $("#ntCategory").watermark("add category", { className: "watermark" });

    // render
    app.Tasks.render(function () {
        // events
        $("#TasksContainer tr").mouseenter(function () {
            $(this).find(".TaskControls").show();

        }).mouseleave(function () {
            $(".TaskControls").hide();
        });

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
            ResetDueDate();
        });

        $("#ntSaveClose").on("click", function () {
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
                CloseDialog();
            });
        });

        $("#ntSaveAnother").on("click", function () {

        });

        $("#ntClose").on("click", CloseDialog);
    });

    // methods
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