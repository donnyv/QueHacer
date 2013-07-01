/// <reference path="util.js" />

(function () {
    
    // globals
    var tasks = {};
    var templatesURL = "Content/tmpl/main.htm";
    var t = [];
    t.push("TasksList");
    t.push("Tasks");
    t.push("NoTasks");

    var TaskContainer = $("#TasksContainer");

    // controller actions
    function AddTaskPOST(NewTask, callback) {
        var errorMsg = "Missing a parameter!";
        if (NewTask == null) { throw errorMsg; return; }
        if (callback == null) { throw errorMsg; return; }

        $.ajax({
            type: "POST",
            url: "/App/AddTask/",
            dataType: "json",
            data: JSON.stringify(NewTask),
            contentType: 'application/json; charset=utf-8',
            processData: false,
            cache: false,
            success: function (result) {
                callback(result);
            }
        });
    }
    function DeleteTaskPOST(id, callback) {
        var errorMsg = "Missing a parameter!";
        if (NewTask == null) { throw errorMsg; return; }
        if (callback == null) { throw errorMsg; return; }

        var packet = {
            id: id
        };

        $.ajax({
            type: "POST",
            url: "/App/DeleteTask/",
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

    // render methods
    function RenderTasks(callback) {
        app.Util.CompileTemplates(templatesURL, t, tasks, function () {
            if (todoDB.Tasks.length == 0) {
                TaskContainer.empty().append(tasks.tl.NoTasks());
            } else {
                TaskContainer.empty().append(tasks.tl.TasksList({ rows: tasks.tl.Tasks(todoDB.Tasks) }));
            }
            callback();
        });
    }

    var tasksList = {
        add: function (item, callback) {
            AddTaskPOST(item, function (result) {
                if (!result.IsError) {
                    todoDB.Tasks.push(item);
                    RenderTasks();
                }

                callback(result);
            });
        },
        Delete: function (item) {

        },
        filterByTime: function () {

        },
        filterByCategory: function () {

        },
        render: function (callback) {
            RenderTasks(callback);
        }
    };

    if (!window.app)
        window.app = {};

    if (!window.app.Tasks)
        window.app.Tasks = tasksList;
})();