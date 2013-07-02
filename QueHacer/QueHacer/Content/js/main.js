/// <reference path="app.Tasks.js" />
/// <reference path="TextToSpeech.js" />

$().ready(function () {
    // globals
    var duedateText = "Add due date";

    // setup
    app.Tasks.currentFilter = { dateFilter: null, status: null, category: "all" };
    $("#ntTaskDesc").watermark("Add your task here!", { className: "watermark" });
    $("#ntCategory").watermark("add category", { className: "watermark" });
    text2speech.setup();

    // render
    app.Tasks.render(function () {
        
    });

    // page events
    $("#AddTask").on("click", function () {
        $("#Overlay").fadeIn(200);
        $("#NewTask").show("drop", { direction: "up" }, 400);
    });

    $("#TimeFilter").on("click", "li:not('.selected')", function () {
        var dateFilter = $(this).attr("data-type");
        app.Tasks.currentFilter.dateFilter = (dateFilter == "all" ? null : dateFilter);
        app.Tasks.currentFilter.status = null;
        $(this).parent("ul").find("li").removeClass("selected");
        $(this).addClass("selected");
        $("#StatusFilter").find("li").removeClass("selected");

        app.Tasks.render();
    });

    $("#StatusFilter").on("click", "li:not('.selected')", function () {
        app.Tasks.currentFilter.dateFilter = null;
        app.Tasks.currentFilter.status = $(this).attr("data-type");
        $(this).parent("ul").find("li").removeClass("selected");
        $(this).addClass("selected");
        $("#TimeFilter").find("li").removeClass("selected");

        app.Tasks.render();
    });

    $("#FilterTask").on("click", function () {
        $("#CategoryList").show();
    });

    $("#CategoryList").hover(null, function () {
        $("#CategoryList").fadeOut(200);
    });

    $("#CategoryList").on("click", "li", function () {
        app.Tasks.currentFilter = { dateFilter: app.Tasks.currentFilter.dateFilter, status: app.Tasks.currentFilter.status, category: $(this).html() };
        app.Tasks.render();
        $("#FilterTask").html($(this).html());
        $("#CategoryList").hide();
    });

    $("#ReadTasks").on("click", function () {
        if ($("#ReadTasks").html() == "Stop reading") {
            $("#ReadTasks").html("Read tasks");
            text2speech.stop();
        } else {
            text2speech.stop();
            text2speech.start();
            $("#ReadTasks").html("Stop reading");
        }
    });

    // tasks events
    $("#TasksContainer").on("mouseenter", "tr", function () {
        $(this).find(".TaskControls").show();
    }).on("mouseleave", "tr", function () {
        $(".TaskControls").hide();
    });

    $("#TasksContainer").on("click", "tr", function () {
        var id = $(this).attr("data-taskid");
        LoadTask(id);
        $("#Overlay").fadeIn(200);
        $("#NewTask").show("drop", { direction: "up" }, 400);
    });

    $("#TasksContainer").on("click", ".DeleteTask", function (event) {
        event.stopPropagation();

        var row = $(this).parents("tr");
        var id = row.attr("data-taskid");
        app.Tasks.Delete(id, function (result) {
            row.fadeOut(300, function () {
                $(this).remove();
            });
        });
    });

    $("#TasksContainer").on("click", ".CompleteTask", function (event) {
        event.stopPropagation();

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

    $("#ntClose").on("click", function(){
        CloseDialog();   
    });

    $("#ntShare").on("click", function () {
        var id = $("#NewTask").attr("data-taskid");
        $("#ntShareURL").val(app.Util.GetSiteBaseURL() +"/task/" + id);
        $("#ntShareURL").fadeIn(200);
    });

    // methods
    function SaveTask(NotCloseWhenDone) {
        var dueDate = $("#ntDueDate").attr("data-duedate") != "" ? app.Util.DatetoUTC(new Date($("#ntDueDate").attr("data-duedate"))) : 0;
        var id = $("#NewTask").attr("data-taskid") == "" ? new ObjectId().toString() : $("#NewTask").attr("data-taskid");
        var cat = $.trim($("#ntCategory").val()) == "" ? "" : $.trim($("#ntCategory").val()).toLowerCase();

        var taskItem = {
            _id: id,
            task: $.trim($("#ntTaskDesc").val()),
            duedate: dueDate,
            category: cat,
            status: app.Tasks.status.unfinished
        };

        if (taskItem.task == "")
            return;

        if ($("#NewTask").attr("data-taskid") == "") {
            app.Tasks.add(taskItem, function (result) {
                if (!NotCloseWhenDone)
                    CloseDialog();
            });
        } else {
            app.Tasks.update(taskItem, function (result) {
                if (!NotCloseWhenDone)
                    CloseDialog();

                app.Tasks.render();
            });
        }
    }
    function LoadTask(id) {
        
        var taskItem = app.Tasks.getTask(id);
        $("#NewTask").attr("data-taskid", taskItem._id);
        $("#ntTaskDesc").val(taskItem.task);

        if (taskItem.duedate != 0) {
            var date = app.Util.UTCtoDate(taskItem.duedate);
            $("#ntDueDateLabel").show();
            $("#ntDueDate").html(date).attr("data-duedate", date);
        }

        $("#ntCategory").val(taskItem.category)

        if (taskItem.status == app.Tasks.status.complete) {
            $("#ntStatus").show();
            $("#ntSaveClose,#ntSaveAnother").hide();
        }

        $("#ntShare").show();
    }
    function ResetDueDate() {
        $("#ntDueDateLabel,#ntRemoveDate").hide();
        $("#ntDueDate").html(duedateText).attr("data-duedate", "");
    }
    function ResetDialog() {
        $("#ntTaskDesc").val("");
        $("#ntCategory").val("")
        ResetDueDate();
        $("#ntStatus").hide();
        $("#ntSaveClose,#ntSaveAnother").show();
        $("#NewTask").attr("data-taskid", "");
        $("#ntShare,#ntShareURL").hide();
        $("#ntShareURL").val("");
    }
    function CloseDialog() {
        $("#Overlay").fadeOut(200);
        $("#NewTask").hide("drop", { direction: "up" }, 300, function () {
            ResetDialog();
        });
        
    }

    
});